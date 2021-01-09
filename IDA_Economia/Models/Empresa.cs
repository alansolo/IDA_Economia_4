using IDA_Economia.EntidadYahooFinanceApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YahooFinance.NET;

namespace IDA_Economia.Models
{
    public class Empresa
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public double Rendimiento { get; set; }
        public double MediaRendimiento { get; set; }
        public double VarianzaRendimiento { get; set; }
        public double DesviacionRendimiento { get; set; }
        public double Varianza { get; set; }
        public double Media { get; set; }
        public double TotalDias { get; set; }
        public decimal MaxPrecio { get; set; }
        public decimal MinPrecio { get; set; }
        //public List<YahooHistoricalPriceData> ListaPrecio = new List<YahooHistoricalPriceData>();
        //public List<YahooFinanceAPI.Models.HistoryPrice> ListaPrecio = new List<YahooFinanceAPI.Models.HistoryPrice>();
        public List<CandleT> ListaPrecio { get; set; }
    }
}