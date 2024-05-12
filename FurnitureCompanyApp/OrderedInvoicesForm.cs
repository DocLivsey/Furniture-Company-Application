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
        }
        
        private void UploadFromDataBase(string sqlQuery)
        {
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlQuery, Connection);
            Set.Reset(); dataAdapter.Fill(Set);
            Table = Set.Tables[0];
            dataGridView1.DataSource = Table;
            dataGridView1.Columns[0].HeaderText = "Invoice ID";
            dataGridView1.Columns[1].HeaderText = "Order Date";
            dataGridView1.Columns[2].HeaderText = "Receiving Date";
            dataGridView1.Columns[3].HeaderText = "Delivery Cost";
            dataGridView1.Columns[4].HeaderText = "Manufacturing Cost";
            dataGridView1.Columns[5].HeaderText = "Components count on receive";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void OrderedInvoices_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UploadFromDataBase("Select * From Receiving_Invoices");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OrderComponentsForm form = new OrderComponentsForm(Connection);
            form.Show();
            form.StartPosition = FormStartPosition.CenterScreen;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void OrderedInvoices_Activated(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                UploadFromDataBase("Select * From Receiving_Invoices");
            }
            else if (radioButton2.Checked)
            {
                UploadFromDataBase("Select * From Receiving_Invoices");
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