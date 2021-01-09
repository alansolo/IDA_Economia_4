using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WhatsAppApi;

namespace IDA_Economia.Controllers
{
    public class WhatsappController : Controller
    {
        // GET: Whatsapp
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MandarWhats(string cadena)
        {
            string Ocadena = cadena;
            char delimitador = ';';

            string[] parametros = Ocadena.Split(delimitador);

            string from = "";
            string to = parametros[1];
            string message = "";
            string pitchName = parametros[0];

            try
            {
                WhatsAppApi.WhatsApp whats = new WhatsAppApi.WhatsApp(from, "imeistring", "nick", false, false);

                whats.OnConnectSuccess += () =>
                {
                    whats.SendMessage(to, message);
                    Session["EnvioExitoso"] = "El mensaje fue enviado con éxito";

                    whats.OnLoginFailed += (data) =>
                    {
                        Session["LogeoFallido"] = "Login failed : {0}";
                    };
                    whats.Login();


                };

                whats.OnConnectFailed += (ex) =>
                {
                    Session["EnvioFallido"] = "Conexión fallida";
                };

                whats.Connect();
            }
            catch (Exception ex)
            {

            }

            return View();
            
        }
    }
}