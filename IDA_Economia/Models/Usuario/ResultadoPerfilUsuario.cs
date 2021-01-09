using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IDA_Economia.Models.Usuario
{
    public class ResultadoPerfilUsuario
    {   
        public List<Entidades.PerfilUsuario> ListaPerfilUsuario { get; set; }
        public Entidades.PerfilUsuario PerfilUsuario { get; set; }
        public string Mensaje { get; set; }
        public bool EsSistema { get; set; }
    }
}