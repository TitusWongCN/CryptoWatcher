using Flurl.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CryptoWatcher.Common
{
    static class WebAPIs
    {
        public static async Task<string> GetCurrentPrice(string fsym, string tsyms = "usdt")
        {
            if (fsym.ToUpper() != "GRID")
            {
                var resp = await $"https://data.gateapi.io/api2/1/ticker/{fsym}_{tsyms}".GetStringAsync();
                JObject _jObject = JObject.Parse(resp);
                return _jObject["last"].ToString() + "," + _jObject["percentChange"].ToString();
            }
            else
            {
                var resp = await "http://119.3.78.178:9001/json".GetStringAsync();
                JObject _jObject = JObject.Parse(resp);
                return _jObject["last"].ToString() + "," + _jObject["percentChange"].ToString();
            }
        }
    }
}
