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
            //button1.Visible = false;
        }
        
        private bool AreAllNodesExpanded(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
                if (!node.IsExpanded)
                    return false;
            return true;
        }

        private bool IsAnyNodeExpanded(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
                if (node.IsExpanded)
                    return true;
            return false;
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
                        OrderedInvoicesForm invoicesForm = new OrderedInvoicesForm(Connection);
                        invoicesForm.Show();
                        invoicesForm.StartPosition = FormStartPosition.CenterScreen;
                        break;
                    
                    case "Заказать сборку":
                        AssemblyInvoiceForm assemblyInvoiceForm = new AssemblyInvoiceForm(Connection);
                        assemblyInvoiceForm.Show();
                        assemblyInvoiceForm.StartPosition = FormStartPosition.CenterParent;
                        break;
                    
                    case "Комплектующие":
                        ComponentsInStockForm componentsInStockForm = new ComponentsInStockForm(Connection);
                        componentsInStockForm.Show();
                        componentsInStockForm.StartPosition = FormStartPosition.CenterScreen;
                        break;
                    
                    case "Мебель":
                        FurnitureInStockForm furnitureInStockForm = new FurnitureInStockForm(Connection);
                        furnitureInStockForm.Show();
                        furnitureInStockForm.StartPosition = FormStartPosition.CenterParent;
                        break;
                    
                    case "Сборка мебели":
                        AssemblySchemeForm schemeForm = new AssemblySchemeForm(Connection);
                        schemeForm.Show();
                        schemeForm.StartPosition = FormStartPosition.CenterScreen;
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
            if (!IsAnyNodeExpanded(treeView1))
                treeView1.Width = 105;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }
    }
}