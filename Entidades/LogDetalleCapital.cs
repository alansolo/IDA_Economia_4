using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class LogDetalleCapital
    {
        public long Id { get; set; }
        public long IdLog { get; set; }
        public long Numero { get; set; }
        public decimal Rendimiento { get; set; }
        public decimal Sigma { get; set; }
        public string Empresa { get; set; }
        public decimal Valor { get; set; }
        public string Usuario { get; set; }
        public DateTime Creado { get; set; }
        public string StrCreado { get; set; }
    }
}
