using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bitcoin_app
{
    [ApiController]
    [Route("/")]
    public class BitcoinController : ControllerBase
    {
        private readonly string _connectionString = "Data Source=bitcoin.db;Version=3;";

        [HttpGet("CurrentPrice")]
        public async Task<IActionResult> GetCurrentPrice()
        {

            decimal currentPrice = await GetCurrentBitcoinPrice();

            return Ok(currentPrice);
        }

        [HttpGet("AveragePriceForDate")]
        public IActionResult GetAveragePriceForDate([FromQuery] DateTime date)
        {
            var prices = GetBitcoinPricesForDate(date);

            decimal averagePrice = CalculateAveragePrice(prices);

            return Ok(averagePrice);
        }
        private async Task<decimal> GetCurrentBitcoinPrice()
        {
            using (HttpClient client = new HttpClient())
            {

                var response = await client.GetStringAsync("https://api.coinlore.net/api/ticker/?id=90");

                var bitcoinData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BitcoinResponse>>(response);

                var price = decimal.Parse(bitcoinData[0].price_usd);

                InsertBitcoinData(bitcoinData[0]);

                return price;
            }
        }

        private void InsertBitcoinData(BitcoinResponse bitcoinData)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                string createTableSql = @"
                CREATE TABLE IF NOT EXISTS bitcoinData (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Symbol TEXT NOT NULL,
                    Name TEXT NOT NULL,
                    Rank INTEGER NOT NULL,
                    PriceUsd DECIMAL NOT NULL,
                    Date TEXT NOT NULL
                );
            ";

                using (SQLiteCommand command = new SQLiteCommand(createTableSql, connection))
                {
                    command.ExecuteNonQuery();

                    string insertSql = "INSERT INTO bitcoinData (Symbol, Name, Rank, PriceUsd,Date) VALUES (@Symbol, @Name, @Rank, @PriceUSD,@Date);";

                    command.CommandText = insertSql;
                    command.Parameters.AddWithValue("@Symbol", bitcoinData.symbol);
                    command.Parameters.AddWithValue("@Name", bitcoinData.name);
                    command.Parameters.AddWithValue("@Rank", bitcoinData.rank);
                    command.Parameters.AddWithValue("@PriceUSD", decimal.Parse(bitcoinData.price_usd));
                    command.Parameters.AddWithValue("@Date", DateTime.UtcNow);

                    command.ExecuteNonQuery();
                }
            }
        }

        private decimal CalculateAveragePrice(decimal[] prices)
        {
            if (prices.Length == 0)
                return 0;

            decimal sum = 0;

            foreach (var price in prices)
            {
                sum += price;
            }

            return sum / prices.Length;
        }

        private decimal[] GetBitcoinPricesForDate(DateTime date)
        {
     
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {

                    string selectSql = "SELECT PriceUsd FROM bitcoinData WHERE DATE(Date) = DATE(@Date);";

                    command.CommandText = selectSql;
                    command.Parameters.AddWithValue("@Date", date);

                    var reader = command.ExecuteReader();

                    var prices = new List<decimal>();

                    while (reader.Read())
                    {
                        prices.Add(reader.GetDecimal(0));
                    }
                    return prices.ToArray();
                }
            }
        }
    }
}
