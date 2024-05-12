using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class OrderComponentsForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        
        public OrderComponentsForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }

        private void OrderComponentsForm_Load(object sender, EventArgs e)
        {
            
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            int componentsId, count;
            try
            {
                componentsId = int.Parse(comboBox1.Text);
                count = int.Parse(textBox2.Text);
                string componentsName = textBox1.Text;
                string orderDate = $"{DateTime.Today.Date}";
                string deliveryDate = dateTimePicker1.Text;
                double manufacturing = random.NextDouble() * random.Next(100, 10000) * count;
                double delivery = random.NextDouble() * manufacturing * count;
                
                ReceiveInvoice invoice = new ReceiveInvoice(
                    orderDate, deliveryDate, delivery, manufacturing, count);
                Console.WriteLine(invoice.Id);
                /*InvoicesQuery.InsertIntoReceivingTable(invoice, Connection);
                invoice.SetIdFromDataBase(Connection);
                
                var map = QueryTools.SelectFromTableWhere("amount", 
                    $"_id = {componentsId}","components_warehouse", Connection);
                if (map.Count > 0)
                {
                    int oldAmount = Convert.ToInt32(map[0]["_amount"]);
                    QueryTools.UpdateTable($"amount = {oldAmount + count}", $"_id = {componentsId}", 
                        "components_warehouse", Connection);
                }
                else
                {
                    FurnitureComponent component = new FurnitureComponent(
                        componentsId, componentsName, orderDate, count);
                    ProductsQuery.InsertIntoComponentsTable(component, Connection);
                    InvoicesAndProductsQuery.InsertIntoLinkTable(invoice, component, Connection);
                }
                */

                var map = QueryTools.SimpleSelectFromTable(
                    "_id", Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                foreach (var row in map)
                    foreach (var value in row.Values)
                        comboBox1.Items.Add(value);
            }
            catch (Exception ignored)
            {
                MessageBox.Show("Введите число", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ValidateInput(Control box, Session.ValidationMode mode)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(textBox1, Session.ValidationMode.NameValidation);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(textBox2, Session.ValidationMode.IdValidation);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("CHANGE INDEX!");
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Console.WriteLine("COMMITED!");
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ValidateInput(comboBox1, Session.ValidationMode.IdValidation);
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void contextMenu1_Popup(object sender, EventArgs e)
        {
            
        }
    }
}