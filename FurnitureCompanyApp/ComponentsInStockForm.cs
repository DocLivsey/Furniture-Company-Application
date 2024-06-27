using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class ComponentsInStockForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private DataTable Table = new DataTable(); 
        private DataSet Set = new DataSet();
        
        public ComponentsInStockForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }
        
        private void ComponentsInStockForm_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UploadFromDataBase($"Select * From {Constants.DatabaseTable.ComponentsWarehouseTable}");
        }
        
        private void UploadFromDataBase(string sqlQuery)
        {
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlQuery, Connection);
            Set.Reset(); dataAdapter.Fill(Set);
            Table = Set.Tables[0];
            dataGridView1.DataSource = Table;
            dataGridView1.Columns[0].HeaderText = "Код комплектующего";
            dataGridView1.Columns[1].HeaderText = "Название комплектующего";
            dataGridView1.Columns[2].HeaderText = "Дата изготовления";
            dataGridView1.Columns[3].HeaderText = "Количество на складе";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OrderComponentsForm form = new OrderComponentsForm(Connection);
            form.Show();
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void ComponentsInStockForm_Activated(object sender, EventArgs e)
        { 
            UploadFromDataBase($"Select * From {Constants.DatabaseTable.ComponentsWarehouseTable}");
        }
    }
}