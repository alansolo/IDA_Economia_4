using Entidades;
using Negocio.Login;
using Negocio.MercadoCapital;
using Negocio.MercadoDivisa;
using Seguridad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IDA_Economia.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ValidarLogin(string usuario, string password)
        {
            string mensejeError = string.Empty;
            string userDefault = string.Empty;
            string passwordDefault = string.Empty;
            List<Parametro> listParametro = new List<Parametro>();
            Parametro parametro = new Parametro();
            string passwordEncrip = string.Empty;
            string passwordDecrip = string.Empty;
            const string key = "idaeconomia";
            List<CatPantalla> listaPantalla = new List<CatPantalla>();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
            {
                mensejeError = "Agregue el usuario y el password correctamente.";
            }
            else
            {
                Entidades.Usuario Usuario = new Usuario();

                parametro.Nombre = "Usuario";
                parametro.Valor = usuario;

                listParametro.Add(parametro);

                Login login = new Login();

                Usuario = login.ObtenerUsuario(listParametro);

                if (!Usuario.Estatus)
                {
                    mensejeError = "El usuario y password son incorrectos.";

                    return Json(mensejeError, JsonRequestBehavior.AllowGet);
                }

                passwordEncrip = EncripDecrip.Encriptar(password, key);

                //passwordDecrip = EncripDecrip.Desencriptar(passwordEncrip, key);

                userDefault = Usuario.Login;
                passwordDefault = Usuario.Password;

                if (usuario == userDefault && passwordEncrip == passwordDefault)
                {
                    Session["Usuario"] = Usuario;
                    mensejeError = "OK";

                    ((Usuario)Session["Usuario"]).Nombre = "nombre";

                    Usuario usu = (Usuario)Session["Usuario"];

                    //OBTENER MENUS DEL USUARIO
                    listParametro = new List<Parametro>();

                    parametro = new Parametro();
                    parametro.Nombre = "IdRol";
                    parametro.Valor = Usuario.IdRol;

                    listParametro.Add(parametro);

                    listaPantalla = login.ObtenerPantalla(listParametro);

                    Session["ListPantalla"] = listaPantalla;

                    //INSERTAR INFORMACION LOG
                    listParametro = new List<Parametro>();

                    parametro = new Parametro();
                    parametro.Nombre = "Usuario";
                    parametro.Valor = userDefault;

                    listParametro.Add(parametro);

                    parametro = new Parametro();
                    parametro.Nombre = "Modulo";
                    parametro.Valor = "Login";

                    listParametro.Add(parametro);

                    parametro = new Parametro();
                    parametro.Nombre = "Empresa";
                    parametro.Valor = "Inicio sesion";

                    listParametro.Add(parametro);

                    parametro = new Parametro();
                    parametro.Nombre = "Resumen";
                    parametro.Valor = "Inicio sesion";

                    listParametro.Add(parametro);

                    Negocio.Log.Log log = new Negocio.Log.Log();
                    log.InsertLog(listParametro);
                }
                else
                {
                    mensejeError = "El usuario y password son incorrectos.";
                }
            }

            return Json(mensejeError, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CerrarSesion()
        {
            string mensaje = string.Empty;
            List<Parametro> listParametro = new List<Parametro>();
            Parametro parametro = new Parametro();
            string usuario = string.Empty;

            try
            {
                usuario = ((Usuario)Session["Usuario"]).Login;

                //INSERTAR INFORMACION LOG
                listParametro = new List<Parametro>();

                parametro = new Parametro();
                parametro.Nombre = "Usuario";
                parametro.Valor = usuario;

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Modulo";
                parametro.Valor = "Cerrar";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Empresa";
                parametro.Valor = "Cerrar sesion";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Resumen";
                parametro.Valor = "Cerrar sesion";

                listParametro.Add(parametro);

                Negocio.Log.Log log = new Negocio.Log.Log();
                log.InsertLog(listParametro);

                Session.Abandon();
                Session.Clear();

                //MENSAJE X

                mensaje = "OK";             
            }
            catch (Exception ex)
            {

            }

            return Json(mensaje, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Register()
        {
            //Comentario
            return View();
        }
    }
}