using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Seguridad;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace Negocio.Correo
{
    public class EnvioCorreo
    {
        public static string Envio(string correoTo, long id)
        {
            string resultado = string.Empty;

            string correoFrom = "idaeconomia.stokhos@gmail.com";//"contacto.stokhos@ida-economia.com"
            string hostUser = "idaecfod";
            string hostPassword = "idA_7324*";
            //string correoTo = "alvarejo9@gmail.com";
            string passwordCorreoFrom = "stokhosEslaPalabraGriegaparaEstocastico"; //"5t0khOs+*+*9"
            //string pathArchivoHtml = @"C:\Users\jalva\Documents\EnvioCorreo\Html\LayoutCorreo.html";

            try
            {
                string pathArchivoHtml = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("/IDA_Economia/bin/Negocio.DLL", "").Replace("file:///", "");
                pathArchivoHtml = pathArchivoHtml + "\\Negocio\\Html\\LayoutCorreo.html";

                string key = "idaeconomia";

                string parametro = EncripDecrip.Encriptar(id + "|" + correoTo, key);

                StringBuilder archivoHtml = new StringBuilder();
                archivoHtml.Append(File.ReadAllText(pathArchivoHtml));

                archivoHtml.Replace("#TITULO", "Correo de activación de cuenta");
                archivoHtml.Replace("#MENSAJE", "Da clic en el siguiente enlace para activar tu cuenta");
                archivoHtml.Replace("#URL", "http://localhost:9426/Activacion/ActivacionCuenta/" + parametro.Replace("/", "|"));

                MailMessage message = new MailMessage();
                message.From = new MailAddress(correoFrom);
                message.To.Add(new MailAddress(correoTo));
                message.Subject = "Test";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = archivoHtml.ToString();

                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587; //465 o 25 para plesk
                smtp.Host = "smtp.gmail.com"; //for gmail host  "204.11.59.220" "webmail.ida-economia.com"
                smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(correoFrom, passwordCorreoFrom);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                try
                {
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    resultado = "Error: No se encontro la cuenta de correo";
                }

                resultado = "OK";
            }
            catch (Exception ex)
            {
                resultado = "Error: ocurrio un problema con el envio de correo.";
            }

            return resultado;
        }

        public static string Recuperacion(string correoTo)
        {
            string resultado = string.Empty;
            string key = "idaeconomia";
            string correoFrom = "contacto.stokhos@ida-economia.com";
            //string correoTo = "alvarejo9@gmail.com";
            string passwordCorreoFrom = "5t0khOs+*+*9";
            //string pathArchivoHtml = @"C:\Users\jalva\Documents\EnvioCorreo\Html\LayoutCorreo.html";

            try
            {
                string pathArchivoHtml = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Replace("/IDA_Economia/bin/Negocio.DLL", "").Replace("file:///", "");
                pathArchivoHtml = pathArchivoHtml + "\\Negocio\\Html\\LayoutCorreo2.html";

                //string key = ConfigurationManager.AppSettings["key"].ToString();

                string parametro = EncripDecrip.Encriptar(correoTo, key);

                StringBuilder archivoHtml = new StringBuilder();
                archivoHtml.Append(File.ReadAllText(pathArchivoHtml));

                archivoHtml.Replace("#TITULO", "Reestablece tu contraseña");
                archivoHtml.Replace("#MENSAJE", "Da clic en el siguiente enlace para reestablecerla");
                archivoHtml.Replace("#URL", "http://localhost:9426/Usuario/Reestablecer/" + parametro.Replace("/", "|"));

                MailMessage message = new MailMessage();
                message.From = new MailAddress(correoFrom);
                message.To.Add(new MailAddress(correoTo));
                message.Subject = "Test";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = archivoHtml.ToString();

                SmtpClient smtp = new SmtpClient();
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //"localhost" for gmail host "204.11.59.220"  
                smtp.EnableSsl = false;
                /*smtp.UseDefaultCredentials = true*/;
                smtp.Credentials = new NetworkCredential(correoFrom, passwordCorreoFrom);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                try
                {
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    resultado = "Error: No se encontro la cuenta de correo";
                }

                resultado = "OK";
            }
            catch (Exception ex)
            {
                resultado = "Error: ocurrio un problema con el envio de correo.";
            }

            return resultado;
        }
    }
}
