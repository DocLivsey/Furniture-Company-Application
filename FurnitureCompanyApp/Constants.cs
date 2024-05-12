using System;
using System.Collections.Generic;

namespace FurnitureCompanyApp
{
    public class Constants
    {
        public static class Connection
        {
            public static readonly string LocalServer = "localhost";
            public static readonly string Port = "5432";
            public static readonly string Userid = "postgres";
            public static readonly string Password = "190122";
            public static readonly string DatabaseName = "furniture company";
        }
        
        public static class DatabaseTable
        {
            public static readonly string ReceivingInvoicesTable = "receiving_invoices";
            public static readonly string ComponentsWarehouseTable = "components_warehouse";
            public static readonly string InvoiceAndComponentLinkTable = "invoice_for_components";

            public static readonly List<string> ReceivingInvoicesTableFields = new List<string>()
            {
                "_id", "order_date", "receiving_date", "delivery_cost", "manufacturing_cost", "components_count"
            };

            public static readonly List<string> ComponentsWarehouseTableFields = new List<string>()
            {
                "_id", "name", "manufacture_date", "amount"
            };

            public static readonly List<string> InvoiceAndComponentLinkTableFields = new List<string>()
            {
                "invoice_id", "component_id"
            };
        }
    }
}