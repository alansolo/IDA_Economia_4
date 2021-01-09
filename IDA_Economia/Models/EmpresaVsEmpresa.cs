using IDA_Economia.EntidadYahooFinanceApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YahooFinance.NET;

namespace IDA_Economia.Models
{
    public class EmpresaVsEmpresa
    {
        public string NombreX { get; set; }
        public int CantidadX { get; set; }
        public double RendimientoX { get; set; }
        public double MediaRendimientoX { get; set; }
        public double VarianzaRendimientoX { get; set; }
        public double DesviacionRendimientoX { get; set; }
        public double VarianzaX { get; set; }
        //public List<YahooHistoricalPriceData> ListaPrecioX { get; set; }
        //public List<YahooFinanceAPI.Models.HistoryPrice> ListaPrecioX { get; set; }
        public List<CandleT> ListaPrecioX = new List<CandleT>();
        public int indiceX { get; set; }
        public string NombreY { get; set; }
        public int CantidadY { get; set; }
        public double RendimientoY { get; set; }
        public double MediaRendimientoY { get; set; }
        public double VarianzaRendimientoY { get; set; }
        public double DesviacionRendimientoY { get; set; }
        public double VarianzaY { get; set; }
        //public List<YahooHistoricalPriceData> ListaPrecioY { get; set; }
        //public List<YahooFinanceAPI.Models.HistoryPrice> ListaPrecioY { get; set; }
        public List<CandleT> ListaPrecioY = new List<CandleT>();
        public int indiceY { get; set; }
        public double RendimientoXPorY { get; set; }
        public double Covarianza { get; set; }
    }
}