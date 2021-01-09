using ClosedXML.Excel;
using Entidades;
using Herramientas;
using IDA_Economia.EntidadYahooFinanceApi;
using IDA_Economia.Models;
using IDA_Economia.Models.MercadoCapital;
using Negocio.MercadoCapital;
using OperacionMatriz;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YahooFinance.NET;

namespace IDA_Economia.Controllers
{
    public class MercadoCapitalController : Controller
    {
        string cookie = "d3d52ihf1icfd&b=3&s=2l"; //"1a9rkttd53me9&b=3&s=8a"; //"5b47s4pehkvjd&b=3&s=is";//
        string crumb = "C9luNcNjVkK"; //"SPwNBQX77jD"; //"ourzFxbliAq";//

        // GET: MercadoCapital
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MercadoCapital()
        {
            if (Session["Usuario"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
        [HttpPost]
        public JsonResult ObtenerCatCapital()
        {
            List<CatCapital> ListCatCapital = new List<CatCapital>();

            MercadoCapital mercadoCapital = new MercadoCapital();
            List<Parametro> ListParametro = new List<Parametro>();

            ResultadoMercadoCapital resultadoMercadoCapital = new Models.MercadoCapital.ResultadoMercadoCapital();

            string mensaje = string.Empty;

            try
            {
                ListCatCapital = mercadoCapital.ObtenerCatCapital(ListParametro);

                Session["ListCatCapital"] = ListCatCapital;

                //SELECCIONAR EL PRIMER ELEMENTO
                if (ListCatCapital.Count > 0)
                {
                    ListCatCapital[0].Check = true;
                }

                resultadoMercadoCapital.ListaCatCapital = ListCatCapital;

                resultadoMercadoCapital.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                mensaje = "ERROR: Metodo: ObtenerInfoInicio_Mercado, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);

                resultadoMercadoCapital.Mensaje = mensaje;
            }

            return Json(resultadoMercadoCapital, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ObtenerEstadistico(string strFechaInicio, string strFechaFinal, List<CatCapital> ListCatCapital)
        {
            ResultadoMercadoCapital resultadoMercadoCapital = new ResultadoMercadoCapital();

            DateTime fechaInicio = new DateTime();
            DateTime fechafinal = new DateTime();

            List<Parametro> listParametro = new List<Parametro>();
            Parametro parametro = new Parametro();

            double totalDias = 0;

            bool primerRegistro = true;
            int cont = 0;
            int cont2 = 0;
            double rendimiento = 0;
            double promedio = 0;
            double varianza = 0;
            double desviacion = 0;

            double inversion = 10000;
            //también se usa para calcular los rendimientos del portafolio pero no se agrega como variable sino como número por la forma del cálculo por matrices.

            //List<YahooHistoricalPriceData> yahooPriceHistoryTemp = new List<YahooHistoricalPriceData>();
            List<CandleT> yahooPriceHistoryTemp = new List<CandleT>();
            List<CandleT> yahooPriceHistory = new List<CandleT>();

            //LISTA DE EMPRESAS
            List<Empresa> ListaEmpresa = new List<Empresa>();
            List<EmpresaVsEmpresa> ListaEmpresaVsEmpresa = new List<EmpresaVsEmpresa>();

            List<CalculoMercadoCapital> ListaCalculoMercadoCapital = new List<CalculoMercadoCapital>();

            List<EncabezadoEmpresa> ListaEncabezadoEmpresa = new List<EncabezadoEmpresa>();
            EncabezadoEmpresa encabezadoEmpresa = new EncabezadoEmpresa();

            fechaInicio = Convert.ToDateTime(strFechaInicio);
            fechafinal = Convert.ToDateTime(strFechaFinal);

            List<GrupoParametro> listGrupoParametro = new List<GrupoParametro>();
            GrupoParametro grupoParametro = new GrupoParametro();
            List<Parametro> listParametroDetalle = new List<Parametro>();

            string mensaje = string.Empty;

            try
            {
                totalDias = (fechafinal - fechaInicio).TotalDays;

                //OBTENER EMPRESAS SELECCIONADAS
                foreach (CatCapital catCapital in ListCatCapital.Where(n => n.Check).ToList())
                {
                    Empresa empresa = new Empresa();
                    empresa.Nombre = catCapital.Valor;

                    ListaEmpresa.Add(empresa);
                }

                if(ListaEmpresa.Count <= 2)
                {
                    resultadoMercadoCapital.Mensaje = "Se debe seleccionar por lo menos tres empresas.";
                     
                    return Json(resultadoMercadoCapital, JsonRequestBehavior.AllowGet);
                }

                if (ListaEmpresa.Count > 7)
                {
                    resultadoMercadoCapital.Mensaje = "Se debe seleccionar como maximo 7 empresas.";

                    return Json(resultadoMercadoCapital, JsonRequestBehavior.AllowGet);
                }


                ////////////////////////
                //1.-DATOS
                ////////////////////////

                //OBTENER COTIZACIONES POR CADA EMPRESA
                //ListaEmpresa.ForEach(x =>
                foreach (Empresa x in ListaEmpresa)
                {
                    /////////////////////////
                    ///OBTENER CRUMB
                    ////////////////////////
                    //YahooFinance.NET.Token.Refresh();
                    //YahooFinance.NET.Token.Refresh();
                    //crumb = Token.Crumb;
                    //cookie = Token.Cookie;

                    //var yahooFinance = new YahooFinanceClient(cookie, crumb);
                    var yahooStockCode = x.Nombre; //yahooFinance.GetYahooStockCode(exchange, symbol);
                    
                    
                    Task<IReadOnlyList<YahooFinanceApi.Candle>> listaPriceData = YahooFinanceApi.Yahoo.GetHistoricalAsync(yahooStockCode, fechaInicio, fechafinal);

                    listaPriceData.Wait();
                    var yahooPriceHistoryAsync = listaPriceData.Result;

                    yahooPriceHistory = new List<CandleT>();
                    yahooPriceHistoryAsync.ToList().ForEach(n =>
                    {
                        CandleT c = new CandleT();
                        c.AdjustedClose = n.AdjustedClose;
                        c.Close = n.Close;
                        c.DateTime = n.DateTime;
                        c.High = n.High;
                        c.Low = n.Low;
                        c.Open = n.Open;
                        c.Volume = n.Volume;                       

                        yahooPriceHistory.Add(c);
                    });

                    //List<YahooHistoricalPriceData> yahooPriceHistory = yahooFinance.GetDailyHistoricalPriceData(yahooStockCode, fechaInicio, fechafinal);

                    //OBTENER REGISTROS TOTALES
                    x.Cantidad = yahooPriceHistory.Count;

                    //CALCULAR RENDIMIENTO
                    cont = 0;
                    primerRegistro = true;
                    foreach (var data in yahooPriceHistory)
                    {
                        if (primerRegistro)
                        {
                            primerRegistro = false;
                        }
                        else
                        {
                            if (yahooPriceHistory[cont - 1].Close != 0)
                            {
                                rendimiento = (Convert.ToDouble(data.Close) / Convert.ToDouble(yahooPriceHistory[cont - 1].Close)) - 1;
                                data.Rendimiento = rendimiento;
                            }
                            else
                            {
                                data.Rendimiento = 0;
                            }
                        }

                        cont++;
                    }

                    //AGREGAR LISTA DE PRECIOS
                    x.ListaPrecio = yahooPriceHistory;

                    //OBTENEMOS SUMATORIA DEL RENDIMIENTO
                    x.Rendimiento = yahooPriceHistory.Sum(n => n.Rendimiento);

                    //decimal promedio = yahooPriceHistory.Average(n => n.Close);
                    //x.Media = Convert.ToDouble(promedio);


                    //OBTENER VARIANZA RENDIMIENTO
                    double M = 0.0;
                    double S = 0.0;
                    int k = 1;

                    yahooPriceHistoryTemp = yahooPriceHistory.ToList();

                    if (yahooPriceHistoryTemp.Count > 0)
                    {
                        yahooPriceHistoryTemp.RemoveAt(0);

                        yahooPriceHistoryTemp.ForEach(n =>
                        {
                            double tmpM = M;
                            M += (Convert.ToDouble(n.Rendimiento) - tmpM) / k;
                            S += (Convert.ToDouble(n.Rendimiento) - tmpM) * (Convert.ToDouble(n.Rendimiento) - M);
                            k++;
                        });

                        varianza = S / (k - 2);
                        desviacion = Math.Sqrt(S / (k - 2));
                    }
                    else
                    {
                        varianza = 0;
                        desviacion = 0;
                    }

                    x.VarianzaRendimiento = varianza;
                    x.DesviacionRendimiento = desviacion;
                }

                //CALCULAR MEDIA RENDIMIENTO
                ListaEmpresa.ForEach(n =>
                {
                    promedio = n.Rendimiento / (n.Cantidad - 1);
                    n.MediaRendimiento = promedio;
                });

                ListaEmpresa.ForEach(n =>
                {
                    if (n.ListaPrecio.Count > 0)
                    {
                        n.MaxPrecio = n.ListaPrecio.Max(m => m.Close);
                        n.MinPrecio = n.ListaPrecio.Min(m => m.Close);
                    }
                    else
                    {
                        n.MaxPrecio = 0;
                        n.MinPrecio = 0;
                    }

                    
                });

                //dgvTotales.DataBind();

                //POSIBLES COMBINACIONES
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    //cont2 = cont + 1;
                    for (cont2 = cont + 1; ListaEmpresa.Count > cont2; cont2++)
                    {
                        EmpresaVsEmpresa empresaVsEmpresa = new EmpresaVsEmpresa();

                        //ELEMENTO X
                        empresaVsEmpresa.NombreX = ListaEmpresa[cont].Nombre;
                        empresaVsEmpresa.CantidadX = ListaEmpresa[cont].Cantidad;
                        empresaVsEmpresa.RendimientoX = ListaEmpresa[cont].Rendimiento;
                        empresaVsEmpresa.MediaRendimientoX = ListaEmpresa[cont].MediaRendimiento;
                        empresaVsEmpresa.VarianzaRendimientoX = ListaEmpresa[cont].VarianzaRendimiento;
                        empresaVsEmpresa.DesviacionRendimientoX = ListaEmpresa[cont].DesviacionRendimiento;
                        empresaVsEmpresa.VarianzaX = ListaEmpresa[cont].Varianza;
                        empresaVsEmpresa.ListaPrecioX = ListaEmpresa[cont].ListaPrecio;
                        empresaVsEmpresa.indiceX = cont;
                        //ELEMENTO Y
                        empresaVsEmpresa.NombreY = ListaEmpresa[cont2].Nombre;
                        empresaVsEmpresa.CantidadY = ListaEmpresa[cont2].Cantidad;
                        empresaVsEmpresa.RendimientoY = ListaEmpresa[cont2].Rendimiento;
                        empresaVsEmpresa.MediaRendimientoY = ListaEmpresa[cont2].MediaRendimiento;
                        empresaVsEmpresa.VarianzaRendimientoY = ListaEmpresa[cont2].VarianzaRendimiento;
                        empresaVsEmpresa.DesviacionRendimientoY = ListaEmpresa[cont2].DesviacionRendimiento;
                        empresaVsEmpresa.VarianzaY = ListaEmpresa[cont2].Varianza;
                        empresaVsEmpresa.ListaPrecioY = ListaEmpresa[cont2].ListaPrecio;
                        empresaVsEmpresa.indiceY = cont2;

                        ListaEmpresaVsEmpresa.Add(empresaVsEmpresa);
                    }
                }

                //CALCULAR RENDIMIENTO X POR RENDIMIENTO Y
                //CALCULAR COVARIANZA
                primerRegistro = true;
                foreach (EmpresaVsEmpresa empresaVsEmpresa in ListaEmpresaVsEmpresa)
                {
                    for (cont = 0; empresaVsEmpresa.ListaPrecioX.Count > cont; cont++)
                    {
                        if (primerRegistro)
                        {
                            primerRegistro = false;
                            empresaVsEmpresa.RendimientoXPorY = 0;
                        }
                        else
                        {
                            empresaVsEmpresa.RendimientoXPorY = empresaVsEmpresa.RendimientoXPorY +
                                                                ((empresaVsEmpresa.ListaPrecioX[cont].Rendimiento - empresaVsEmpresa.MediaRendimientoX)
                                                                * (empresaVsEmpresa.ListaPrecioY[cont].Rendimiento - empresaVsEmpresa.MediaRendimientoY));
                        }

                    }

                    empresaVsEmpresa.Covarianza = empresaVsEmpresa.RendimientoXPorY / (empresaVsEmpresa.ListaPrecioX.Count - 1);

                }

                int numEmpresas = ListaEmpresa.Count;
                EmpresaVsEmpresa empresaVsEmpresaCovarianza = new EmpresaVsEmpresa();
                //double covarianza = 0;
                int indiceX = 0;
                int indiceY = 0;

                double[,] matrizVarianzaComp = new double[numEmpresas + 1, numEmpresas + 1];
                //GENERAR MATRIZ VARIANZA COVARIANZA
                for (cont = 0; numEmpresas + 1 > cont; cont++)
                {
                    for (cont2 = 0; numEmpresas + 1 > cont2; cont2++)
                    {
                        if (cont >= numEmpresas || cont2 >= numEmpresas)
                        {
                            if (cont == cont2)
                            {
                                matrizVarianzaComp[cont, cont2] = 0;
                            }
                            else
                            {
                                matrizVarianzaComp[cont, cont2] = 1;
                            }
                        }
                        else
                        {
                            if (cont == cont2)
                            {
                                matrizVarianzaComp[cont, cont2] = ListaEmpresa[cont].VarianzaRendimiento;
                            }
                            else
                            {
                                if (cont <= cont2)
                                {
                                    indiceX = cont;
                                    indiceY = cont2;
                                }
                                else
                                {
                                    indiceX = cont2;
                                    indiceY = cont;
                                }

                                empresaVsEmpresaCovarianza = ListaEmpresaVsEmpresa.Where(n => n.indiceX == indiceX && n.indiceY == indiceY).FirstOrDefault();

                                if (empresaVsEmpresaCovarianza != null)
                                {
                                    matrizVarianzaComp[cont, cont2] = empresaVsEmpresaCovarianza.Covarianza;
                                }

                            }
                        }
                    }
                }

                ////////////////////////
                //2.-PUNTO DE VARIANZA MINIMA GLOBAL
                ////////////////////////

                Matrix matrizInversa = new Matrix(matrizVarianzaComp);

                double[,] matrizInversaFinal = matrizInversa.Inversa();

                //MATRIZ B1 DEL PUNTO
                double[,] B1 = new double[numEmpresas + 1, 1];

                for (cont = 0; numEmpresas + 1 > cont; cont++)
                {
                    if (cont >= numEmpresas)
                    {
                        B1[cont, 0] = 1;
                    }
                    else
                    {
                        B1[cont, 0] = 0;
                    }
                }

                //MATRIZ W
                double[,] matrizW = matrizInversa.ProductoMatrices(matrizInversaFinal, B1);


                ////////////////////////
                //3.-TASA DE RENDIMIENTO DEL PORTAFOLIO
                ////////////////////////

                //CREAR MATRIZ DE 1X3
                double[,] matrizR1x3 = new double[1, ListaEmpresa.Count];
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    matrizR1x3[0, cont] = ListaEmpresa[cont].MediaRendimiento;
                }

                //CRAR MATRIZ DE 3X1
                double[,] matrizR3x1 = new double[ListaEmpresa.Count, 1];
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    matrizR3x1[cont, 0] = ListaEmpresa[cont].MediaRendimiento;
                }

                //MULTIPLICAR MATRICES DE RENDIMIENTOS
                Matrix matrizOperacion = new Matrix();
                double[,] matrizRendiPortafolio = matrizOperacion.ProductoMatrices(matrizR1x3, matrizR3x1);

                double rendimientoPortafolio = matrizRendiPortafolio[0, 0];

                ////////////////////////
                //4.-VARIANZA DEL PORTAFOLIO
                ////////////////////////
                double[,] matrizVarianza = new double[ListaEmpresa.Count, ListaEmpresa.Count];
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    for (cont2 = 0; ListaEmpresa.Count > cont2; cont2++)
                    {
                        matrizVarianza[cont, cont2] = matrizVarianzaComp[cont, cont2];
                    }
                }

                double[,] matrizWS = matrizOperacion.ProductoMatrices(matrizR1x3, matrizVarianza);

                double[,] matrizVarianzaPorta = matrizOperacion.ProductoMatrices(matrizWS, matrizR3x1);

                double varianzaPortafolio = matrizVarianzaPorta[0, 0];

                double varianzaPortaRaiz = Math.Sqrt(varianzaPortafolio);

                ////////////////////////
                //5.-BUSQUEDA DEL CONJUNTO DE PORTAFOLIOS EFICIENTE
                ////////////////////////

                //AGREGAR DATOS VARIANZA
                double[,] matrizV2 = new double[ListaEmpresa.Count + 2, ListaEmpresa.Count + 2];
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    for (cont2 = 0; ListaEmpresa.Count > cont2; cont2++)
                    {
                        matrizV2[cont, cont2] = matrizVarianza[cont, cont2];
                    }
                }

                //AGREGAR INFORMACION DEL RENDIMIENTO DE LA MATRIZ 1x3
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    matrizV2[ListaEmpresa.Count, cont] = matrizR1x3[0, cont];
                }

                //AGREGAR INFORMACION DEL RENDIMIENTO DE LA MATRIZ 3X1
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    matrizV2[cont, ListaEmpresa.Count] = matrizR3x1[cont, 0];
                }

