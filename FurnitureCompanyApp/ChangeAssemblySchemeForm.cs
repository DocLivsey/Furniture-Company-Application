using System;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class ChangeAssemblySchemeForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private bool ValidInput { get; set; }
        private Scheme ChangeableScheme { get; set; }
        
        public ChangeAssemblySchemeForm(NpgsqlConnection connection, Scheme changeableScheme)
        {
            InitializeComponent();
            Connection = connection;
            ChangeableScheme = changeableScheme;
        }

        private void ChangeAssemblySchemeForm_Load(object sender, EventArgs e)
        {
            UpdateFormSates();
            StartPosition = FormStartPosition.CenterParent;
        }

        private void ChangeAssemblySchemeForm_Activated(object sender, EventArgs e)
        {
            UpdateFormSates();
        }
        
        private void UpdateFormSates()
        {
            UpdateComboBox();
            UpdateTextFields();
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
        
        private void UpdateTextFields()
        {
            textBox1.Text = ChangeableScheme.RequiredAmount.ToString();
            comboBox1.Text = ChangeableScheme.ComponentId.ToString();
        }
        
        private void ValidateInput()
        {
            button1.Enabled = ValidInput;
        }
        
        private void SetToolTip(Control box, string text)
        {
            Session.FormsAction.SetErrorToolTip(box, text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(comboBox1, Session.FormsAction.ValidationMode.IdValidation,
                "Код комплектующего должен быть целым числом");
            ValidateInput();
        }

        private void comboBox1_TextUpdate(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(
                textBox1, Session.FormsAction.ValidationMode.IdValidation,
                "количество должно быть целым числом");
            ValidateInput();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim().Length != 0 &&
                comboBox1.Text.Trim().Length != 0 &&
                ValidInput)
            {
                int componentsId = int.Parse(comboBox1.Text);
                int requiredAmount = int.Parse(textBox1.Text);
                
                var map = QueryTools.SelectFromTableWhere("_id", 
                    $"_id = {componentsId}", Constants.DatabaseTable.ComponentsWarehouseTable, Connection);
                if (map.Count == 0)
                {
                    var result = MessageBox.Show(
                        "На складе нет комплектующих с заданным кодом\n" +
                        "Пожалуйста выберите комплектующее из представленного списка или закажите новое\n" +
                        "'ОК' - заказать новое, 'ОТМЕНА' - вернуться назад",
                        "Внимание",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                    );
                    if (result == DialogResult.OK)
                    {
                        OrderComponentsForm form = new OrderComponentsForm(Connection);
                        form.Show();
                    }
                }
                else
                {
                    ChangeableScheme.ComponentId = ChangeableScheme.ComponentId != componentsId
                        ? componentsId
                        : ChangeableScheme.ComponentId;
                    ChangeableScheme.RequiredAmount = ChangeableScheme.RequiredAmount != requiredAmount
                        ? requiredAmount
                        : ChangeableScheme.RequiredAmount;

                    var updateSchemeQuery = $"component_id = {ChangeableScheme.ComponentId}, " +
                                            $"required_amount = {ChangeableScheme.RequiredAmount}";
                    QueryTools.UpdateTable(updateSchemeQuery, $"scheme_id = {ChangeableScheme.SchemeId}",
                        Constants.DatabaseTable.AssemblySchemasTable, Connection);
                }
            }
            else
            {
                if (textBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox1, "Введите пожалуйста необходимое количество заданного комплектующего");
                }
                if (comboBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(comboBox1, "Введите пожалуйста производственный код комплектующего");
                }
            }
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}