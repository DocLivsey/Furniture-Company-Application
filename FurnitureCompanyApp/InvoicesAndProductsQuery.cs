using System;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class InvoicesAndProductsQuery : QueryTools
    {
        public static void InsertIntoLinkTable(ReceiveInvoice invoice,
            FurnitureComponent product, NpgsqlConnection connection)
        {
            Query = "Insert into invoices_for_components " +
                    "(invoice_id, component_id) " +
                    "values (@INVOICE_ID, @COMPONENT_ID)";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            try
            {
                command.Parameters.AddWithValue("INVOICE_ID", invoice.Id);
                command.Parameters.AddWithValue("COMPONENT_ID", product.Id);
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