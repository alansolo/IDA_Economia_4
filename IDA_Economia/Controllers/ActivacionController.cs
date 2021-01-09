using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Seguridad;
using System.Configuration;
using DocumentFormat.OpenXml.Bibliography;
using Datos.Usuario;
using System.Data;
using Newtonsoft.Json;
using Entidades;

namespace IDA_Economia.Controllers
{
    public class ActivacionController : Controller
    {
        // GET: Activacion
        public ActionResult ActivacionCuenta(string id)
        {
            try
            {
                id = id.Replace("|","/");

                string key = "idaeconomia".ToString();

                string parametro = EncripDecrip.Desencriptar(id, key);
                List<string> parametros = parametro.Split('|').ToList();

                if (parametros.Count > 0)
                {
                    string idBase = parametros[0];
                    string cuenta = parametros[1];
                }
                else
                {
                    ViewBag.Mensaje = "No se encontraron parametros";

                    return View();
                }

                //DESCIFRAR PARAMETRO

                Parametro oparametro = new Parametro();
                List<Parametro> ListParametro = new List<Parametro>();

                oparametro = new Parametro();
                oparametro.Nombre = "Id";
                oparametro.Valor = parametros[0];
                ListParametro.Add(oparametro);

                oparametro = new Parametro();
                oparametro.Nombre = "Correo";
                oparametro.Valor = parametros[1];
                ListParametro.Add(oparametro);

                oparametro = new Parametro();
                oparametro.Nombre = "Estatus";
                oparametro.Valor = "1";
                ListParametro.Add(oparametro);


                //CONEXION CON LA BASE DE DATOS
                object Resultado = new object();
                BDUsuario bdUsuario = new BDUsuario();
                const string spName = "ActivarUsuario";
                DataTable dtResultado = new DataTable();
                Entidades.PerfilUsuario usuarioactivado = new Entidades.PerfilUsuario();

                try
                {
                    Resultado = bdUsuario.InsertUsuario(spName, ListParametro);

                    dtResultado = (DataTable)Resultado;

                    if (dtResultado.Rows.Count > 0)
                    {
                        var jsonListUsuario = JsonConvert.DeserializeObject<Entidades.PerfilUsuario>(dtResultado.Rows[0][0].ToString());
                        usuarioactivado = jsonListUsuario;

                        if (usuarioactivado.Estatus)
                        {
                            usuarioactivado.StrEstatus = "Activo";
                        }
                        else
                        {
                            usuarioactivado.StrEstatus = "Inactivo";
                        }

                        usuarioactivado.Confirmacion = usuarioactivado.Password;
                    }

                }
                catch (Exception ex)
                {
                }

                //CONSTRUIR MENSAJE PARA LA PAGINA
                ViewBag.Mensaje = "Tu cuenta se creo con exito";
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "No se pudo actualizar tu cuenta.";
            }

            return View();
        }
    }
}