using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class AssemblyInvoiceForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private bool ValidInput { get; set; }
        private DataTable Table = new DataTable(); 
        private DataSet Set = new DataSet();
        
        public AssemblyInvoiceForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }

        private void AssemblyInvoiceForm_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterParent;
            UpdateFormSates();
        }

        private void AssemblyInvoiceForm_Activated(object sender, EventArgs e)
        {
            UpdateFormSates();
        }
        
        private void UpdateFormSates()
        {
            ResetDataGridViewSource();
            UpdateComboBox1();
            UpdateComboBox2();
            ValidateInput();
        }
        
        private void ResetDataGridViewSource()
        {
            NpgsqlDataAdapter dataAdapter = UploadFromDataBase();
            Set.Reset(); dataAdapter.Fill(Set);
            Table = Set.Tables[0];
            dataGridView1.DataSource = Table;
            dataGridView1.Columns[0].HeaderText = "Код накладной";
            dataGridView1.Columns[1].HeaderText = "Код схемы";
            dataGridView1.Columns[2].HeaderText = "Стоимость сборки";
            dataGridView1.Columns[3].HeaderText = "Дата сборки";
        }
        
        private NpgsqlDataAdapter UploadFromDataBase()
        {
            try
            {
                string sqlQuery = $"select * from {Constants.DatabaseTable.AssemblyInvoicesTable}";
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
        
        private void UpdateComboBox1()
        {
            var map = QueryTools.SimpleSelectFromTable(
                "scheme_id", Constants.DatabaseTable.AssemblySchemasTable, Connection);
            foreach (var row in map)
                foreach (var value in row.Values)
                    if (!comboBox1.Items.Contains(value))
                        comboBox1.Items.Add(value);
        }
        
        private void UpdateComboBox2()
        {
            var map = QueryTools.SimpleSelectFromTable(
                "furniture_id", Constants.DatabaseTable.FurnitureWarehouseTable, Connection);
            foreach (var row in map)
                foreach (var value in row.Values)
                    if (!comboBox2.Items.Contains(value))
                        comboBox2.Items.Add(value);
        }
        
        private void SetToolTip(Control box, string text)
        {
            Session.FormsAction.SetErrorToolTip(box, text);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim().Length != 0 &&
                comboBox2.Text.Trim().Length != 0 &&
                textBox1.Text.Trim().Length != 0 &&
                textBox2.Text.Trim().Length != 0 &&
                textBox3.Text.Trim().Length != 0 &&
                ValidInput)
            {
                int schemeId = int.Parse(comboBox1.Text);
                double assemblyPrice = double.Parse(textBox1.Text);
                var assemblyDate = dateTimePicker1.Value.Date;
                var furnitureId = int.Parse(comboBox2.Text);
                var furnitureName = textBox2.Text;
                var furnitureAmount = int.Parse(textBox3.Text);
                
                var map = QueryTools.SelectFromTableWhere("scheme_id", 
                    $"scheme_id = {schemeId}", Constants.DatabaseTable.AssemblySchemasTable, Connection);
                if (map.Count == 0)
                {
                    var result = MessageBox.Show(
                        "Нет схем сборок с заданным номером\n" +
                        "Пожалуйста выберите схему из представленного списка или создайте новую\n" +
                        "'ОК' - создать новую, 'ОТМЕНА' - вернуться назад",
                        "Внимание",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                    );
                    if (result == DialogResult.OK)
                    {
                        CreateAssemblySchemeForm form = new CreateAssemblySchemeForm(Connection);
                        form.Show();
                    }
                }
                else
                {
                    var joinedTables = $"{Constants.DatabaseTable.RequiredComponentsTable} rc " +
                                $"join {Constants.DatabaseTable.ComponentsWarehouseTable} " +
                                "cw on rc.component_id = cw._id";
                    map = QueryTools.SelectFromTableWhere("amount, required_amount, component_id", 
                        $"scheme_id = {schemeId}", joinedTables, Connection);
                    bool isCorrect = true;
                    double totalResultPrice = 0;
                    foreach (var match in map)
                    {
                        var amount = Convert.ToInt32(match["amount"]);
                        var requiredAmount = Convert.ToInt32(match["required_amount"]);
                        if (amount < requiredAmount)
                        {
                            MessageBox.Show(
                                "Невозможно заказть сборку из-за недостатка " +
                                $"комплектующего (id: {match["component_id"]}) на складе",
                                "Ошибка",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                                );
                            isCorrect = false;
                        }
                        else
                        {
                            var componentId = Convert.ToInt32(match["component_id"]);
                            joinedTables = $"{Constants.DatabaseTable.ComponentsWarehouseTable} cw " +
                                           $"join {Constants.DatabaseTable.InvoiceAndComponentLinkTable} ic " +
                                           "on cw._id = ic._component_id " +
                                           $"join {Constants.DatabaseTable.ReceivingInvoicesTable} ri " +
                                           "on ic.invoice_id = ri._id";
                            var map1 = QueryTools.SelectFromTableWhere("manufacturing_cost, delivery_cost", 
                                $"_component_id = {componentId}", joinedTables, Connection);
                            var totalDeliveryCost = 0.0; 
                            var totalManufacturingCost = 0.0;
                            foreach (var pair in map1)
                            {
                                totalDeliveryCost += Convert.ToDouble(pair["delivery_cost"]);
                                totalManufacturingCost += Convert.ToDouble(pair["manufacturing_cost"]);
                            }
                            amount -= requiredAmount;
                            QueryTools.UpdateTable($"amount = {amount}", $"_id = {componentId}",
                                Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                            var resultPrice = totalManufacturingCost + totalDeliveryCost + assemblyPrice;
                            totalResultPrice += resultPrice;
                        }
                    }
                    if (isCorrect)
                    {
                        map = QueryTools.SelectFromTableWhere("amount, result_price", $"furniture_id = {furnitureId}",
                            Constants.DatabaseTable.FurnitureWarehouseTable, Connection);
                        if (map.Count > 0)
                        {
                            int oldAmount = Convert.ToInt32(map[0]["amount"]);
                            var oldPrice = double.Parse(map[0]["result_price"].ToString());
                            QueryTools.UpdateTable($"amount = {oldAmount + furnitureAmount}," + 
                                                   $" result_price = {(oldPrice + totalResultPrice).ToString().Replace(',', '.')}",
                                $"furniture_id = {furnitureId}", 
                                Constants.DatabaseTable.FurnitureWarehouseTable, Connection);
                        }
                        else
                        {
                            AssemblyInvoice invoice = new AssemblyInvoice(
                                schemeId, assemblyPrice, assemblyDate.ToString());
                            InvoicesQuery.InsertIntoAssemblyTable(invoice, Connection, false);
                                
                            Furniture furniture = new Furniture(
                                furnitureId, schemeId, furnitureName,
                                furnitureAmount, Convert.ToDouble(totalResultPrice));
                            ProductsQuery.InsertIntoFurnitureTable(furniture, Connection);
                        }
                    }
                }
            }
            else
            {
                if (comboBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(comboBox1, "Введите пожалуйста номер схемы сборки");
                }
                if (textBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox1, "Введите пожалуйста стоимость сборки");
                }
                if (textBox3.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox3, "Введите пожалуйста количество");
                }
                if (textBox2.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox2, "Введите пожалуйста наименование");
                }
            }
            UpdateFormSates();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                "Код должен быть целым числом");
            ValidateInput();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox1, Session.FormsAction.ValidationMode.PriceValidation,
                "Стоимость должна быть целым числом или дестяичной дробью");
            ValidateInput();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Random random = new Random();
                textBox1.Text = $"{random.Next(100, 10000) * random.NextDouble()}";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(textBox2, Session.FormsAction.ValidationMode.NameValidation,
                "Наименование не должно содержать пробелов");
            ValidateInput();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox3, Session.FormsAction.ValidationMode.IdValidation,
                "количество должно быть целым числом");
            ValidateInput();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var map = QueryTools.SelectFromTableWhere(
                "furniture_name", $"furniture_id = {comboBox2.SelectedItem}",
                Constants.DatabaseTable.FurnitureWarehouseTable, Connection)[0];
            textBox2.Text = map["furniture_name"].ToString();
            textBox2.Enabled = false;
        }

        private void comboBox2_TextUpdate(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                "Код должен быть целым числом");
            ValidateInput();
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            if (Session.FormsAction.ValidateBoxInput(
                    comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                    "Код должен быть целым числом"))
                if (!comboBox1.Items.Contains(Convert.ToInt32(comboBox1.Text)))
                {
                    textBox1.Text = "";
                    textBox1.Enabled = true;
                }
        }
    }
}