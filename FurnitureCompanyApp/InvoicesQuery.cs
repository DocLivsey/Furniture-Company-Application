using System;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class InvoicesQuery : QueryTools
    {
        public static class InvoicesTransaction
        {
            public static void AddAndUpdateTransaction()
            {
                
            }
        }
        
        
        public static void InsertIntoReceivingTable(ReceiveInvoice invoice, NpgsqlConnection connection)
        {
            Query = "Insert into receiving_invoices " +
                    "(_id, order_date, receiving_date, delivery_cost, manufacturing_cost, components_count)" +
                    "values (@ID, @ORDER_DATE, @RECEIVING_DATE, @DELIVERY_COST, @MANUFACTURING_COST, @COMPONENTS_COUNT)";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            try
            {
                command.Parameters.AddWithValue("ID", invoice.Id);
                command.Parameters.AddWithValue("ORDER_DATE", invoice.OrderDate);
                command.Parameters.AddWithValue("RECEIVING_DATE", invoice.ReceivingDate);
                command.Parameters.AddWithValue("DELIVERY_COST", invoice.DeliveryCost);
                command.Parameters.AddWithValue("MANUFACTURING_COST", invoice.ManufacturingCost);
                command.Parameters.AddWithValue("COMPONENTS_COUNT", invoice.ComponentsCount);
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