using System;
using System.Windows.Forms;
using System.Data;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class OrderedInvoicesForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private DataTable Table = new DataTable(); 
        private DataSet Set = new DataSet();
        
        public OrderedInvoicesForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
            textBox1.Visible = false;
            label1.Visible = false;
        }
        
        private void UploadFromDataBase(string sqlQuery)
        {
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlQuery, Connection);
            Set.Reset(); dataAdapter.Fill(Set);
            Table = Set.Tables[0];
            dataGridView1.DataSource = Table;
            dataGridView1.Columns[0].HeaderText = "Код накладной";
            dataGridView1.Columns[1].HeaderText = "Дата заказа";
            dataGridView1.Columns[2].HeaderText = "Дата доставки";
            dataGridView1.Columns[3].HeaderText = "Стоимость доставки";
            dataGridView1.Columns[4].HeaderText = "Стоимость изготовления";
            dataGridView1.Columns[5].HeaderText = "Количество комплектующих с доставки";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void OrderedInvoices_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UploadFromDataBase("Select * From Receiving_Invoices");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var row = dataGridView1.SelectedRows[0];
            if (!(row is null))
            {
                var fields = row.Cells;
                var invoiceId = Convert.ToInt32(fields[0].Value);
                var map = QueryTools.SelectFromTableWhere("component_id",
                    $"invoice_id = {invoiceId}", Constants.DatabaseTable.InvoiceAndComponentLinkTable, Connection)[0];
                var componentId = Convert.ToInt32(map["component_id"]);
                ReceiveInvoice invoice = new ReceiveInvoice(fields[1].Value.ToString(),
                    fields[2].Value.ToString(), Convert.ToDouble(fields[3].Value),
                    Convert.ToDouble(fields[4].Value), Convert.ToInt32(fields[5].Value),
                    invoiceId);
                map = QueryTools.SelectFromTableWhere("name, amount",
                    $"_id = {componentId}", Constants.DatabaseTable.ComponentsWarehouseTable, Connection)[0];
                var name = map["name"].ToString();
                var amount = Convert.ToInt32(map["amount"]);
                FurnitureComponent component = new FurnitureComponent(componentId, name,
                    fields[1].Value.ToString(), amount);
                EditInvoiceForm form = new EditInvoiceForm(Connection, invoice, component, false);
                form.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OrderComponentsForm form = new OrderComponentsForm(Connection);
            form.Show();
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                try
                {
                    dataGridView1.DataSource = Table;
            
                    DateTime today = DateTime.Today.Date;
                    DataTable dataTable = (DataTable)dataGridView1.DataSource;
                    DataView dataView = dataTable.DefaultView;

                    dataGridView1.Columns[4].HeaderText = "Receiving_Date";
            
                    var filteredRows = dataView.Table.AsEnumerable()
                        .Where(row => DateTime.Parse(row.Field<string>("Receiving_Date")) <= today);
                    dataTable = filteredRows.CopyToDataTable();
                    dataGridView1.DataSource = dataTable;

                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show(
                        "На данный момент все заказы уже доставлены",
                        "Ooops...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    radioButton1.Checked = false;
                }
                dataGridView1.Columns[4].HeaderText = "Receiving Date";
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                try
                {
                    dataGridView1.DataSource = Table;
            
                    DateTime today = DateTime.Today.Date;
                    DataTable dataTable = (DataTable)dataGridView1.DataSource;
                    DataView dataView = dataTable.DefaultView;

                    dataGridView1.Columns[4].HeaderText = "Receiving_Date";
            
                    var filteredRows = dataView.Table.AsEnumerable()
                        .Where(row => DateTime.Parse(row.Field<string>("Receiving_Date")) > today);
                    dataTable = filteredRows.CopyToDataTable();
                    dataGridView1.DataSource = dataTable;

                }
                catch (InvalidOperationException exception)
                {
                    MessageBox.Show(
                        "На данный момент нет доставленных заказов",
                        "Ooops...",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    radioButton2.Checked = false;
                }
                dataGridView1.Columns[4].HeaderText = "Receiving Date";
            }
        }
        
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = Table;
        }

        private void OrderedInvoices_Activated(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                radioButton1_CheckedChanged(sender, e);
            }
            else if (radioButton2.Checked)
            {
                radioButton2_CheckedChanged(sender, e);
            }
            else
            {
                UploadFromDataBase("Select * From Receiving_Invoices");
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            button2.Enabled = dataGridView1.SelectedRows.Count != 0;
        }
    }
}