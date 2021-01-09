using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    [Serializable]
    public class PerfilUsuario
    {
        public long Id { get; set; }
        public long IdRol { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; }
        public string Confirmacion { get; set; }
        public bool Estatus { get; set; }
        public string StrEstatus { get; set; }
        public string Estudios { get; set; }
        public string Perfil { get; set; }
        public string Ocupacion { get; set; }
        public string Experiencia { get; set; }
        public string Marketing { get; set; }
    }
}
