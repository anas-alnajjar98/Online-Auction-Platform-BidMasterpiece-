using Stripe;
using Stripe.Checkout;
using System;
using System.Threading.Tasks;

public class StripePaymentService
{
    public StripePaymentService()
    {
        StripeConfiguration.ApiKey = "sk_test_51Q3FzBRqxwpgnuaX7azGSStPP6UpFrrMOYsg51jX6Tkoj2M4q95UWWxkWvy8DuIdyVcav2EOZxtXf5O5wMhWDxQC003Xx9VDhk"; // Use environment variable ideally.
    }

    public async Task<string> CreateCheckoutSessionAsync(string productName, long amount, string successUrl, string cancelUrl)
    {
        try
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = productName,
                            },
                            UnitAmount = amount,
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Id;
        }
        catch (Exception ex)
        {
            // Log or handle the error
            throw new Exception("Payment failed to process", ex);
        }
    }
}