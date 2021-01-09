using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.Models
{
    public class CurvaVarianza
    {
        public int Numero { get; set; }
        public double RendimientoAsumido { get; set; }
        public double Sigma { get; set; }
        public double[,] W { get; set; }
    }
}