using System;
using System.Collections.Generic;
using System.Text;

namespace bitcoin_app
{
    public class BitcoinResponse
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public int rank { get; set; }
        public string price_usd { get; set; }
    }
}
