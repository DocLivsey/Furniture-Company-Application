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

        public static void UpdateInvoiceTable()
        {
            
        }
        
        public static void InsertIntoReceivingTable(ReceiveInvoice invoice, NpgsqlConnection connection, bool hasId)
        {
            if (hasId)
                Query = "Insert into receiving_invoices " +
                    "(_id, order_date, receiving_date, delivery_cost, manufacturing_cost, components_count) " +
                    "values (@ID, @ORDER_DATE, @RECEIVING_DATE, @DELIVERY_COST, @MANUFACTURING_COST, @COMPONENTS_COUNT)";
            else
                Query = "Insert into receiving_invoices " +
                        "(order_date, receiving_date, delivery_cost, manufacturing_cost, components_count) " +
                        "values (@ORDER_DATE, @RECEIVING_DATE, @DELIVERY_COST, @MANUFACTURING_COST, @COMPONENTS_COUNT) " +
                        "returning _id";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            try
            {
                if (hasId)
                    command.Parameters.AddWithValue("ID", invoice.Id);
                command.Parameters.AddWithValue("ORDER_DATE", invoice.OrderDate);
                command.Parameters.AddWithValue("RECEIVING_DATE", invoice.ReceivingDate);
                command.Parameters.AddWithValue("DELIVERY_COST", invoice.DeliveryCost);
                command.Parameters.AddWithValue("MANUFACTURING_COST", invoice.ManufacturingCost);
                command.Parameters.AddWithValue("COMPONENTS_COUNT", invoice.ComponentsCount);
                if (hasId)
                    command.ExecuteNonQuery();
                else
                {
                    int id = Convert.ToInt32(command.ExecuteScalar());
                    invoice.Id = id;
                }
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
        
        public static void InsertIntoAssemblyTable(AssemblyInvoice invoice, NpgsqlConnection connection, bool hasId)
        {
            if (hasId)
                Query = $"Insert into {Constants.DatabaseTable.AssemblyInvoicesTable} " +
                        "(invoice_id, scheme_id, assembly_price, assembly_date) " +
                        $"values ({invoice.Id}, {invoice.SchemeId}, {invoice.AssemblyPrice}, '{invoice.AssemblyDate}')";
            else
                Query = $"Insert into {Constants.DatabaseTable.AssemblyInvoicesTable} " +
                        "(scheme_id, assembly_price, assembly_date) " +
                        $"values ({invoice.SchemeId}, {invoice.AssemblyPrice.ToString().Replace(',', '.')}, " +
                        $"'{invoice.AssemblyDate}') returning invoice_id";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            try
            {
                if (hasId)
                    command.ExecuteNonQuery();
                else
                {
                    int id = Convert.ToInt32(command.ExecuteScalar());
                    invoice.Id = id;
                }
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