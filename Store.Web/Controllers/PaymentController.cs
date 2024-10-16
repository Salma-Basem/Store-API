using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Store.Service.Services.BasketService.Dtos;
using Store.Service.Services.PaymentService;
using Stripe;
using Stripe.V2;

namespace Store.Web.Controllers
{
  
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        const string endpointSecret = "whsec_bf63bbdb352228f880e3312850a785e7cf0a5969648063fdf6cdfb3ab6c93ba7";
        public PaymentController(IPaymentService paymentService,ILogger<PaymentController> logger)
        {
           _paymentService = paymentService;
           _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(CustomerBasketDto input)
            => Ok(await _paymentService.CreateOrUpdatePaymentIntent(input));

        [HttpPost]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                        Request.Headers["Stripe-Signature"],endpointSecret);

                PaymentIntent paymentIntent;
                if (stripeEvent.Type == "Payment_intent.payment_failed")
                { 
                    paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation("Payment Failed",paymentIntent.Id  );
                   var order = await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                    _logger.LogInformation("Order Updated To Payment Failed: ", order.Id);
                }
                else if (stripeEvent.Type == "Payment_intent.succeeded")
                {
                    paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    _logger.LogInformation("Payment Succeeded", paymentIntent.Id);
                    var order = await _paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                    _logger.LogInformation("Order Updated To Payment Succeeded: ", order.Id);
 
                }
                else if (stripeEvent.Type == "Payment_intent.created")
                {
                   _logger.LogInformation("Payment created");

                }

                else
                {
                    Console.WriteLine("Unhandled event type:{0}", stripeEvent.Type);

                }
                return Ok();
            }
            catch(StripeException e)
            {
                return BadRequest();
            }

        }
    }
}
