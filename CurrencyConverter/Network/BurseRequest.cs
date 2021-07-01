using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Network
{
    /// <summary>
    /// Запрос на биржу
    /// </summary>
    class BurseRequest
    {
        public static (string ResponseString, HttpStatusCode ResponseCode) getStockQuotes(string url)
        {
            string response_string = "";
            HttpWebRequest request = (HttpWebRequest)  WebRequest.Create(url);
            using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                using (Stream stream = response.GetResponseStream())   
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        response_string = reader.ReadToEnd();
                        return (response_string, response.StatusCode);
                    }
                        
            
        }
    }
}
