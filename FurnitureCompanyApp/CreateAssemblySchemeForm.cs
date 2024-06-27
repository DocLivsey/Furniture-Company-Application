using System;
using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class CreateAssemblySchemeForm : Form
    {
        private NpgsqlConnection Connection { get; set; }
        private bool ValidInput { get; set; }
        
        public CreateAssemblySchemeForm(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }

        private void CreateAssemblyScheme_Load(object sender, EventArgs e)
        {
            UpdateFormSates();
            StartPosition = FormStartPosition.CenterParent;
        }

        private void CreateAssemblyScheme_Activated(object sender, EventArgs e)
        {
            UpdateFormSates();
        }
        
        private void UpdateFormSates()
        {
            ValidateInput();
            UpdateComboBox();
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
            
            map = QueryTools.SimpleSelectFromTable(
                "scheme_id", Constants.DatabaseTable.AssemblySchemasTable, Connection);
            foreach (var row in map)
                foreach (var value in row.Values)
                    if (!comboBox2.Items.Contains(value))
                        comboBox2.Items.Add(value);
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

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            ValidInput = Session.FormsAction.ValidateBoxInput(comboBox2, Session.FormsAction.ValidationMode.IdValidation,
                "Номер схемы должен быть целым числом");
            ValidateInput();
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
            if (comboBox1.Text.Trim().Length != 0 &&
                textBox1.Text.Trim().Length != 0 &&
                comboBox2.Text.Trim().Length != 0 &&
                ValidInput)
            {
                int componentsId = int.Parse(comboBox1.Text);
                int schemeId = int.Parse(comboBox2.Text);
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
                    Scheme scheme = new Scheme(schemeId, componentsId, requiredAmount);
                    scheme.InsertIntoDatabase(Connection, this);
                }
            }
            else
            {
                if (comboBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(comboBox1, "Введите пожалуйста код комплектующего");
                }
                if (comboBox2.Text.Trim().Length == 0)
                {
                    SetToolTip(comboBox2, "Введите пожалуйста номер схемы сборки");
                }
                if (textBox1.Text.Trim().Length == 0)
                {
                    SetToolTip(textBox1, "Введите пожалуйста необходимое количество заданного комплектующего");
                }
            }
            UpdateFormSates();
            Close();
        }
    }
}