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
            QueryTools.UpdateTable("receiving_date = 'monday'", "_id = 1",
                "receiving_invoices", Connection);
            var map = QueryTools.SelectFromTableWhere("_id", "_id = 1",
                "receiving_invoices", Connection);
            foreach (var dict in map)
                foreach (var pair in dict)
                    Console.WriteLine(pair);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ErrorProvider errorProvider = new ErrorProvider();
            errorProvider.SetError(button2, "ERROR");
            errorProvider.SetIconAlignment(button2, ErrorIconAlignment.BottomLeft);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            
        }
    }
}