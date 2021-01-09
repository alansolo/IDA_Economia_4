using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.Models.MercadoDivisa
{
    public class ResultadoMercadoDivisa
    {
        public List<DatosDivisa> ListaDatos { get; set; }
        public List<CatDivisa> ListaCatDivisa { get; set; }
        public string Mensaje { get; set; }
    }
}