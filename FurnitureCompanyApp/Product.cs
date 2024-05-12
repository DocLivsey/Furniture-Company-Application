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
}