using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class LogDetalleDinero
    {
        public long Id { get; set; }
        public long IdLog { get; set; }
        public string Empresa { get; set; }
        public DateTime Fecha { get; set; }
        public string StrFecha { get; set; }
        public decimal Valor { get; set; }
        public string Usuario { get; set; }
        public DateTime Creado { get; set; }
        public string StrCreado { get; set; }
    }
}
