using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.Models.Usuario
{
    public class ResultadoUsuario
    {
        public List<Entidades.Usuario> ListaUsuario { get; set; }
        public List<Entidades.CatRol> ListaCatRol { get; set; }
        public Entidades.Usuario Usuario { get; set; }
        public Entidades.CatRol CatRol { get; set; }
        public string Mensaje { get; set; }
        public bool EsSistema { get; set; }
    }
}