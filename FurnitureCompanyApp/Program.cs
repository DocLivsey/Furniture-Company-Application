using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LaunchApp();
        }

        private static void Test()
        {
            NpgsqlConnection connection = new NpgsqlConnection($"Server={Constants.Connection.LocalServer}; " +
                                                               $"Port={Constants.Connection.Port}; " + 
                                                               $"UserID={Constants.Connection.Userid}; " +
                                                               $"Password={Constants.Connection.Password}; " +
                                                               $"Database={Constants.Connection.DatabaseName}");
            connection.Open();
            QueryTools.DeleteFromTable("_id = 43", Constants.DatabaseTable.ReceivingInvoicesTable, connection);
        }

        private static void LaunchApp()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}