using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class Usuario
    {
        public long Id { get; set; }
        public long IdRol { get; set; }
        public string Rol { get; set; }
        public string Login { get; set; }
        public string PasswordVisible { get; set; }
        public string Password { get; set; }
        public string ConfirmarPassword { get; set; }
        public string Nombre { get; set; }
        public bool Estatus { get; set; }
        public string StrEstatus { get; set; }
    }
}
