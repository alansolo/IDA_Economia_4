using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class Log
    {
        public long Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Creado { get; set; }
        public string StrCreado { get; set; }
        public string Modulo { get; set; }
        public string Empresa { get; set; }
        public string Resumen { get; set; }
        public string Detalle { get; set; }
        public List<LogDetalleDivisa> DetalleDivisa { get; set; }
        public List<LogDetalleDinero> DetalleDinero { get; set; }
        public List<LogDetalleCapital> DetalleCapital { get; set; }
        public bool EsDetalle { get; set; }
    }
}
