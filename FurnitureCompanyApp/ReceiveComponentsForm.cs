using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class ReceiveComponentsForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private bool ValidInput { get; set; }
        private DataTable Table = new DataTable(); 
        private DataSet Set = new DataSet();
        
        public ReceiveComponentsForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }

        private void UpdateFormSates()
        {
            ResetDataGridViewSource();
            UpdateComboBox();
            ValidateInput();
        }
        
        private void ReceiveComponentsForm_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UpdateFormSates();
            //dataGridView1.EditingControl.Enabled = false;
        }

        private void ResetDataGridViewSource()
        {
            DataSet temporaryDataSet = new DataSet();
            NpgsqlDataAdapter dataAdapter = UploadFromDataBase();
            if (dataAdapter != null)
            {
                temporaryDataSet.Reset();
                dataAdapter.Fill(temporaryDataSet);
                if (!temporaryDataSet.Equals(Set))
                {
                    Set.Reset(); dataAdapter.Fill(Set);
                    Table = Set.Tables[0];
                    dataGridView1.DataSource = Table;
                    dataGridView1.Columns[0].HeaderText = "Invoice ID";
                    dataGridView1.Columns[1].HeaderText = "Order Date";
                    dataGridView1.Columns[2].HeaderText = "Receiving Date";
                    dataGridView1.Columns[3].HeaderText = "Delivery Cost";
                    dataGridView1.Columns[4].HeaderText = "Manufacturing Cost";
                    dataGridView1.Columns[5].HeaderText = "Components\n" + "count on receive";
                    dataGridView1.Columns[6].HeaderText = "Component ID";
                    dataGridView1.Columns[7].HeaderText = "Name";
                    dataGridView1.Columns[8].HeaderText = "Manufacture Date";
                    dataGridView1.Columns[9].HeaderText = "Amount";
                }
            }
        }
        
        private NpgsqlDataAdapter UploadFromDataBase()
        {
            try
            {
                string sqlQuery = "Select * From Receiving_Invoices " +
                                  "join components_warehouse wh on wh._id = (" +
                                  "SELECT ic._component_id from invoices_for_components ic " +
                                  "where receiving_invoices._id = ic.invoice_id)";
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlQuery, Connection);
                return dataAdapter;
            }
            catch (Exception e)
            {
                Console.WriteLine($"exception: {e}");
                Console.WriteLine($"Message: {e.Message}");
                return null;
            }
        }
        
        private void ValidateInput()
        {
            button1.Enabled = ValidInput;
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

        private void SetToolTip(Control box, string text)
        {
            Session.FormsAction.SetErrorToolTip(box, text);
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if ((comboBox1.Text.Trim().Length != 0) &&
                (textBox1.Text.Trim().Length != 0) &&
                (textBox2.Text.Trim().Length != 0) &&
                (textBox3.Text.Trim().Length != 0) &&
                (textBox4.Text.Trim().Length != 0) &&
                (textBox5.Text.Trim().Length != 0) &&
                ValidInput)
            {
                int invoiceId = int.Parse(textBox5.Text);
                int componentsId = int.Parse(comboBox1.Text);
                int count = int.Parse(textBox3.Text);
                
                double manufacturing = double.Parse(textBox2.Text);
                double delivery = double.Parse(textBox4.Text);
                
                string componentsName = textBox1.Text;
                string orderDate = dateTimePicker1.Value.Date.ToString();
                string receiveDate = dateTimePicker2.Value.Date.ToString();
                
                ReceiveInvoice invoice = new ReceiveInvoice(
                    orderDate, receiveDate, delivery, manufacturing, count, invoiceId);
                InvoicesQuery.InsertIntoReceivingTable(invoice, Connection, true);

                var map = QueryTools.SelectFromTableWhere("amount", 
                    $"_id = {componentsId}", Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                FurnitureComponent component;
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
                    SetToolTip(textBox2, "Введите пожалуйста стоимость изготовления комплектующего");
                }
                if (textBox3.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox3, "Введите пожалуйста количество комплектующего в заказе");
                }
                if (textBox4.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox4, "Введите пожалуйста стоимость доставки комплектующего");
                }
                if (textBox5.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox5, "Введите пожалуйста код накладной соответсвующий данной доствке");
                }
            }
            
            UpdateFormSates();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            var row = dataGridView1.SelectedRows[0];
            if (!(row is null))
            {
                var fields = row.Cells;
                var invoiceId = Convert.ToInt32(fields[0].Value);
                var orderDate = DateTime.Parse(fields[1].Value.ToString()).Date;
                var receivingDate = DateTime.Parse(fields[2].Value.ToString()).Date;
                var deliveryCost = Convert.ToDouble(fields[3].Value);
                var manufacturingCost = Convert.ToDouble(fields[4].Value);
                var componentsCount = Convert.ToInt32(fields[5].Value);

                var componentId = Convert.ToInt32(fields[6].Value);
                var componentsName = fields[7].Value.ToString();
                var manufactureDate = fields[8].Value.ToString();
                var amount = Convert.ToInt32(fields[9].Value);
                
                ReceiveInvoice invoice = new ReceiveInvoice(orderDate.ToString(), receivingDate.ToString(),
                    deliveryCost, manufacturingCost, componentsCount, invoiceId);
                FurnitureComponent component = new FurnitureComponent(
                    componentId, componentsName, manufactureDate, amount);
                EditInvoiceForm form = new EditInvoiceForm(Connection, invoice, component, true);
                form.Show();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OrderComponentsForm form = new OrderComponentsForm(Connection);
            form.Show();
            form.StartPosition = FormStartPosition.CenterScreen;
        }
        
        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void label2_Click(object sender, EventArgs e)
        {
            
        }

        private void label3_Click(object sender, EventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox1, Session.FormsAction.ValidationMode.NameValidation,
                "Наименование не должно содержать пробелов");
            ValidateInput();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox2, Session.FormsAction.ValidationMode.PriceValidation,
                "Стоимость изготовления должна быть целым числом или дестяичной дробью");
            ValidateInput();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox3, Session.FormsAction.ValidationMode.IdValidation,
                "количество должно быть целым числом");
            ValidateInput();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox4, Session.FormsAction.ValidationMode.PriceValidation,
                "Стоимость доставки должна быть целым числом или дестяичной дробью");
            ValidateInput();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                "Код комплектующего должен быть целым числом");
            ValidateInput();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox5, Session.FormsAction.ValidationMode.IdValidation,
                "Код накладной должен быть целым числом");
            ValidateInput();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                button2.Enabled = dataGridView1.SelectedRows.Count != 0;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ReceiveComponentsForm_Activated(object sender, EventArgs e)
        {
            try
            {
                UpdateFormSates();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}