                //RELLENAR CON 1 LA MATRIZ EN EJE X
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    matrizV2[ListaEmpresa.Count + 1, cont] = 1;
                }

                //RELLENAR CON 1 LA MATRIZ EN EJE Y
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    matrizV2[cont, ListaEmpresa.Count + 1] = 1;
                }


                matrizOperacion = new Matrix(matrizV2);

                double[,] matrizV2Inversa = matrizOperacion.Inversa();

                double[,] B2V = new double[ListaEmpresa.Count + 2, 1];

                B2V[ListaEmpresa.Count + 1, 0] = 1;


                ////////////////////////
                //6.-NUMEROS PORTAFLIO QUE FORMARAN PARTE DE LA CURVA DE VARIANZA MINIMA
                ////////////////////////

                List<double> ListaNumeroAleatorio = new List<double>();
                Random random = new Random();

                double a = random.NextDouble();
                a = a / 100;

                for (cont = 0; 20 > cont; cont++)
                {
                    ListaNumeroAleatorio.Add(random.NextDouble() / 1000);
                }

                ListaNumeroAleatorio = ListaNumeroAleatorio.OrderBy(n => n).ToList();

                double[,] W2;
                List<CurvaVarianza> ListaCurvaVarianza = new List<CurvaVarianza>();
                double[,] W2T;
                double[,] oW2T;


                listGrupoParametro = new List<GrupoParametro>();

                int w = 0;
                foreach (double valor in ListaNumeroAleatorio)
                {
                    B2V[ListaEmpresa.Count, 0] = valor;

                    W2 = matrizOperacion.ProductoMatrices(matrizV2Inversa, B2V);

                    double[,] W2Resumen = new double[ListaEmpresa.Count, 1];

                    for (cont = 0; ListaEmpresa.Count > cont; cont++)
                    {
                        W2Resumen[cont, 0] = W2[cont, 0];
                    }

                    matrizOperacion = new Matrix(W2Resumen);
                    oW2T = matrizOperacion.TranspuestaX();
                    W2T = matrizOperacion.EscalarMult(oW2T, 10000);

                    //OBTENER RENDIMIENTO ASUMIDO
                    double[,] matrizRA = matrizOperacion.ProductoMatrices(W2T, matrizR3x1);
                    double rendimientoAsumido = matrizRA[0, 0];

                    //OBTENER SIGMA
                    double[,] matrizSigmaA = matrizOperacion.ProductoMatrices(W2T, matrizVarianza);
                    double[,] matrizSigmaB = matrizOperacion.ProductoMatrices(matrizSigmaA, W2Resumen);

                    double sigma = matrizSigmaB[0, 0];

                    CurvaVarianza curvaVarianza = new CurvaVarianza();
                    curvaVarianza.Numero = w + 1;
                    curvaVarianza.RendimientoAsumido = rendimientoAsumido;
                    curvaVarianza.Sigma = sigma;
                    curvaVarianza.W = W2T;

                    ListaCurvaVarianza.Add(curvaVarianza);


                    //EMPRESAS
                    for (int l = 0; curvaVarianza.W.Length > l; l++)
                    {
                        //AGREGAR INFORMACION DE LOG
                        listParametroDetalle = new List<Parametro>();

                        //AGREGAR DETALLE DE LOG
                        parametro = new Parametro();
                        parametro.Nombre = "Numero";
                        parametro.Valor = w + 1;

                        listParametroDetalle.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "Rendimiento";
                        parametro.Valor = rendimientoAsumido;

                        listParametroDetalle.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "Sigma";
                        parametro.Valor = sigma;

                        listParametroDetalle.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "Usuario";
                        parametro.Valor = ((Entidades.Usuario)Session["Usuario"]).Login;

                        listParametroDetalle.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "Empresa";
                        parametro.Valor = ListaEmpresa[l].Nombre;

                        listParametroDetalle.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "Valor";
                        parametro.Valor = curvaVarianza.W[0, l].ToString("0.0000##");

                        listParametroDetalle.Add(parametro);

                        grupoParametro = new GrupoParametro();
                        grupoParametro.ListGrupoParametro = listParametroDetalle;
                        listGrupoParametro.Add(grupoParametro);
                    }                   


                    w++;
                }

                DataTable dtExportarInformacion = new DataTable();

                Session["dtInformacionCapital"] = dtExportarInformacion;

                if (Session["Usuario"] == null)
                {
                    resultadoMercadoCapital.Mensaje = "Sesion Expirada";

                    return Json(resultadoMercadoCapital, JsonRequestBehavior.AllowGet);
                }


                double minCurvaVarianzaX = 0;

                minCurvaVarianzaX = ListaCurvaVarianza.Min(n => n.Sigma);              

                dtExportarInformacion.Columns.Add("Numero");
                dtExportarInformacion.Columns.Add("Rendimiento_Asumido");
                dtExportarInformacion.Columns.Add("Riesgo");
                for (cont = 0; ListaEmpresa.Count > cont; cont++)
                {
                    dtExportarInformacion.Columns.Add(ListaEmpresa[cont].Nombre);
                }

                cont = 0;
                foreach (CurvaVarianza curVarianza in ListaCurvaVarianza)
                {
                 
                    dtExportarInformacion.Rows.Add(curVarianza.Numero.ToString("0"), curVarianza.RendimientoAsumido.ToString("#,#.00"), curVarianza.Sigma.ToString("#,#.00"));

                    for (cont2 = 0; curVarianza.W.Length > cont2; cont2++)
                    {
                        dtExportarInformacion.Rows[cont][cont2 + 3] = curVarianza.W[0, cont2].ToString("#,#.00");
                    }

                    cont++;
                }


                dtExportarInformacion.Columns.Cast<DataColumn>().ToList().ForEach(n =>
                {
                    encabezadoEmpresa = new EncabezadoEmpresa();
                    encabezadoEmpresa.Columna = n.ColumnName;
                    ListaEncabezadoEmpresa.Add(encabezadoEmpresa);
                });

                int i = 0;
                var ListDatos = dtExportarInformacion.Rows.Cast<DataRow>().ToList().Select(n => new DatosMecadoCapital
                {
                    id = i++,
                    datos = n.ItemArray
                }).ToList();
                 


                resultadoMercadoCapital.ListaEmpresa = ListaEmpresa;
                resultadoMercadoCapital.ListaEncabezadoEmpresa = ListaEncabezadoEmpresa;
                resultadoMercadoCapital.ListaCurvaVarianza = ListaCurvaVarianza;
                resultadoMercadoCapital.ListaCalculoMercadoCapital = ListaCalculoMercadoCapital;
                resultadoMercadoCapital.ListaDatos = ListDatos.ToList();

                

                //INSERTAR INFORMACION LOG
                listParametro = new List<Parametro>();

                parametro = new Parametro();
                parametro.Nombre = "Usuario";
                parametro.Valor = ((Entidades.Usuario)Session["Usuario"]).Login;

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Modulo";
                parametro.Valor = "Mercado Capital";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Empresa";
                parametro.Valor = string.Join(", ", ListaEmpresa.Select(n => n.Nombre).ToArray());

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Resumen";
                parametro.Valor = "Generar Estadistico Mercado Capital";

                listParametro.Add(parametro);

                parametro = new Parametro();
                parametro.Nombre = "Detalle";
                parametro.Valor = "Fecha Inicio: " + strFechaInicio + ", Fecha Final: " + strFechaFinal;

                listParametro.Add(parametro);
                
                //INSERTAR LOG
                Negocio.Log.Log log = new Negocio.Log.Log();
                log.InsertLogCapital(listParametro, listGrupoParametro);

                resultadoMercadoCapital.Mensaje = "OK";

            }
            catch (Exception ex)
            {
                mensaje = "ERROR: Metodo: ObtenerEstadistico_Mercado, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);

                resultadoMercadoCapital.Mensaje = mensaje;
            }

            return Json(resultadoMercadoCapital, JsonRequestBehavior.AllowGet);
        }       
        
        public JsonResult ObtenerSoporteResistencia(List<CatCapital> ListCatCapital)
        {
            SoporteResistencia soporte = new SoporteResistencia();
            ListaY listaY = new ListaY();

            string queryDinamico = string.Empty;
            queryDinamico = "SELECT Fecha,";

            //LISTA EMPRESAS

            //INFORMACION DE LA BASE DE DATOS

            foreach (CatCapital cat in ListCatCapital)
            {
                queryDinamico = queryDinamico + "[" + cat.Valor + "],";
            }
            queryDinamico = queryDinamico.Substring(0, queryDinamico.Length - 1);
            queryDinamico = queryDinamico + " FROM [BD_IdaEconomia_dummy].[idaeconomia].[SOP_RES]";

            MercadoCapital mercado = new MercadoCapital();
            DataTable dtSR = (DataTable)mercado.ObtenerSoporteResistencia(queryDinamico);

            //LLENAR LISTA X CON LAS FECHAS

            if(dtSR.Rows.Count > 0)
            {
                soporte.ListaX = new List<string>();
                foreach(DataRow dr in dtSR.Rows)
                {
                    soporte.ListaX.Add(Convert.ToDateTime(dr["Fecha"].ToString()).ToString("dd/MM/yyyy"));                 
                }

                soporte.ConjuntoListaY = new List<ListaY>();

                foreach (CatCapital cat in ListCatCapital)
                {
                    listaY = new ListaY();
                    listaY.Label = cat.Nombre;

                    listaY.ListaValor = new List<double>();

                    foreach (DataRow dr in dtSR.Rows)
                    {                     
                        listaY.ListaValor.Add(Convert.ToDouble(dr[cat.Valor].ToString()));
                    }

                    soporte.ConjuntoListaY.Add(listaY);
                }
            }        

            return Json(soporte, JsonRequestBehavior.AllowGet);
        }
        public void ExportarExcel()
        {
            string nombreArchivo = "Calculo_De_Portafolio_Eficiente.xlsx";
            string hojaArchivo = "Riesgo_Rendimiento";
            DataTable dtInformacion = new DataTable();
            string mensaje = string.Empty;

            try
            {
                dtInformacion = (DataTable)Session["dtInformacionCapital"];

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
                mensaje = "ERROR: Metodo: ExportarExcel_Mercado, Source: " + ex.Source + ", Mensaje: " + ex.Message;
                ArchivoLog.EscribirLog(null, mensaje);
            }
        }


    }
}