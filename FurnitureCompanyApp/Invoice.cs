using System;
using FurnitureCompanyApp;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class Invoice
    {
        public int Id;
    }

    public class ReceiveInvoice : Invoice
    {
        public string OrderDate;
        public string ReceivingDate;
        public double DeliveryCost;
        public double ManufacturingCost;
        public int ComponentsCount;

        public ReceiveInvoice(string orderDate, string receivingDate, double deliveryCost,
            double manufacturingCost, int componentsCount, int id = -1)
        {
            if (id != -1)
                Id = id;
            
            OrderDate = orderDate;
            ReceivingDate = receivingDate;
            DeliveryCost = deliveryCost;
            ManufacturingCost = manufacturingCost;
            ComponentsCount = componentsCount;
        }

        override 
        public string ToString()
        {
            return "Invoice:\n" +
                   $"ID={Id}; " +
                   $"ORDER_DATE={OrderDate}; " +
                   $"RECEIVING_DATE={ReceivingDate}; " +
                   $"DELIVERY_COST={DeliveryCost}; " +
                   $"MANUFACTURING_COST={ManufacturingCost}; " +
                   $"COMPONENTS_COUNT={ComponentsCount}";
        }

        private int GetIdFromDataBase(NpgsqlConnection connection)
        {
            var map = QueryTools.SelectFromTableWhere("_id", 
                $"order_date = {OrderDate} " + 
                $"and receiving_date = {ReceivingDate} " + 
                $"and delivery_cost = {DeliveryCost}" + 
                $"and manufacturing_cost = {ManufacturingCost}" +
                $"and components_count = {ComponentsCount}", 
                "receiving_invoices", connection);
            
            return Convert.ToInt32(map[0]["_id"]);
        }

        public void SetIdFromDataBase(NpgsqlConnection connection)
        { Id = GetIdFromDataBase(connection); }
        
        public static ReceiveInvoice GetInvoiceFromDatabase(int id, NpgsqlConnection connection)
        {
            var fields = string.Join(", ", Constants.DatabaseTable.ReceivingInvoicesTableFields.ToArray());
            var map = QueryTools.SelectFromTableWhere(fields, $"_id = {id}",
                Constants.DatabaseTable.ReceivingInvoicesTable, connection)[0];
            return new ReceiveInvoice(
                map["order_date"].ToString(), 
                map["receiving_date"].ToString(),
                Convert.ToDouble(map["delivery_cost"]), 
                Convert.ToDouble(map["manufacturing_cost"]), 
                Convert.ToInt32(map["components_count"]),
                Convert.ToInt32(map["_id"])
                );
        }
    }
}