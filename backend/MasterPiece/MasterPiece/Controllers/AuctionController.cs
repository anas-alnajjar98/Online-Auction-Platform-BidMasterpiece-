using Hangfire;
using MasterPiece.Data;
using MasterPiece.Dtos;
using MasterPiece.Helper;
using MasterPiece.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly EmailHelper _emailHelper;

        public AuctionController(AuctionDbContext context, EmailHelper emailHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
        }

        [HttpPost("CreateAuction")]
        public async Task<IActionResult> CreateAuction(CreateAuctionDto auctionDto)
        {
           
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == auctionDto.ProductId);

            
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            if (product.ApprovalStatus != "Pending")
            {
                return BadRequest(new { message = "Product is not in a pending state." });
            }

            
            var auction = new Auction
            {
                ProductId = auctionDto.ProductId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(auctionDto.DurationHours), 
                CurrentHighestBid = product.StartingPrice,
                AuctionStatus = "ongoing"
            };

            
            _context.Auctions.Add(auction);

          
            product.ApprovalStatus = "Accepted";
            _context.Products.Update(product);

            await _context.SaveChangesAsync();

            BackgroundJob.Schedule(() => EndAuction(auction.AuctionId), auction.EndTime);

            return Ok(new { message = "Auction created and product approved", auctionId = auction.AuctionId });
        }


        [AutomaticRetry(Attempts = 1)]
        [HttpPost("EndAuction/{auctionId:int}")]
        public async Task EndAuction(int auctionId)
        {
            var auction = await _context.Auctions
                .Include(a => a.Bids)
                .Include(a => a.Product)
                .FirstOrDefaultAsync(a => a.AuctionId == auctionId);

            if (auction == null)
                return;

            var highestBid = auction?.Bids?.OrderByDescending(b => b.BidAmount).FirstOrDefault();
            if (highestBid == null)
            {
                auction.AuctionStatus = "failed";
                await _context.SaveChangesAsync();
                return;
            }

            auction.CurrentHighestBidderId = highestBid.UserId;
            auction.CurrentHighestBid = highestBid.BidAmount;
            auction.AuctionStatus = "pending_payment";
            auction.EndTime = DateTime.Now;

            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            // Create and save the payment first to generate the paymentId
            var payment = new Payment
            {
                AuctionId = auction.AuctionId,
                UserId = highestBid.UserId,
                PaymentAmount = highestBid.BidAmount,
                PaymentStatus = "pending",
                PaymentDate = DateTime.Now,
                PaymentDueDate = DateTime.Now.AddHours(24)
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // After saving, retrieve the paymentId
            var paymentId = payment.PaymentId;  // Retrieve the auto-generated paymentId

            // Generate payment link with paymentId
            var paymentLink = $"http://127.0.0.1:5501/payment.html?auctionId={paymentId}";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == highestBid.UserId);
            if (user != null)
            {
                try
                {
                    _emailHelper.SendMessage(
                        user.Username,
                        user.Email,
                        "Auction Won: Payment Required",
                        $"You have won the auction for {auction.Product.ProductName}. Please complete your payment within 24 hours by clicking the following link: {paymentLink}"
                    );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }

        [HttpPost("CheckPendingPayments")]
        public async Task<IActionResult> CheckPendingPayments()
        {
            
            var pendingPayments = await _context.Payments
                .Include(p => p.Auction)
                .ThenInclude(a => a.Product)
                .Where(p => p.PaymentStatus == "pending" && DateTime.Now < p.PaymentDueDate)
                .ToListAsync();

            foreach (var payment in pendingPayments)
            {
                // Reschedule auction if payment is overdue
                var auction = payment.Auction;

                if (auction != null)
                {
                    // Notify the user that the auction is rescheduled before resetting the highest bidder
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == auction.CurrentHighestBidderId);

                    if (user != null)
                    {
                        _emailHelper.SendMessage(
                            user.Username,
                            user.Email,
                            "Auction Rescheduled",
                            $"The auction for {auction.Product.ProductName} has been rescheduled due to non-payment. " +
                            $"The auction will now end on {auction.EndTime.AddDays(1)}."
                        );
                    }

                    
                    auction.CurrentHighestBid = 0;
                    auction.CurrentHighestBidderId = null;
                    auction.AuctionStatus = "rescheduled";
                    auction.EndTime = DateTime.Now.AddDays(1); 

                    _context.Auctions.Update(auction);

                    
                    payment.PaymentStatus = "expired";
                    _context.Payments.Update(payment);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Pending payments checked and necessary actions taken.");
        }
    }
}
