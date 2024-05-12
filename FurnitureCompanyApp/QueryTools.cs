using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class QueryTools
    {
        protected static string Query { get; set; }
        
        private static Dictionary<string, object> FieldsTable { get; set; }
        private static List<string> ValuesNames { get; set; }

        private static readonly BindingFlags BindingFlags = BindingFlags.Public | 
                                                              BindingFlags.NonPublic | 
                                                              BindingFlags.Instance | 
                                                              BindingFlags.Static;
        
        public static List<Dictionary<string, object>> SimpleSelectFromTable(string fields,
            string tableName, NpgsqlConnection connection)
        {
            List<Dictionary<string, object>> fieldsTableList = new List<Dictionary<string, object>>();
            List<string> fieldsNames = Regex.Split(fields, @"[,\s]+").ToList();
            Query = $"Select {fields} from {tableName}";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Dictionary<string, object> fieldsTable = new Dictionary<string, object>();
                    foreach (var field in fieldsNames)
                        fieldsTable.Add(field, reader[field]);
                    fieldsTableList.Add(fieldsTable);
                }
            }
            return fieldsTableList;
        }

        public static List<Dictionary<string, object>> SelectFromTableWhere(string fields, string condition,
            string tableName, NpgsqlConnection connection)
        {
            List<Dictionary<string, object>> fieldsTableList = new List<Dictionary<string, object>>();
            List<string> fieldsNames = Regex.Split(fields, @"[,\s]+").ToList();
            Query = $"Select {fields} from {tableName} where {condition}";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Dictionary<string, object> fieldsTable = new Dictionary<string, object>();
                    foreach (var field in fieldsNames)
                        fieldsTable.Add(field, reader[field]);
                    fieldsTableList.Add(fieldsTable);
                }
            }
            return fieldsTableList;
        }

        public static void UpdateTable(string setValues, string condition, string tableName, NpgsqlConnection connection)
        {
            Query = $"Update {tableName} set {setValues} where {condition}";
            NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            command.ExecuteNonQuery();
        }
        
        public static void InsertInto(Invoice invoice, NpgsqlConnection connection, string tableName)
        {
            FieldsTable = new Dictionary<string, object>();
            foreach (var field in invoice.GetType().GetProperties(BindingFlags).ToList())
            {
                switch (field.Name)
                {
                    case "Id":
                        FieldsTable.Add("_id", field.GetValue(invoice));
                        break;
                    case "ReceivingDate":
                        FieldsTable.Add("receiving_date", field.GetValue(invoice));
                        break;
                    case "DeliveryCost":
                        FieldsTable.Add("delivery_cost", field.GetValue(invoice));
                        break;
                    case "ManufacturingCost":
                        FieldsTable.Add("manufacturing_cost", field.GetValue(invoice));
                        break;
                    case "ComponentsCount":
                        FieldsTable.Add("components_count", field.GetValue(invoice));
                        break;
                }
            }

            string values = string.Join(", ", FieldsTable.Keys.ToArray());
            
            Query = $"Insert into {tableName} ({values}) VALUES ()";
            //NpgsqlCommand command = new NpgsqlCommand(Query, connection);
            Console.WriteLine(Query);
        }

        public static void UpdateSelect(string query)
        {
            Query = query;
            
        }
    }
}