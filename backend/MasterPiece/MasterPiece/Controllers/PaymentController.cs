using MasterPiece.Data;
using MasterPiece.Dtos;
using MasterPiece.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AuctionDbContext _db;

        public PaymentController(AuctionDbContext db)
        {
            _db = db;
        }

        [HttpPost("CreatePaymentIntent")]
        public IActionResult CreatePaymentIntent([FromBody] PaymentRequest request)
        {
            try
            {
                if (request == null || request.Amount <= 0 || request.AuctionId <= 0)
                {
                    return BadRequest(new { message = "Invalid payment request." });
                }

                // Initialize Stripe API
                StripeConfiguration.ApiKey = "sk_test_51Q3FzBRqxwpgnuaX7azGSStPP6UpFrrMOYsg51jX6Tkoj2M4q95UWWxkWvy8DuIdyVcav2EOZxtXf5O5wMhWDxQC003Xx9VDhk";

                // Create payment intent options
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(request.Amount * 100), // Convert to cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "auctionId", request.AuctionId.ToString() }
                    }
                };

                // Create the payment intent using Stripe
                var service = new PaymentIntentService();
                var paymentIntent = service.Create(options);

                return Ok(new { clientSecret = paymentIntent.ClientSecret });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating payment intent.", details = ex.Message });
            }
        }

        [HttpGet("GetAuctionDetailsByPayment/{paymentId}")]
        public async Task<IActionResult> GetAuctionDetailsByPayment(int paymentId)
        {
            try
            {
                
                var payment = await _db.Payments
                    .Include(p => p.Auction)
                    .ThenInclude(a => a.Product)
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found." });
                }

               
                var auction = payment.Auction;

                return Ok(new
                {
                    auction.AuctionId,
                    productName = auction.Product.ProductName,
                    currentHighestBid = auction.CurrentHighestBid,
                    totalAmount = auction.CurrentHighestBid
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving auction details by payment.", details = ex.Message });
            }
        }


        [HttpPut("UpdatePaymentStatus/{paymentId}")]
        public async Task<IActionResult> UpdatePaymentStatus(int paymentId, [FromBody] PaymentStatusUpdateRequest request)
        {
            try
            {
               
                if (string.IsNullOrWhiteSpace(request.Status))
                {
                    return BadRequest(new { message = "Payment status is required." });
                }

                
                var payment = await _db.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                
                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found." });
                }

               
                payment.PaymentStatus = request.Status;
                await _db.SaveChangesAsync();

                return Ok(new { message = "Payment status updated successfully." });
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "Error updating payment status.", details = ex.Message });
            }
        }
        [HttpPost("CreateOrderHistory/{paymentId}")]
        public async Task<IActionResult> CreateOrderHistory(int paymentId)
        {
            var payment = await _db.Payments.Where(x => x.PaymentId == paymentId).FirstOrDefaultAsync();

            if (payment == null)
            {
                return NotFound("No payment record found for this payment ID.");
            }

            
            var orderHistory = new OrderHistory
            {
                AuctionId = payment.AuctionId,  
                UserId = payment.UserId,        
                TotalAmount = payment.PaymentAmount, 
                OrderDate = DateTime.Now    
            };

            
            _db.OrderHistories.Add(orderHistory);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Order history created successfully." });
        }
        [HttpGet("GetThankYouDetailsByPayment/{paymentId}")]
        public async Task<IActionResult> GetThankYouDetailsByPayment(int paymentId)
        {
            try
            {
                
                var payment = await _db.Payments
                    .Include(p => p.Auction)
                        .ThenInclude(a => a.Product)
                    .Include(p => p.User) 
                    .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

                if (payment == null)
                {
                    return NotFound(new { message = "Payment not found." });
                }

                var response = new
                {
                    ProductName = payment.Auction.Product.ProductName,
                    img= payment.Auction.Product.ImageUrl,
                    DeliveryDate = DateTime.UtcNow.AddDays(3).ToString("dd MMM yyyy"), 
                    DeliveryAddress = payment.User.Address,
                    ImageUrl = payment.Auction.Product.ImageUrl 
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving payment details.", details = ex.Message });
            }
        }



    }
}
