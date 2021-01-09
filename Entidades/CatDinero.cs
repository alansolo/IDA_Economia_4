using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class CatDinero
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public DateTime Creado { get; set; }
        public Nullable<DateTime> Modificado { get; set; }
        public string UsuarioModificado { get; set; }
        public string Check { get; set; }
    }
}
