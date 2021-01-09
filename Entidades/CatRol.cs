using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class CatRol
    {
        public long Id { get; set; }
        public string Rol { get; set; }
        public string Descripcion { get; set; }
        public DateTime Creado { get; set; }
        public Nullable<DateTime> Modificado { get; set; }
        public string UsuarioModificado { get; set; }
    }
}
