using System;
using Npgsql;
using PokerstarsAutoNotes.Infrastructure.Data;

namespace PokerstarsAutoNotes.Infrastructure.Database
{
    public class DatabaseResolver
    {
        public IPokerDatabase Resolve(Model.Database database)
        {
            if (database == null)
                return null;
            //check for pt3
            try
            {
                var connectionString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
                    database.Server, database.Port, database.Username, database.Password, database.Name);
                using (var connection = new NpgsqlConnection(connectionString))
                using (var command = new NpgsqlCommand("select * from pg_tables where schemaname = 'public' and tablename = 'holdem_limit'", connection))
                {
                    command.CommandTimeout = 360;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new PokerTrackerDatabase(database);
                        }
                    }
                }
                using (var connection = new NpgsqlConnection(connectionString))
                using (var command = new NpgsqlCommand("select * from pg_tables where schemaname = 'public' and tablename = 'cash_limit'", connection))
                {
                    command.CommandTimeout = 360;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Pt4Database(database);
                        }
                    }
                }
                using (var connection = new NpgsqlConnection(connectionString))
                using (var command = new NpgsqlCommand("select * from pg_tables where schemaname = 'public'and tablename = 'currency'", connection))
                {
                    command.CommandTimeout = 360;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new HoldemManagerDatabase(database);
                        }
                    }
                }
                using (var connection = new NpgsqlConnection(connectionString))
                using (var command = new NpgsqlCommand("select * from pg_tables where schemaname = 'public'and tablename = 'tooltips'", connection))
                {
                    command.CommandTimeout = 360;
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return new Hm2Database(database);
                        }
                    }
                }
            }
            catch (NpgsqlException)
            {
                return null;
            }

            return null;
        }
    }
}
