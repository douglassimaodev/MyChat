using MyChat.Models;
using System.Globalization;

namespace MyChat.Services
{
    public class StockService : IStockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StockService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Tuple<bool, string>> GetStock(string stockCode)
        {
            var url = $"https://stooq.com/q/l/?s={stockCode}&f=sd2t2ohlcv&h&e=csv";
            var client = _httpClientFactory.CreateClient("MyClient");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var lines = content.Split('\n');

                if (lines.Length > 1)
                {
                    var values = lines[1].Split(',');

                    if (values.Length == 8 && decimal.TryParse(values[6], out decimal price))
                    {
                        var stockQuote = new StockQuote
                        {
                            Symbol = stockCode.ToUpper(),
                            Price = price
                        };

                        var culture = CultureInfo.CreateSpecificCulture("en-US");
                        return new Tuple<bool, string>(true, $"{stockQuote.Symbol} quote is {stockQuote.Price.ToString("C", culture)} per share");
                    }
                    else
                    {
                        return new Tuple<bool, string>(false, $"{stockCode} is not a valid parameter");
                    }
                }
            }

            return new Tuple<bool, string>(false, $"{stockCode} is not a valid command");
        }
    }
}
