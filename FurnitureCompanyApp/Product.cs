using System;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class Product
    {
        public int Id { get; set; }

        protected Product(int id)
        { Id = id; }
    }

    public class FurnitureComponent : Product
    {
        public string ComponentsName { get; set; }
        public string ManufactureDate { get; set; }
        public int Amount { get; set; }

        public FurnitureComponent(int id, string componentsName, string manufactureDate, int amount) : base(id)
        {
            ComponentsName = componentsName;
            ManufactureDate = manufactureDate;
            Amount = amount;
        }

        public static FurnitureComponent GetComponentFromDatabase(int id, NpgsqlConnection connection)
        {
            var fields = string.Join(", ", Constants.DatabaseTable.ComponentsWarehouseTableFields.ToArray());
            var map = QueryTools.SelectFromTableWhere(fields, $"_id = {id}",
                Constants.DatabaseTable.ComponentsWarehouseTable, connection)[0];
            return new FurnitureComponent(
                Convert.ToInt32(map["_id"]),
                map["name"].ToString(),
                map["manufacture_date"].ToString(),
                Convert.ToInt32(map["amount"])
                );
        }
    }

    public class Furniture : Product
    {
        public int SchemeId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public double ResultPrice { get; set; }

        public Furniture(int id, int schemeId, string name, int amount, double resultPrice) : base(id)
        {
            SchemeId = schemeId;
            Name = name;
            Amount = amount;
            ResultPrice = resultPrice;
        }
        
        public static Furniture GetComponentFromDatabase(int id, NpgsqlConnection connection)
        {
            var fields = string.Join(", ", Constants.DatabaseTable.ComponentsWarehouseTableFields.ToArray());
            var map = QueryTools.SelectFromTableWhere(fields, $"furniture_id = {id}",
                Constants.DatabaseTable.FurnitureWarehouseTable, connection)[0];
            return new Furniture(
                Convert.ToInt32(map["furniture_id"]),
                Convert.ToInt32(map["scheme_id"]),
                map["name"].ToString(),
                Convert.ToInt32(map["amount"]),
                Convert.ToDouble(map["result_price"])
            );
        }
    }
}