using System;
using System.Windows.Forms;
using Npgsql;

namespace FurnitureCompanyApp
{
    public class Scheme
    {
        public int SchemeId { get; set; }
        public int ComponentId { get; set; }
        public int RequiredAmount { get; set; }

        public Scheme(int schemeId, int componentId, int requiredAmount)
        {
            SchemeId = schemeId;
            ComponentId = componentId;
            RequiredAmount = requiredAmount;
        }

        public override string ToString()
        {
            return $"SCHEME_ID = {SchemeId}, COMPONENT_ID = {ComponentId}, REQUIRED_AMOUNT = {RequiredAmount}";
        }
        
        public void InsertIntoDatabase(NpgsqlConnection connection, Form form)
        {
            var map = QueryTools.SelectFromTableWhere("scheme_id",
                $"scheme_id = {SchemeId} and component_id = {ComponentId}",
                Constants.DatabaseTable.RequiredComponentsTable, connection);
            if (map.Count > 0)
            {
                MessageBox.Show("Такая накладная уже есть");
                form.Close();
            }
            else
            {
                map = QueryTools.SelectFromTableWhere("scheme_id",
                    $"scheme_id = {SchemeId}", Constants.DatabaseTable.AssemblySchemasTable, connection);
                if (map.Count == 0)
                {
                    var sql = $"insert into {Constants.DatabaseTable.AssemblySchemasTable} " +
                              $"(scheme_id) values ({SchemeId})";
                    NpgsqlCommand command = new NpgsqlCommand(sql, connection);
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (NpgsqlException exception)
                    {
                        MessageBox.Show(
                            exception.Message + "\nErrorCode: " + exception.ErrorCode, 
                            "Ошибка добавления в базу данных",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            
                var auxiliarySql = $"insert into {Constants.DatabaseTable.RequiredComponentsTable} " +
                                   "(scheme_id, component_id, required_amount) values " +
                                   $"({SchemeId}, {ComponentId}, {RequiredAmount})";
                NpgsqlCommand cmd = new NpgsqlCommand(auxiliarySql, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (NpgsqlException exception)
                {
                    MessageBox.Show(
                        exception.Message + "\nErrorCode: " + exception.ErrorCode, 
                        "Ошибка добавления в базу данных",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
    
}