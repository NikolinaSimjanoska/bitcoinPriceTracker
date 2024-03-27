using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace bitcoin_app
{
    public interface ISeeder
    {
        void Seed();
    }

    public class BitcoinDbSeeder : ISeeder
    {
        private readonly string _connectionString = "Data Source=bitcoin.db;Version=3;";

        public void Seed()
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    //ali tabela ze obstaja
                    string checkTableExistsSql = "SELECT name FROM sqlite_master WHERE type='table' AND name='bitcoinData';";
                    command.CommandText = checkTableExistsSql;

                    var result = command.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                    {
                        string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS bitcoinData (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Symbol TEXT NOT NULL,
                            Name TEXT NOT NULL,
                            Rank INTEGER NOT NULL,
                            PriceUsd DECIMAL NOT NULL,
                            Date DATE NOT NULL
                        );
                    ";
                        command.CommandText = createTableSql;
                        command.ExecuteNonQuery();

                        SeedRandomData(command);
                    }
                }
            }
        }

        private void SeedRandomData(SQLiteCommand command)
        {
            Random random = new Random();

            DateTime startDate = DateTime.UtcNow.AddHours(-14 * 24);

            for (int i = 0; i < 14 * 24; i++)
            {

                decimal randomPrice = (decimal)random.Next(40000, 70000) + (decimal)random.NextDouble();

                string insertSql = "INSERT INTO bitcoinData (Symbol, Name, Rank, PriceUsd, Date) VALUES ('BTC', 'Bitcoin', 1, @PriceUSD, @Date);";
                command.CommandText = insertSql;
                command.Parameters.AddWithValue("@PriceUSD", randomPrice);
                command.Parameters.AddWithValue("@Date", startDate.AddHours(i).ToString("yyyy-MM-dd HH:mm:ss"));

                command.ExecuteNonQuery();

                command.Parameters.Clear();
            }
        }
    }
}
