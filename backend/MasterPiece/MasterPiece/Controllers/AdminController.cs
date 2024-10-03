using MasterPiece.Data;
using MasterPiece.Dtos;
using MasterPiece.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly EmailHelper _emailHelper;

        public AdminController(AuctionDbContext context, EmailHelper emailHelper)
        {
            _context = context;
            _emailHelper = emailHelper;
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            
            var users = await _context.Users
                .Include(u => u.Blogs) 
                .Include(u => u.Bids)   
                .Include(u => u.Payments)
                .Where(u=>u.IsDeleted == false)
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound(new { message = "No users found." });
            }

            var userResponse = new List<object>();
            foreach (var user in users)
            {
                userResponse.Add(new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.Address,
                    user.Gender,
                    user.ImageUrl,
                    user.IsAdmin,
                    user.CreatedAt,
                    user.UpdatedAt,
                    Blogs = user.Blogs.Select(b => new
                    {
                        b.BlogId,
                        b.Title,
                        b.Content,
                       b.PublishedAt
                    }),
                    Bids = user.Bids.Select(b => new
                    {
                        b.BidId,
                        b.AuctionId,
                        b.BidAmount,
                        b.BidTime
                    }),
                    Payments = user.Payments.Select(p => new
                    {
                        p.PaymentId,
                        p.PaymentAmount,
                        p.PaymentStatus,
                        p.PaymentDate,
                        p.PaymentDueDate
                    })
                });
            }

            return Ok(userResponse);
        }

        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.IsDeleted = true; 
            await _context.SaveChangesAsync();

            return Ok(new { message = "User marked as deleted." });
        }

        [HttpGet("GetAllProductTopalceAuction")]
        public async Task<IActionResult> GetAllProductTopalceAuction()
        {
            try
            {
               
                var products = await _context.Products
                    .Where(a => a.ApprovalStatus == "Pending")
                    .Select(p => new
                    {
                        p.ProductId,
                        p.ProductName,
                        p.Description,
                        p.StartingPrice,
                        p.ImageUrl,
                        p.Stock,
                        p.Condition,
                        p.Location,
                        p.Country,
                        p.Brand,
                        p.Quantity,
                        Category = p.Category.CategoryName 
                    })
                    .ToListAsync();

                
                return Ok( products);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "An error occurred while fetching the products", details = ex.Message });
            }
        }
        [HttpPut("RejectProduct/{productId}")]
        public async Task<IActionResult> RejectProduct(int productId)
        {
            try
            {
                
                var product = await _context.Products.FindAsync(productId);

               
                if (product == null)
                {
                    return NotFound(new { message = "Product not found." });
                }

                
                product.ApprovalStatus = "Rejected";
                product.UpdatedAt = DateTime.Now;

                
                await _context.SaveChangesAsync();

                return Ok(new { message = "Product rejected successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error rejecting product.", details = ex.Message });
            }
        }
        [HttpGet("EndedAuction")]
        public async Task<IActionResult> EndedAuction()
        {
            var endedAuctions = await _context.Auctions
                .Where(x => x.EndTime < DateTime.Now && x.IsNotificationSent == false && x.AuctionStatus == "pending_payment")
                .Include(x => x.Product) // Include product details
                .Include(x => x.CurrentHighestBidder) // Include highest bidder details
                .Select(x => new
                {
                    ProductName = x.Product.ProductName ?? "Unknown Product", // Handle null product name
                    AuctionId= x.AuctionId,
                    CurrentHighestBid = x.CurrentHighestBid, // No need for ?? because it's a decimal and cannot be null
                    HighestBidder = x.CurrentHighestBidder != null ? new
                    {
                        x.CurrentHighestBidder.UserId,
                        x.CurrentHighestBidder.Username,
                        x.CurrentHighestBidder.Email,

                       
                    } : null // Handle case where there is no highest bidder
                })
                .ToListAsync();

            if (endedAuctions == null || !endedAuctions.Any())
            {
                return NotFound(new { message = "No auctions have ended." });
            }

            return Ok(new
            {
                message = "Auctions fetched successfully.",
                endedAuctions
            });
        }






    }
}
