using CurrencyConverter.Model.CBR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Model
{
    public class Currency
    {
        public string Name { get; set; }
        public string ID { get; set; }
        public string CharCode { get; set; }
        public string NumCode { get; set; }
        public int Nominal { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }

    public interface IFinanceExchange
    {
        string Date { get; }
        string Timestamp { get;  }
        SortedDictionary<string, Currency> Valute { get;}
        IEnumerable<string> GetCurrenciesNames();
        IEnumerable<Currency> GetCurrencies();
        (Currency, Currency) GetInitPair();
    }

    abstract class FinanceSource
    {
        public string Name { get; protected set; }
        public string Url { get; protected set; }
        protected FinanceSource(string name, string url)
        {
            Name = name;
            Url = url;
        }
        public abstract IFinanceExchange DoRequestWintHandle();
    }
}
