using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Entidades;
using Negocio.Usuario;
using PayPal.Api;

namespace IDA_Economia.Controllers
{
    public class PagosController : Controller 
    {
        // GET: Pagos
        public ActionResult Index()
        {
            return View();
        }

        string ClientId = "Aa8En13kfiKnl9pbuMPAx9t6nUGQT3Dh4N6sx7nzaH4JP0-TCgq1QpwcJ7O2mP9ltv6Zg_ysMERpjkdZ" /*"AS_M54Npvyf12oyF0mzgOA8g_TkhsWR0rKlEBDJOwNPykj5IXKhBf870nOSn0vsNS0KmyxkDvOuX8_cM"*/;
        string ClientSecret = "EGvJXMJ7aTVMRiCxL-SdATdRju2hROxgZy6ctuNSjcpVnWyY3RXT4I9vr_hoVUFHndYxHPe4nhWTpDi_" /*"EPJZ4l3AnSsLtX8w0KNZnglqiZpVhSi_KfxGmZaESWhhM0u9Sac9nZxo1vs_fxP17oAk1rzzofShVz7S"*/;
        private Payment payment;

        bool aplicarDescuento = false;

        public ActionResult PagodeMembresia()
        {
            return View();
        }

        // GET: Pagos
        public ActionResult VestibulodePagos()
        {
            //// Agrega credenciales
            //MercadoPago.SDK.AccessToken = "PROD_ACCESS_TOKEN";

            //// Crea un objeto de preferencia
            //Preference preference = new Preference();

            //// Crea un ítem en la preferencia
            //preference.Items.Add(
            //  new MercadoPago.DataStructures.Preference.Item()
            //  {
            //      Title = "Mi producto",
            //      Quantity = 1,
            //      CurrencyId = MercadoPago.Common.CurrencyId.MXN,
            //      UnitPrice = (decimal)75.56
            //  }
            //);
            //preference.Save();

            string payerId = Request.Params["PayerId"];
            var paymentId = Request.Params["paymentId"];

            if (!string.IsNullOrEmpty(payerId))
            {
                Session["MensajePago"] = "";
                Session["TipoPago"] = 0;

                APIContext apiContext = GetAPIContext();

                var executedPayment = ExecutePayment(apiContext, payerId, paymentId);

                if (executedPayment.state.ToLower() != "approved")
                {
                    Session["MensajePago"] = "Error";
                }
                else
                {
                    //MANDAR A LA BASE DE DATOS EN UN A BANDERA QUE TU CREARAS

                    Session["MensajePago"] = "Correcto";
                }

                return RedirectToAction("VestibulodePagos", "Pagos");
            }
            else
            {
                if (Session["MensajePago"] != null)
                {
                    ViewData["MensajePago"] = Session["MensajePago"];
                }
            }

            return View();
        }

