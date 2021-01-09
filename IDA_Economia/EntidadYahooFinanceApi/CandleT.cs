using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.EntidadYahooFinanceApi
{
    public class CandleT
    {
        public DateTime DateTime { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Volume { get; set; }
        public decimal AdjustedClose { get; set; }
        public double Rendimiento { get; set; }
    }
}