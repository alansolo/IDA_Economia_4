using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.Models.MercadoDinero
{
    public class ResultadoMercadoDinero
    {
        public List<DatosDinero> ListaDatos { get; set; }
        public List<CatDinero> ListaCatDinero { get; set; }
        public string Mensaje { get; set; }
    }
}