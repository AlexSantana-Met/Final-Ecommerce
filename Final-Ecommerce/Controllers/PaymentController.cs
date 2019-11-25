using Final_Ecommerce.Models;
using Final_Ecommerce.Repository;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Final_Ecommerce.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        // GET: Pago
        public ActionResult PaymentWithPayPal(string Cancel = null)
        {
            if (Session["usr"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //Se obtiene apiContext
            APIContext apiContext = PayPalConfig.GetAPIContext();
            try
            {
                string PayerId = Request.Params["PayerId"];
                if (string.IsNullOrEmpty(PayerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority
                        + "PaymentWithPayPal/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executePayment = ExecutePayment(apiContext, PayerId, Session[guid] as string);

                    if (executePayment.ToString().ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }

            }
            catch (Exception e)
            {
                return View("FailureView");
            }
            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;

        private object ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            List<Item> itemsPayPal = new List<Item>();
            foreach (CarritoModel item in ((List<CarritoModel>)Session["carrito"]))
            {
                Item i = new Item();
                i.name = item.Nombre.ToString();
                i.currency = "MXN";
                i.price = item.Precio_venta.ToString();
                i.quantity = item.Cantidad.ToString();
                i.sku = "sku";
                itemsPayPal.Add(i);
            }

            List<CarritoModel> carrito = (List<CarritoModel>)Session["carrito"];
            double subtotal = (double)carrito.Sum(i => i.Cantidad * i.Precio_venta);
            //double shipping = subtotal * .15;

            var ItemList = new ItemList() { items = itemsPayPal };

            var payer = new Payer() { payment_method = "paypal" };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = subtotal.ToString()
            };

            var amount = new Amount()
            {
                currency = "MXN",
                total = subtotal.ToString(),
                details = details
            };

            var transactionList = new List<Transaction>();
            transactionList.Add(new Transaction()
            {
                description = "Transaction Description",
                invoice_number = "#100000",
                amount = amount,
                item_list = ItemList
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }

        public ViewResult FailureView()
        {
            return View();
        }

        public ViewResult SuccessView()
        {
            return View();
        }

    }
}