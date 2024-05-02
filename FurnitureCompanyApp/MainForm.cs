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
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.IsSelected)
            {
                switch (e.Node.Text)
                {
                    case "Заказ компонентов":
                        ReceiveComponentsForm form = new ReceiveComponentsForm();
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

        private void Open_Node4(object sender, TreeNodeMouseClickEventArgs e)
        {
            var form = new ReceiveComponentsForm();
            form.Show();
        }
    }
}