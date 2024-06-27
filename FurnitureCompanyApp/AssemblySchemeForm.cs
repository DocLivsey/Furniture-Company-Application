using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class AssemblySchemeForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private DataTable Table = new DataTable(); 
        private DataSet Set = new DataSet();
        
        public AssemblySchemeForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
            label1.Visible = false;
            textBox1.Visible = false;
        }

        private void AssemblySchemeForm_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UploadFromDataBase($"Select * From {Constants.DatabaseTable.RequiredComponentsTable}");
        }
        
        private void UploadFromDataBase(string sqlQuery)
        {
            NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlQuery, Connection);
            Set.Reset(); dataAdapter.Fill(Set);
            Table = Set.Tables[0];
            dataGridView1.DataSource = Table;
            dataGridView1.Columns[0].HeaderText = "Код схемы";
            dataGridView1.Columns[1].HeaderText = "Номер списка необходимых для сборки комплектующих";
        }

        private void AssemblySchemeForm_Activated(object sender, EventArgs e)
        {
            UploadFromDataBase($"Select * From {Constants.DatabaseTable.RequiredComponentsTable}");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var row = dataGridView1.SelectedRows[0];
            if (!(row is null))
            {
                var fields = row.Cells;
                var schemeId = Convert.ToInt32(fields[0].Value);
                var componentId = Convert.ToInt32(fields[1].Value);
                var requiredAmount = Convert.ToInt32(fields[2].Value);

                Scheme scheme = new Scheme(schemeId, componentId, requiredAmount);
                ChangeAssemblySchemeForm form = new ChangeAssemblySchemeForm(Connection, scheme);
                form.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CreateAssemblySchemeForm form = new CreateAssemblySchemeForm(Connection);
            form.Show();
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            button2.Enabled = dataGridView1.SelectedRows.Count != 0;
        }
    }
}