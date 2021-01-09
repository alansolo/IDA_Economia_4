using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class CatCapital
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public DateTime Creado { get; set; }
        public Nullable<DateTime> Modificado { get; set; }
        public string UsuarioModificado { get; set; }
        public bool Check { get; set; }
    }
}
