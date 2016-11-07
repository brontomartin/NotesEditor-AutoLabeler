using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PokerstarsAutoNotes.Infrastructure.Database
{
    public class AvailableDatabaseResolver
    {
        public IEnumerable<string> Resolve(Model.Database database)
        {
            var databasesString = new List<string>();
            try
            {
                var conString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                                              database.Server, database.Port, database.Username, database.Password,
                                              "postgres");
                using (var connection = new NpgsqlConnection(conString))
                using (var command = new NpgsqlCommand("select datname from pg_database where datistemplate = 'f' AND datname <> 'postgres' order by datname", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            databasesString.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (NpgsqlException)
            {
                return null;
            }
            return databasesString;
                //.Select(databaseString => new DatabaseType
                //{
                //    Name = databaseString,
                //    DatabaseTypeEnum = IoC.Resolve<DatabaseResolver>().Resolve(database)
                //}).ToList();
        }
    }
}
