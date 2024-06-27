using System;
using FurnitureCompanyApp;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class Invoice
    {
        public int Id;

        public Invoice(int id = -1)
        {
            if (id != -1)
                Id = id;
        }
    }

    public class ReceiveInvoice : Invoice
    {
        public string OrderDate;
        public string ReceivingDate;
        public double DeliveryCost;
        public double ManufacturingCost;
        public int ComponentsCount;

        public ReceiveInvoice(string orderDate, string receivingDate, double deliveryCost,
            double manufacturingCost, int componentsCount, int id = -1) : base(id)
        {
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
                $"order_date = '{OrderDate}' " + 
                $"and receiving_date = '{ReceivingDate}' " + 
                $"and delivery_cost = {DeliveryCost.ToString().Replace(",", ".")} " + 
                $"and manufacturing_cost = {ManufacturingCost.ToString().Replace(",", ".")} " +
                $"and components_count = {ComponentsCount}", 
                Constants.DatabaseTable.ReceivingInvoicesTable, connection)[0];
            
            return int.Parse(map["_id"].ToString());
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

    public class AssemblyInvoice : Invoice
    {
        public int SchemeId { get; set; }
        public double AssemblyPrice { get; set; }
        public string AssemblyDate { get; set; }

        public AssemblyInvoice(int schemeId, double assemblyPrice, string assemblyDate, int id = -1) : base(id)
        {
            SchemeId = schemeId;
            AssemblyPrice = assemblyPrice;
            AssemblyDate = assemblyDate;
        }
        
        public static AssemblyInvoice GetInvoiceFromDatabase(int id, NpgsqlConnection connection)
        {
            var fields = string.Join(", ", Constants.DatabaseTable.ReceivingInvoicesTableFields.ToArray());
            var map = QueryTools.SelectFromTableWhere(fields, $"invoice_id = {id}",
                Constants.DatabaseTable.AssemblyInvoicesTable, connection)[0];
            return new AssemblyInvoice(
                Convert.ToInt32(map["scheme_id"]),
                Convert.ToDouble(map["assembly_price"]),
                (map["assembly_date"]).ToString(),
                Convert.ToInt32(map["invoice_id"]));
        }
    }
}