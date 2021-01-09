using ClosedXML.Excel;
using Entidades;
using Herramientas;
using IDA_Economia.Models;
using IDA_Economia.Models.MercadoDinero;
using Negocio.MercadoDinero;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Web;
using System.Web.Mvc;

namespace IDA_Economia.Controllers
{
    public class MercadoDineroController : Controller
    {
        // GET: MercadoDinero
        [HttpGet]
        public ActionResult MercadoDinero()
        {
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerCatDinero()
        {
            List<CatDinero> ListCatDinero = new List<CatDinero>();

            MercadoDinero mercadoDinero = new MercadoDinero();
            List<Parametro> ListParametro = new List<Parametro>();

            ResultadoMercadoDinero resultadoMercadoDinero = new ResultadoMercadoDinero();

            string mensaje = string.Empty;

            try
            {
                ListCatDinero = mercadoDinero.ObtenerCatDinero(ListParametro);

                Session["ListCatDinero"] = ListCatDinero;

                //SELECCIONAR EL PRIMER ELEMENTO
                if (ListCatDinero.Count > 0)
                {
                    ListCatDinero[0].Check = ListCatDinero[0].Valor;
                }

                resultadoMercadoDinero.ListaCatDinero = ListCatDinero;
                resultadoMercadoDinero.Mensaje = "OK";
            }
            catch(Exception ex)
            {
                mensaje = "ERROR: Metodo: ObtenerEstadistico_Dinero, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);

                resultadoMercadoDinero.Mensaje = mensaje;
            }

            return Json(resultadoMercadoDinero, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObtenerEstadistico(string strFechaInicio, string strFechaFinal, List<CatDinero> ListCatDinero)
        {
            ResultadoMercadoDinero resultadoMercadoDinero = new ResultadoMercadoDinero();
            resultadoMercadoDinero.ListaDatos = new List<DatosDinero>();
            DatosDinero datosDinero = new DatosDinero();

            string seriesID = string.Empty;
            string dinero = string.Empty;
            string fechainicio = string.Empty;
            string fechafinal = string.Empty;

            CatDinero catDineroDefault = new CatDinero();

            List<Parametro> listParametro = new List<Parametro>();
            Parametro parametro = new Parametro();

            List<GrupoParametro> listGrupoParametro = new List<GrupoParametro>();
            GrupoParametro grupoParametro = new GrupoParametro();
            List<Parametro> listParametroDetalle = new List<Parametro>();

            string Lang = "es-MX";//set your culture here           

            string mensaje = string.Empty;

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo(Lang);

                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Lang);

                //Se solicita la información al cliente para definir el código que realizará la petición al API:
                //string valor = "";

                //OBTENER LA DIVISA SELECCIONADA

                catDineroDefault = ListCatDinero.Where(n => !string.IsNullOrEmpty(n.Check)).FirstOrDefault();

                if (catDineroDefault != null)
                {
                    seriesID = catDineroDefault.Valor;
                    dinero = catDineroDefault.Nombre;

                }
                else
                {
                    resultadoMercadoDinero.Mensaje = "Debe seleccionar por lo menos una divisa.";

                    return Json(resultadoMercadoDinero, JsonRequestBehavior.AllowGet);
                }

                fechainicio = Convert.ToDateTime(strFechaInicio).ToString("yyyy-MM-dd");

                fechafinal = Convert.ToDateTime(strFechaFinal).ToString("yyyy-MM-dd");

                string url = "https://www.banxico.org.mx/SieAPIRest/service/v1/series/" + seriesID + "/datos/" + fechainicio + "/" + fechafinal;

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //Se crea una cadena con los valores que se requiera consumir
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Accept = "application/json; charset=utf-8";
                request.Headers["Bmx-Token"] = "6af6e6645653ed1cb3ecb5165c3d30df2d5289600811f7067b2169c9ff030eb4";
                //request.Method = "GET";
                request.PreAuthenticate = true;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception(String.Format(
                    "Server error (HTTP {0}: {1}).",
                    response.StatusCode,
                    response.StatusDescription));

                //De esta forma se obtiene el JSON de la respuesta en una cadena. Esta cadena puede ser mapeada a objetos de la siguiente forma:
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));


                object c = jsonSerializer.ReadObject(response.GetResponseStream());

                Response resBan = (Response)c;

                var uno1 = resBan.seriesResponse.series[0];

                if (uno1.Data == null)
                {
                    resultadoMercadoDinero.Mensaje = "No Datos";

                    return Json(resultadoMercadoDinero, JsonRequestBehavior.AllowGet);
                }


                //LINQ C#
                //PARA BUSCAR MAS INFORMACION
                //var dos1 = uno1.Data.Where(n => (Convert.ToDecimal(n.Data)) > 9).ToList();

                var dos1 = uno1.Data.ToList();

                DataTable dt1 = new DataTable();

                dt1.Columns.Add("Fecha");
                dt1.Columns.Add("Valor");

                if (Session["Usuario"] == null)
                {
                    resultadoMercadoDinero.Mensaje = "Sesion Expirada";

                    return Json(resultadoMercadoDinero, JsonRequestBehavior.AllowGet);
                }


                listGrupoParametro = new List<GrupoParametro>();

                int cont = 0;
                dos1.ForEach(m =>
                {
                    dt1.Rows.Add(m.Date, Convert.ToDouble(m.Data).ToString("0.00##"));

                    datosDinero = new DatosDinero();
                    datosDinero.Fecha = m.Date;
                    datosDinero.Valor = Convert.ToDouble(m.Data);

                    resultadoMercadoDinero.ListaDatos.Add(datosDinero);

                    listParametroDetalle = new List<Parametro>();

                    //AGREGAR DETALLE DE LOG
                    parametro = new Parametro();
                    parametro.Nombre = "Empresa";
                    parametro.Valor = dinero;

                    listParametroDetalle.Add(parametro);

                    parametro = new Parametro();
                    parametro.Nombre = "Fecha";
                    parametro.Valor = DateTime.ParseExact(m.Date, "dd/MM/yyyy", CultureInfo.CurrentCulture);

                    listParametroDetalle.Add(parametro);

                    parametro = new Parametro();
                    parametro.Nombre = "Valor";
                    parametro.Valor = m.Data;

                    listParametroDetalle.Add(parametro);

                    parametro = new Parametro();
                    parametro.Nombre = "Usuario";
                    parametro.Valor = ((Entidades.Usuario)Session["Usuario"]).Login;

                    listParametroDetalle.Add(parametro);

                    grupoParametro = new GrupoParametro();
                    grupoParametro.ListGrupoParametro = listParametroDetalle;
                    listGrupoParametro.Add(grupoParametro);

                    cont++;
                });


                Session["dtInformacionDinero"] = dt1;

                //INSERTAR INFORMACION LOG
                listParametro = new List<Parametro>();

                parametro = new Parametro();
                parametro.Nombre = "Usuario";
                parametro.Valor = ((Entidades.Usuario)Session["Usuario"]).Login;

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Modulo";
                parametro.Valor = "Mercado Dinero";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Empresa";
                parametro.Valor = dinero;

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Resumen";
                parametro.Valor = "Generar Estadistico Mercado Divisa";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Detalle";
                parametro.Valor = "Fecha Inicio: " + strFechaInicio + ", Fecha Final: " + strFechaFinal;

                listParametro.Add(parametro);               

                //INSERTAR LOG
                Negocio.Log.Log log = new Negocio.Log.Log();
                log.InsertLogDinero(listParametro, listGrupoParametro);

                resultadoMercadoDinero.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: Metodo: ObtenerEstadistico_Dinero, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);

                resultadoMercadoDinero.Mensaje = mensaje;
            }

            return Json(resultadoMercadoDinero, JsonRequestBehavior.AllowGet);
        }
        public void ExportarExcel()
        {
            string nombreArchivo = "Historico_Dinero.xlsx";
            string hojaArchivo = "Dinero";
            DataTable dtInformacion = new DataTable();

            string mensaje = string.Empty;

            try
            {
                dtInformacion = (DataTable)Session["dtInformacionDinero"];

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtInformacion, hojaArchivo);

                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("content-disposition", "attachment; filename=" + nombreArchivo);

                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }

                }
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: Metodo: ExportarExcel_Dinero, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);
            }

        }
    }
}