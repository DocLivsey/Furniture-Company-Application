using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            Connection = new NpgsqlConnection($"Server={Constants.LocalServer}; " +
                                              $"Port={Constants.Port}; " + 
                                              $"UserID={Constants.Userid}; " +
                                              $"Password={Constants.Password}; " +
                                              $"Database={Constants.DatabaseName}");
            Connection.Open();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                switch (e.Node.Text)
                {
                    case "Заказ компонентов":
                        ReceiveComponentsForm form = new ReceiveComponentsForm(Connection);
                        form.Show();
                        break;
                    
                    case "Сделанные заказы":
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
    }
}