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
        private bool ValidInput { get; set; }
        private NpgsqlConnection Connection { get; set; }
        
        public OrderComponentsForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }

        private void OrderComponentsForm_Load(object sender, EventArgs e)
        {
            UpdateFormSates();
        }
        
        private void OrderComponentsForm_Activated(object sender, EventArgs e)
        {
            UpdateFormSates();
        }
        
        private void UpdateFormSates()
        {
            ValidateInput();
            UpdateComboBox();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim().Length != 0 &&
                textBox1.Text.Trim().Length != 0 &&
                textBox2.Text.Trim().Length != 0 &&
                ValidInput)
            {
                Random random = new Random();
                int componentsId = int.Parse(comboBox1.Text);
                int count = int.Parse(textBox2.Text);
                string componentsName = textBox1.Text;
                string orderDate = $"{DateTime.Today.Date}";
                string deliveryDate = dateTimePicker1.Value.Date.ToString();
                double manufacturing = random.NextDouble() * random.Next(100, 10000) * count;
                double delivery = random.NextDouble() * manufacturing * count;
                
                ReceiveInvoice invoice = new ReceiveInvoice(
                    orderDate, deliveryDate, delivery, manufacturing, count);
                InvoicesQuery.InsertIntoReceivingTable(invoice, Connection, false);

                FurnitureComponent component;
                var map = QueryTools.SelectFromTableWhere(
                    "amount", $"_id = {componentsId}", 
                    Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                if (map.Count > 0)
                {
                    int oldAmount = Convert.ToInt32(map[0]["amount"]);
                    QueryTools.UpdateTable($"amount = {oldAmount + count}", $"_id = {componentsId}", 
                        Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                    component = FurnitureComponent.GetComponentFromDatabase(componentsId, Connection);
                }
                else
                {
                    component = new FurnitureComponent(
                        componentsId, componentsName, orderDate, count);
                    ProductsQuery.InsertIntoComponentsTable(component, Connection);
                }
                InvoicesAndProductsQuery.InsertIntoLinkTable(invoice, component, Connection);
            }
            else
            {
                if (comboBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(comboBox1, "Введите пожалуйста код комплектующего");
                }
                if (textBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox1, "Введите пожалуйста наименование комплектующего");
                }
                if (textBox2.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox2, "Введите пожалуйста количество комплектующего в заказе");
                }
            }
            UpdateFormSates();
            Close();
        }
        
        private void ValidateInput()
        {
            button1.Enabled = ValidInput;
        }
        
        private void SetToolTip(Control box, string text)
        {
            Session.FormsAction.SetErrorToolTip(box, text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox1, Session.FormsAction.ValidationMode.NameValidation,
                "Наименование не должно содержать пробелов");
            ValidateInput();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox2, Session.FormsAction.ValidationMode.IdValidation,
                "количество должно быть целым числом");
            ValidateInput();
        }
        
        private void UpdateComboBox()
        {
            var map = QueryTools.SimpleSelectFromTable(
                "_id", Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
            foreach (var row in map)
                foreach (var value in row.Values)
                    if (!comboBox1.Items.Contains(value))
                        comboBox1.Items.Add(value);
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var map = QueryTools.SelectFromTableWhere(
                "name", $"_id = {comboBox1.SelectedItem}",
                Constants.DatabaseTable.ComponentsWarehouseTable, Connection)[0];
            textBox1.Text = map["name"].ToString();
            textBox1.Enabled = false;
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            if (Session.FormsAction.ValidateBoxInput(
                    comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                    "Код комплектующего должен быть целым числом"))
                if (!comboBox1.Items.Contains(Convert.ToInt32(comboBox1.Text)))
                {
                    textBox1.Text = "";
                    textBox1.Enabled = true;
                }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                "Код комплектующего должен быть целым числом");
            ValidateInput();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}