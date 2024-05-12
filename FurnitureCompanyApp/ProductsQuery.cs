using System;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class ProductsQuery : QueryTools
    {
        public static void InsertIntoComponentsTable(FurnitureComponent product, NpgsqlConnection connection)
        {
            Query = "Insert into components_warehouse " +
                    "(_id, name, manufacture_date, amount) values " +
                    "(@ID, @NAME, @MANUFACTURE_DATE, @AMOUNT)";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            try
            {
                command.Parameters.AddWithValue("ID", product.Id);
                command.Parameters.AddWithValue("NAME", product.ComponentsName);
                command.Parameters.AddWithValue("MANUFACTURE_DATE", product.ManufactureDate);
                command.Parameters.AddWithValue("AMOUNT", product.Amount);
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException e)
            {
                MessageBox.Show(
                    e.Message + "\nErrorCode: " + e.ErrorCode, 
                    "Ошибка добавления в базу данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.Message);
            }
        }
    }
}