using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Npgsql;
using Excel = Microsoft.Office.Interop.Excel;

namespace FurnitureCompanyApp
{
    public partial class FurnitureInStockForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private DataTable Table = new DataTable(); 
        private DataSet Set = new DataSet();
        
        public FurnitureInStockForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
            textBox1.Visible = false;
            label1.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
        }
        
        private void FurnitureInStockForm_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UploadFromDataBase($"Select * From {Constants.DatabaseTable.FurnitureWarehouseTable}");
        }
        
        private void FurnitureInStockForm_Activated(object sender, EventArgs e)
        {
            UploadFromDataBase($"Select * From {Constants.DatabaseTable.FurnitureWarehouseTable}");
        }
        
        private void UploadFromDataBase(string sqlQuery)
        {
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlQuery, Connection);
            Set.Reset(); dataAdapter.Fill(Set);
            Table = Set.Tables[0];
            dataGridView1.DataSource = Table;
            dataGridView1.Columns[0].HeaderText = "Код мебели";
            dataGridView1.Columns[1].HeaderText = "Код Схемы";
            dataGridView1.Columns[2].HeaderText = "Название мебели";
            dataGridView1.Columns[3].HeaderText = "Количество на складе";
            dataGridView1.Columns[4].HeaderText = "Итоговая стоимость";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AssemblyInvoiceForm form = new AssemblyInvoiceForm(Connection);
            form.Show();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var periodBegin = dateTimePicker1.Value.Date;
            var periodEnd = dateTimePicker2.Value.Date;
            var rows = dataGridView1.SelectedRows;
            var list = new List<int>();
            foreach (DataGridViewRow row in rows)
            {
                Console.WriteLine($"row: {row}, furniture_id = {row.Cells[0].Value}");
                list.Add(Convert.ToInt32(row.Cells[0].Value));
            }

            var listOfDict = new List<Dictionary<int, List<object>>>();
            foreach (var id in list)
            {
                var joinedTable = $"{Constants.DatabaseTable.FurnitureWarehouseTable} fw " +
                                  $"join {Constants.DatabaseTable.RequiredComponentsTable} rc on fw.scheme_id = rc.scheme_id " +
                                  $"join {Constants.DatabaseTable.ComponentsWarehouseTable} cw on rc.component_id = cw._id " +
                                  $"join {Constants.DatabaseTable.InvoiceAndComponentLinkTable} ifc on cw._id = ifc._component_id " +
                                  $"join {Constants.DatabaseTable.ReceivingInvoicesTable} ri on ifc.invoice_id = ri._id ";
                var sql = $"select receiving_date, components_count from {joinedTable} where furniture_id = {id}";
            
                var map = QueryTools.SelectFromTableWhere(
                    "furniture_id, furniture_name, component_id, name, receiving_date," +
                    " components_count, required_amount",
                    $"furniture_id = {id}", joinedTable, Connection);
                int componentId;
                var dict = new Dictionary<int, List<object>>();
                foreach (var match in map)
                {
                    componentId = Convert.ToInt32(match["component_id"]);
                    if (dict.ContainsKey(componentId))
                    {
                        if(DateTime.Parse(match["receiving_date"].ToString()) == periodBegin)
                            dict[componentId][2] = (int)dict[componentId][2] + Convert.ToInt32(match["components_count"]);
                        
                        if (DateTime.Parse(match["receiving_date"].ToString()) == periodEnd)
                            dict[componentId][3] = (int)dict[componentId][3] + Convert.ToInt32(match["components_count"]);
                    }
                    else
                    {
                        var before = 0;
                        var after = 0;
                        if(DateTime.Parse(match["receiving_date"].ToString()).Date == periodBegin)
                            before = Convert.ToInt32(match["components_count"]) + Convert.ToInt32(match["required_amount"]);
                        if (DateTime.Parse(match["receiving_date"].ToString()).Date == periodEnd)
                            after = Convert.ToInt32(match["components_count"]);
                        else if (DateTime.Parse(match["receiving_date"].ToString()).Date <= periodEnd)
                            after = Convert.ToInt32(match["components_count"]);
                        dict.Add(componentId, new List<object>()
                        {
                            match["furniture_name"], match["name"], before, after 
                        });
                    }
                }
                listOfDict.Add(dict);
            }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            String fileName = ofd.FileName;
            
            dynamic excel = Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application", true));
            excel.Visible = true;

            Excel.Workbook workbook = excel.Workbooks.Open(fileName, 0, false, 5);
            Excel.Worksheet worksheet = (Excel.Worksheet)excel.ActiveSheet;
            worksheet.Cells[1, "A"] = "Нименование мебели";
            worksheet.Cells[1, "B"] = "Наименование комплектующего";
            worksheet.Cells[1, "C"] = "Количество на " + periodBegin;
            worksheet.Cells[1, "D"] = "Количество на " + periodEnd;

            int i = 2;
            foreach (var dict in listOfDict)
            {
                foreach (var line in dict)
                {
                    int j = 1;
                    foreach (var obj in line.Value)
                    {
                        Console.Write(obj + " ");
                        worksheet.Cells[i, j] = obj;
                        j++;
                    }
                    Console.WriteLine();
                    i++;
                }
                i++;
            }
            
            ((Excel.Range)worksheet.Columns[1]).AutoFit();
            ((Excel.Range)worksheet.Columns[2]).AutoFit();
            ((Excel.Range)worksheet.Columns[3]).AutoFit();
            ((Excel.Range)worksheet.Columns[4]).AutoFit();
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            button1.Enabled = dataGridView1.SelectedRows.Count != 0;
        }
    }
}