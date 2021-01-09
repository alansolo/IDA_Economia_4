using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.Models.MercadoCapital
{
     
    public class ResultadoMercadoCapital
    {
        public List<Empresa> ListaEmpresa { get; set; }
        public List<EncabezadoEmpresa> ListaEncabezadoEmpresa { get; set; }
        public List<CurvaVarianza> ListaCurvaVarianza { get; set; }
        public List<CalculoMercadoCapital> ListaCalculoMercadoCapital { get; set; }
        public List<DatosMecadoCapital> ListaDatos { get; set; }
        public List<CatCapital> ListaCatCapital { get; set; }
        public string Mensaje { get; set; }
    }
}