using CurrencyConverter.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CurrencyConverter.Model.CBR
{
    class CBRFinanceSource : FinanceSource
    {
        private static FinanceSource instance = null;
        protected CBRFinanceSource() : base("CBR", "https://www.cbr-xml-daily.ru/daily_json.js") { }

        public override IFinanceExchange DoRequestWintHandle()
        {
            var response = BurseRequest.getStockQuotes(Url);
            if (response.ResponseCode == HttpStatusCode.OK)
                return CBRXmlDailyResponse.LoadFromText(response.ResponseString);
            
            return null;
        }

        public static FinanceSource GetInstance()
        {
            if (instance == null)
            {
                instance = new CBRFinanceSource();
            }
            return instance;
        }
    }
    class CBRXmlDailyResponse : IFinanceExchange
    {
        public string Date { get; set; }
        public string Timestamp { get; set; }
        public string PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public CBRXmlDailyResponse()
        {
            Valute = new SortedDictionary<string, Currency>();
            Currency rub = new Currency();
            rub.Value = 1;
            rub.CharCode = "RUB";
            rub.Nominal = 1;
            rub.Name = "Российский рубль";
            Valute.Add("RUB", rub);
        }
        public SortedDictionary<string, Currency> Valute { get; set; }

        public IEnumerable<string> GetCurrenciesNames() => Valute.Keys;
        public IEnumerable<Currency> GetCurrencies() => Valute.Values;

        public static CBRXmlDailyResponse LoadFromText(string response) => JsonConvert.DeserializeObject<CBRXmlDailyResponse>(response);

        public (Currency, Currency) GetInitPair()
        {
            return (Valute["RUB"], Valute["USD"]);
        }
    }
}
