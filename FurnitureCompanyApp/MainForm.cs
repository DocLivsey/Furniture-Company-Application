using System;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class MainForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        
        public MainForm()
        {
            InitializeComponent();
            button1.Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            Connection = new NpgsqlConnection($"Server={Constants.Connection.LocalServer}; " +
                                              $"Port={Constants.Connection.Port}; " + 
                                              $"UserID={Constants.Connection.Userid}; " +
                                              $"Password={Constants.Connection.Password}; " +
                                              $"Database={Constants.Connection.DatabaseName}");
            Connection.Open();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                switch (e.Node.Text)
                {
                    case "Заказ компонентов":
                        ReceiveComponentsForm componentsForm = new ReceiveComponentsForm(Connection);
                        componentsForm.Show();
                        componentsForm.StartPosition = FormStartPosition.CenterScreen;
                        break;
                    
                    case "Сделанные заказы":
                        OrderedInvoicesForm form = new OrderedInvoicesForm(Connection);
                        form.Show();
                        form.StartPosition = FormStartPosition.CenterScreen;
                        break;
                    
                    case "Заказать сборку":
                        break;
                    
                    case "Комплектующие":
                        break;
                    
                    case "Собранная мебель":
                        break;
                    
                    case "Сборка мебели":
                        break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test test = new Test(Connection);
            test.Show();
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            treeView1.Width = 230;
        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if (!Program.IsAnyNodeExpanded(treeView1))
                treeView1.Width = 105;
        }
    }
}