        //TIPO DE PAGO UNO
        public ActionResult SendPay()
        {
            string mensaje = string.Empty;
            string descripcionPay = string.Empty;
            string namePay = string.Empty;
            decimal pricePay = 0;
            decimal taxPay = 0;
            decimal totalPay = 0;

            //RedirectToAction("Index", "Home");

            try
            {
                APIContext apiContext = GetAPIContext();

                string payerId = Request.Params["PayerId"];
                var paymentId = Request.Params["paymentId"];

                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Pagos/VestibulodePagos?";

                    var guid = Convert.ToString((new Random()).Next(100000));

                    namePay = "Membresia Mensual";
                    descripcionPay = "Pago Membresia Mensual";
                    pricePay = 100;
                    taxPay = pricePay * Convert.ToDecimal(0.16);
                    totalPay = pricePay + taxPay;

                    Item itemPay = new Item();
                    itemPay.name = namePay;
                    itemPay.quantity = "1";
                    itemPay.currency = "MXN";
                    itemPay.price = pricePay.ToString();
                    itemPay.sku = "PAGOMENSUAL100";

                    Details detailPay = new Details();
                    detailPay.tax = taxPay.ToString();
                    detailPay.shipping = "0";
                    detailPay.subtotal = pricePay.ToString();

                    Amount amountPay = new Amount();
                    amountPay.currency = "MXN";
                    amountPay.total = totalPay.ToString();
                    amountPay.details = detailPay;

                    var createdPayment = CreatePayment(itemPay, detailPay, amountPay, apiContext, descripcionPay, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = string.Empty;

                    while (links.MoveNext())
                    {
                        Links link = links.Current;
                        if (link.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = link.href;
                        }
                    }


                    //VARIABLE DE SESION CON EL TIPO DE PAGO
                    Session["TipoPago"] = 1;

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId);

                    if (executedPayment.state.ToLower() != "approved")
                    {
                        mensaje = "Error";
                    }

                    mensaje = "Correcto";
                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult SendPayReMensual(int rol, int estatusp)
        {
            decimal price = 99;
            decimal factorIVA = Convert.ToDecimal(1.16);
            decimal precioIva = price * factorIVA;
            decimal precioSinIva = price;
            int diasCobro = 30;
            string paypalRedirectUrl = string.Empty;
            int intervalo = 1;

            int IdRol = rol;
            Session["IdRol"] = IdRol;

            int EstatusPago = estatusp;
            Session["EstatusPago"] = EstatusPago;


            try
            {
                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Pagos/VestibulodePagosRe?";
                var guid = Convert.ToString((new Random()).Next(100000));

                var plan = CrearPlanRecurrente("Plan Mensual", "Plan de pago mensual - $" + precioIva.ToString("0.00"), PlanInterval.Month, intervalo, precioIva, precioSinIva, baseURI + "guid=" + guid, aplicarDescuento);
                if (plan != null)
                {
                    var agreement = EjecutarPlanRecurrente(plan, diasCobro);

                    if (agreement != null)
                    {
                        var links = agreement.links.GetEnumerator();

                        while (links.MoveNext())
                        {
                            Links link = links.Current;
                            if (link.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                paypalRedirectUrl = link.href;
                            }
                        }

                        return Redirect(paypalRedirectUrl);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult SendPayReTrimestral(decimal price, int rol, int estatusp)
        {
            decimal factorIVA = Convert.ToDecimal(1.16);
            decimal precioIva = price * factorIVA;
            decimal precioSinIva = price;
            int diasCobro = 30;
            string paypalRedirectUrl = string.Empty;
            int intervalo = 3;

            int IdRol = rol;
            Session["IdRol"] = IdRol;

            int EstatusPago = estatusp;
            Session["EstatusPago"] = EstatusPago;

            try
            {
                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Pagos/VestibulodePagosRe?";
                var guid = Convert.ToString((new Random()).Next(100000));

                var plan = CrearPlanRecurrente("Plan Trimestral", "Plan de pago trimestral - $" + precioIva.ToString("0.00"), PlanInterval.Month, intervalo, precioIva, precioSinIva, baseURI + "guid=" + guid, aplicarDescuento);
                if (plan != null)
                {
                    var agreement = EjecutarPlanRecurrente(plan, diasCobro);

                    if (agreement != null)
                    {
                        var links = agreement.links.GetEnumerator();

                        while (links.MoveNext())
                        {
                            Links link = links.Current;
                            if (link.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                paypalRedirectUrl = link.href;
                            }
                        }

                        return Redirect(paypalRedirectUrl);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult SendPayReAnual(decimal price, int rol, int estatusp)
        {
            decimal factorIVA = Convert.ToDecimal(1.16);
            decimal precioIva = price * factorIVA;
            decimal precioSinIva = price;
            int diasCobro = 30;
            string paypalRedirectUrl = string.Empty;
            int intervalo = 12;

            int IdRol = rol;
            Session["IdRol"] = IdRol;

            int EstatusPago = estatusp;
            Session["EstatusPago"] = EstatusPago;

            try
            {
                string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Pagos/VestibulodePagosRe?";
                var guid = Convert.ToString((new Random()).Next(100000));

                var plan = CrearPlanRecurrente("Plan Trimestral", "Plan de pago trimestral - $" + precioIva.ToString("0.00"), PlanInterval.Month, intervalo, precioIva, precioSinIva, baseURI + "guid=" + guid, aplicarDescuento);
                if (plan != null)
                {
                    var agreement = EjecutarPlanRecurrente(plan, diasCobro);

                    if (agreement != null)
                    {
                        var links = agreement.links.GetEnumerator();

                        while (links.MoveNext())
                        {
                            Links link = links.Current;
                            if (link.rel.ToLower().Trim().Equals("approval_url"))
                            {
                                paypalRedirectUrl = link.href;
                            }
                        }

                        return Redirect(paypalRedirectUrl);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return View();
        }

        public ActionResult VestibulodePagosRe()
        {

            Negocio.Usuario.Usuario Usuario = new Negocio.Usuario.Usuario();
            List<Parametro> ListParametro = new List<Parametro>();
            Parametro parametro = new Parametro();
            Entidades.Usuario usuariopago = new Entidades.Usuario();

            var token = Request.Params["token"];

            if (!string.IsNullOrEmpty(token))
            {
                var executeAgreement = ExecuteBillingAgreement(token);

                if (executeAgreement.state.ToLower() == "active")
                {
                    try
                    {
                        usuariopago = (Entidades.Usuario)Session["Usuario"];
                        int IdRol = (int)Session["IdRol"];
                        int EstatusPago = (int)Session["EstatusPago"];

                        parametro = new Parametro();
                        parametro.Nombre = "Id";
                        parametro.Valor = usuariopago.Id;
                        ListParametro.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "IdRol";
                        parametro.Valor = IdRol;
                        ListParametro.Add(parametro);

                        parametro = new Parametro();
                        parametro.Nombre = "EstatusPago";
                        parametro.Valor = EstatusPago;
                        ListParametro.Add(parametro);

                        usuariopago = Usuario.AgregarPagoUsuario(ListParametro);
                    }
                    catch (Exception ex)
                    {
                    }

                    Session["MensajePago"] = "Pago realizado \"Correctamente\"";
                }
                else
                {
                    Session["MensajePago"] = "Error al realizar el pago, intente nuevamente.";
                }

                return RedirectToAction("VestibulodePagosRe", "Pagos");
            }
            else
            {
                if (Session["MensajePago"] != null)
                {
                    ViewData["MensajePago"] = Session["MensajePago"];
                }
            }

            return View();
        }

        private string GetAccessToken()
        {
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }

        public Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }

        public APIContext GetAPIContext()
        {
            var apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();

            return apiContext;
        }
        //CONFIGURACION PAGO
        private Payment CreatePayment(Item itemPay, Details detailPay, Amount amountPay, APIContext apiContext, string tipoPago, string redirectUrl)
        {
            var item = new Item()
            {
                name = itemPay.name,
                currency = itemPay.currency,
                price = itemPay.price,
                quantity = itemPay.quantity,
                sku = itemPay.sku
            };

            var itemList = new ItemList();
            itemList.items = new List<Item>();
            itemList.items.Add(item);

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl, //"http://localhost:1187/", //"http://localhost:3000/cancel",
                return_url = redirectUrl //"http://localhost:1187/" //"http://localhost:3000/process"
            };

            var details = new Details()
            {
                tax = detailPay.tax,
                shipping = detailPay.shipping,
                subtotal = detailPay.subtotal,

            };

            var amount = new Amount()
            {
                currency = amountPay.currency,
                total = amountPay.total,
                details = details
            };

            var transaction = new List<Transaction>();
            transaction.Add(new Transaction()
            {
                description = tipoPago,
                invoice_number = Convert.ToString((new Random()).Next(100000)),
                amount = amount,
                item_list = itemList
            });

            var payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transaction,
                redirect_urls = redirUrls
            };

            return payment.Create(apiContext);
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            //EXECUTE PAYMENT
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            payment = new Payment()
            {
                id = paymentId
            };

            return payment.Execute(apiContext, paymentExecution);
        }

        //CONFIGURACION PLAN RECURRENTE
        private Plan CrearPlanRecurrente(string name, string description, string recurrencia, int intervalo, decimal precioIva, decimal precioSinIva, string urlPay, bool aplicaDescuento)
        {
            decimal porcentajeTotal = 100;
            decimal porcentajeDescuento = 10; //deberia de ser tomado del web.config
            decimal porcentajeDiferencia = porcentajeTotal - porcentajeDescuento;

            try
            {
                if (aplicaDescuento)
                {
                }

                var plan = new Plan
                {
                    name = name,
                    description = description,
                    type = "fixed",

                    payment_definitions = new List<PaymentDefinition>
                    {
                        new PaymentDefinition
                        {
                          name = "Standard Plan",
                          type = "REGULAR",
                          frequency = recurrencia.ToUpper(),
                          frequency_interval = intervalo.ToString(),
                          amount = GetCurrency(Convert.ToDecimal(precioIva).ToString("0.##")),
                          cycles = "1",
                          charge_models = new List<ChargeModel>
                          {
                            new ChargeModel
                            {
                              type = "TAX",
                              amount = GetCurrency(Convert.ToDecimal(precioIva - precioSinIva).ToString("0.##"))
                            }
                          }
                        }
                      },

                    merchant_preferences = new MerchantPreferences()
                    {
                        setup_fee = GetCurrency("0"),
                        return_url = urlPay,
                        cancel_url = urlPay + "?cancel",
                        auto_bill_amount = "YES",
                        initial_fail_amount_action = "CONTINUE",
                        max_fail_attempts = "0"
                    }
                };

                // PayPal Authentication tokens
                var apiContext = GetAPIContext();

                // Create plan
                plan = plan.Create(apiContext);

                // Activate the plan
                var patchRequest = new PatchRequest()
                {
                    new Patch()
                    {
                        op = "replace",
                        path = "/",
                        value = new Plan() { state = "ACTIVE" }
                    }
                };

                plan.Update(apiContext, patchRequest);

                return plan;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private Agreement EjecutarPlanRecurrente(Plan plan, int diasCobro)
        {
            try
            {
                DateTime startDate = DateTime.Now.AddDays(diasCobro);
                DateTime endDate = startDate.AddMonths(1);

                APIContext apiContext = GetAPIContext();

                var shippingAddress = new ShippingAddress()
                {
                    line1 = "AVENIDA JUAREZ 1123, COL. UNIVERSIDAD",
                    city = "TOLUCA",
                    state = "ESTADO DE MEXICO",
                    postal_code = "95070",
                    country_code = "MX"
                };

                var agreement = new Agreement()
                {
                    name = plan.name,
                    description = plan.description,
                    start_date = startDate.ToString("yyyy-MM-ddTHH:mm:ss") + "Z",
                    //update_time = endDate.ToString("yyyy-MM-ddTHH:mm:ss") + "Z",
                    payer = new Payer() { payment_method = "paypal" },
                    plan = new Plan() { id = plan.id },
                    shipping_address = shippingAddress
                };

                var createdAgreement = agreement.Create(apiContext);

                return createdAgreement;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private class PlanInterval
        {
            public static string Week { get { return "WEEK"; } }
            public static string Day { get { return "DAY"; } }
            public static string Month { get { return "MONTH"; } }
            public static string Year { get { return "YEAR"; } }
        }
        private Currency GetCurrency(string value)
        {
            return new Currency() { value = value, currency = "MXN" };
        }
        public Agreement ExecuteBillingAgreement(string token)
        {
            // PayPal Authentication tokens
            var apiContext = GetAPIContext();

            var agreement = new Agreement() { token = token };
            var executedAgreement = agreement.Execute(apiContext);

            return executedAgreement;
        }

    }
}