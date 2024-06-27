using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using Npgsql;

namespace FurnitureCompanyApp
{
    public partial class Test : Form
    {
        private NpgsqlConnection Connection { get; set; }
        
        public Test(NpgsqlConnection connection)
        {
            InitializeComponent();
            Connection = connection;
        }

        private void Test_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{dateTimePicker1.Value}, {dateTimePicker1.Text}");

            string today = dateTimePicker1.Text;
            DateTime todayDate = DateTime.Parse(today);

            MessageBox.Show(todayDate.ToString());
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Console.WriteLine(dateTimePicker1.Value);
        }

        private void maskedTextBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}