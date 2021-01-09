using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Runtime.Serialization.Json;
using IDA_Economia.Models;
using System.Data;
using System.Text;
using IDA_Economia.Models.MercadoDivisa;
using ClosedXML.Excel;
using System.IO;
using System.Net.Http;
using Entidades;
using Negocio.MercadoDivisa;
using System.Globalization;
using Herramientas;

namespace IDA_Economia.Controllers
{
    public class MercadoDivisaController : Controller
    {
        // GET: MercadoDivisa
        [HttpGet]
        public ActionResult MercadoDivisa()
        {
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerCatDivisa()
        {
            List<CatDivisa> ListCatDivisa = new List<CatDivisa>();

            MercadoDivisa mercadoDivisa = new MercadoDivisa();
            List<Parametro> ListParametro = new List<Parametro>();

            ResultadoMercadoDivisa resultadoMercadoDivisa = new ResultadoMercadoDivisa();

            string mensaje = string.Empty;

            try
            {
                ListCatDivisa = mercadoDivisa.ObtenerCatDivisa(ListParametro);

                Session["ListCatDivisa"] = ListCatDivisa;

                //SELECCIONAR EL PRIMER ELEMENTO
                if (ListCatDivisa.Count > 0)
                {
                    ListCatDivisa[0].Check = ListCatDivisa[0].Valor;
                }

                resultadoMercadoDivisa.ListaCatDivisa = ListCatDivisa;
                resultadoMercadoDivisa.Mensaje = "OK";
            }
            catch(Exception ex)
            {
                mensaje = "ERROR: Metodo: ObtenerInfoInicio_Divisa, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);

                resultadoMercadoDivisa.Mensaje = mensaje;
            }

            return Json(resultadoMercadoDivisa, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObtenerEstadistico(string strFechaInicio, string strFechaFinal, List<CatDivisa> ListCatDivisa)
        {
            ResultadoMercadoDivisa resultadoMercadoDivisa = new ResultadoMercadoDivisa();
            resultadoMercadoDivisa.ListaDatos = new List<DatosDivisa>();
            DatosDivisa datosDivisa = new DatosDivisa();

            string seriesID = "";
            string fechainicio = "";
            string fechafinal = "";

            CatDivisa catDivisaDefault = new CatDivisa();
            string divisa = string.Empty;

            List<Parametro> listParametro = new List<Parametro>();
            Parametro parametro = new Parametro();

            List<GrupoParametro> listGrupoParametro = new List<GrupoParametro>();
            GrupoParametro grupoParametro = new GrupoParametro();
            List<Parametro> listParametroDetalle = new List<Parametro>();

            string mensaje = string.Empty;

            string Lang = "es-MX";//set your culture here

            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo(Lang);

                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Lang);

                //Se solicita la información al cliente para definir el código que realizará la petición al API:
                //string valor = "";

                //OBTENER LA DIVISA SELECCIONADA
                catDivisaDefault = ListCatDivisa.Where(n => !string.IsNullOrEmpty(n.Check)).FirstOrDefault();

                if (catDivisaDefault != null)
                {
                    seriesID = catDivisaDefault.Valor;
                    divisa = catDivisaDefault.Nombre;
                }
                else
                {
                    resultadoMercadoDivisa.Mensaje = "Debe seleccionar por lo menos una moneda.";

                    return Json(resultadoMercadoDivisa, JsonRequestBehavior.AllowGet);
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

                //LINQ C#
                //PARA BUSCAR MAS INFORMACION
                //var dos1 = uno1.Data.Where(n => (Convert.ToDecimal(n.Data)) > 9).ToList();

                if(uno1.Data == null)
                {
                    resultadoMercadoDivisa.Mensaje = "No Datos";

                    return Json(resultadoMercadoDivisa, JsonRequestBehavior.AllowGet);
                }

                var dos1 = uno1.Data.ToList();

                DataTable dt1 = new DataTable();

                dt1.Columns.Add("Fecha");
                dt1.Columns.Add("Valor");


                if (Session["Usuario"] == null)
                {
                    resultadoMercadoDivisa.Mensaje = "Sesion Expirada";

                    return Json(resultadoMercadoDivisa, JsonRequestBehavior.AllowGet);
                }

                listGrupoParametro = new List<GrupoParametro>();

                int cont = 0;
                dos1.ForEach(m =>
                {
                    dt1.Rows.Add(m.Date, Convert.ToDouble(m.Data).ToString("0.0000##"));                    

                    datosDivisa = new DatosDivisa();
                    datosDivisa.Fecha = m.Date;
                    datosDivisa.Valor = Convert.ToDouble(m.Data);

                    resultadoMercadoDivisa.ListaDatos.Add(datosDivisa);

                    listParametroDetalle = new List<Parametro>();

                    //AGREGAR DETALLE DE LOG
                    parametro = new Parametro();
                    parametro.Nombre = "Empresa";
                    parametro.Valor = divisa;

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


                Session["dtInformacionDivisa"] = dt1;


                //INSERTAR INFORMACION LOG
                listParametro = new List<Parametro>();

                parametro = new Parametro();
                parametro.Nombre = "Usuario";
                parametro.Valor = ((Entidades.Usuario)Session["Usuario"]).Login;

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Modulo";
                parametro.Valor = "Mercado Divisa";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Empresa";
                parametro.Valor = divisa;

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
                log.InsertLogDivisa(listParametro, listGrupoParametro);

                resultadoMercadoDivisa.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: Metodo: ObtenerEstadistico_Divisa, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);

                resultadoMercadoDivisa.Mensaje = mensaje;
            }

            return Json(resultadoMercadoDivisa, JsonRequestBehavior.AllowGet);
        }
        public void ExportarExcel()
        {
            string nombreArchivo = "Historico_Divisa.xlsx";
            string hojaArchivo = "Divisa";
            DataTable dtInformacion = new DataTable();

            string mensaje = string.Empty;

            try
            {
                dtInformacion = (DataTable)Session["dtInformacionDivisa"];

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
                mensaje = "ERROR: Metodo: ExportarExcel_Divisa, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);
            }

        }
    }

}