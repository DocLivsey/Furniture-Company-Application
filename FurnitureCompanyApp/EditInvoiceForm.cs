using System;
using System.Drawing;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class EditInvoiceForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private bool ValidInput { get; set; }
        private bool ShowExtendedTable { get; set; }
        private ReceiveInvoice ChangeableInvoice { get; set; }
        private FurnitureComponent ChangeableComponent { get; set; } 
        
        public EditInvoiceForm(NpgsqlConnection connection, ReceiveInvoice invoice,
            FurnitureComponent component, bool showExtendedTable)
        {
            InitializeComponent();
            Connection = connection;
            ChangeableInvoice = invoice;
            ChangeableComponent = component;
            ShowExtendedTable = showExtendedTable;
        }

        private void EditInvoiceForm_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent; 
            /*tableLayoutPanel2.Visible = ShowExtendedTable;
            if (!ShowExtendedTable)
                tableLayoutPanel1.Location = new Point(105, 12);*/
            if (!ShowExtendedTable)
                panel8.Enabled = false;
            UpdateFormSates();
        }
        
        private void UpdateFormSates()
        {
            UpdateTextFields();
            ValidateInput();
        }
        
        private void UpdateTextFields()
        {
            textBox1.Text = ChangeableComponent.ComponentsName;
            dateTimePicker2.Value = DateTime.Parse(ChangeableInvoice.ReceivingDate);
            textBox3.Text = ChangeableInvoice.ComponentsCount.ToString();
            textBox2.Text = ChangeableInvoice.ManufacturingCost.ToString();
            dateTimePicker1.Value = DateTime.Parse(ChangeableComponent.ManufactureDate);
            textBox4.Text = ChangeableInvoice.DeliveryCost.ToString();
        }
        
        private void ValidateInput()
        {
            button2.Enabled = ValidInput;
        }
        
        private void SetToolTip(Control box, string text)
        {
            Session.FormsAction.SetErrorToolTip(box, text);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length != 0 &&
                textBox2.Text.Trim().Length != 0 &&
                textBox3.Text.Trim().Length != 0 &&
                textBox4.Text.Trim().Length != 0 &&
                ValidInput)
            {
                var componentName = textBox1.Text;
                var manufactureCost = textBox2.Text;
                var deliveryCost = textBox4.Text;
                var receiveCount = Convert.ToInt32(textBox3.Text);
                var receiveDate = dateTimePicker2.Value;
                var orderDate = dateTimePicker1.Value;
                var countDifference = receiveCount - ChangeableInvoice.ComponentsCount;
                
                ChangeableInvoice.OrderDate = DateTime.Parse(ChangeableInvoice.OrderDate) != orderDate
                    ? orderDate.ToString()
                    : ChangeableInvoice.OrderDate;
                ChangeableInvoice.ReceivingDate = DateTime.Parse(ChangeableInvoice.ReceivingDate) != receiveDate
                    ? receiveDate.ToString()
                    : ChangeableInvoice.ReceivingDate;
                ChangeableInvoice.DeliveryCost = !ChangeableInvoice.DeliveryCost.ToString().Equals(deliveryCost)
                    ? Convert.ToDouble(deliveryCost)
                    : ChangeableInvoice.DeliveryCost;
                ChangeableInvoice.ManufacturingCost =
                    !ChangeableInvoice.ManufacturingCost.ToString().Equals(manufactureCost)
                        ? Convert.ToDouble(manufactureCost)
                        : ChangeableInvoice.ManufacturingCost;
                ChangeableInvoice.ComponentsCount = ChangeableInvoice.ComponentsCount != receiveCount
                    ? receiveCount
                    : ChangeableInvoice.ComponentsCount;
                
                ChangeableComponent.ComponentsName = !ChangeableComponent.ComponentsName.Equals(componentName)
                    ? componentName
                    : ChangeableComponent.ComponentsName;
                ChangeableComponent.ManufactureDate = DateTime.Parse(ChangeableComponent.ManufactureDate) != orderDate
                    ? orderDate.ToString()
                    : ChangeableComponent.ManufactureDate;
                ChangeableComponent.Amount = countDifference != 0
                    ? ChangeableComponent.Amount + countDifference
                    : ChangeableComponent.Amount;

                var updateInvoiceQuery = $"order_date = '{ChangeableInvoice.OrderDate}', " +
                                         $"receiving_date = '{ChangeableInvoice.ReceivingDate}', " +
                                         $"delivery_cost = {ChangeableInvoice.DeliveryCost.ToString().Replace(',', '.')}, " +
                                         $"manufacturing_cost = {ChangeableInvoice.ManufacturingCost.ToString().Replace(',', '.')}, " +
                                         $"components_count = {ChangeableInvoice.ComponentsCount}";
                QueryTools.UpdateTable(updateInvoiceQuery, $"_id = {ChangeableInvoice.Id}",
                    Constants.DatabaseTable.ReceivingInvoicesTable, Connection);
                
                var updateComponentQuery = $"name = '{ChangeableComponent.ComponentsName}', " +
                                           $"manufacture_date = '{ChangeableComponent.ManufactureDate}', " +
                                           $"amount = {ChangeableComponent.Amount}";
                QueryTools.UpdateTable(updateComponentQuery, $"_id = {ChangeableComponent.Id}",
                    Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
            }
            else
            {
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
            }
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox1, Session.FormsAction.ValidationMode.NameValidation,
                "Наименование не должно содержать пробелов");
            ValidateInput();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox3, Session.FormsAction.ValidationMode.IdValidation,
                "количество должно быть целым числом");
            ValidateInput();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox2, Session.FormsAction.ValidationMode.PriceValidation,
                "Стоимость изготовления должна быть целым числом или дестяичной дробью");
            ValidateInput();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox4, Session.FormsAction.ValidationMode.PriceValidation,
                "Стоимость доставки должна быть целым числом или дестяичной дробью");
            ValidateInput();
        }

        private void EditInvoiceForm_Activated(object sender, EventArgs e)
        {
            UpdateFormSates();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var messageBoxResult = MessageBox.Show(
                "Вы уверены, что хотите удалить комплектующее и соответсвующую ему накладную?",
                "Подтверждаение операции",
                    MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
                );
            if (messageBoxResult == DialogResult.Yes)
            {
                var joinedTables = $"{Constants.DatabaseTable.ComponentsWarehouseTable} cw " +
                               $"join {Constants.DatabaseTable.InvoiceAndComponentLinkTable} ic " +
                               "on cw._id = ic.component_id " +
                               $"join {Constants.DatabaseTable.ReceivingInvoicesTable} ri " +
                               "on ic.invoice_id = ri._id";
                var map = QueryTools.SelectFromTableWhere("amount", 
                    $"component_id = {ChangeableComponent.Id}", joinedTables, Connection);
                
                QueryTools.DeleteFromTable($"invoice_id = {ChangeableInvoice.Id}",
                    Constants.DatabaseTable.InvoiceAndComponentLinkTable, Connection);
                
                if (map.Count > 1)
                {
                    var newAmount = ChangeableComponent.Amount - ChangeableInvoice.ComponentsCount;
                    QueryTools.UpdateTable($"amount = {newAmount}", $"_id = {ChangeableComponent.Id}",
                        Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                }
                else
                {
                    QueryTools.DeleteFromTable($"_id = {ChangeableComponent.Id}",
                        Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                }
                
                QueryTools.DeleteFromTable($"_id = {ChangeableInvoice.Id}",
                    Constants.DatabaseTable.ReceivingInvoicesTable, Connection);
                Close();
            }
            else
            {
                Close();
            }
        }
    }
}