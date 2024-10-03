using MasterPiece.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        public BidsController(AuctionDbContext context)
        {
            _context = context;
        }

        [HttpGet("UserWinningBids/{userId:int}")]
        public async Task<IActionResult> UserWinningBids(int userId)
        {
            // Fetching data from OrderHistory for the winning bids
            var winningBids = await _context.OrderHistories
                .Where(oh => oh.UserId == userId)       // Get orders for the user
                .Include(oh => oh.Auction)              // Include related auction
                .ThenInclude(a => a.Product)            // Include related product details
                .Select(oh => new
                {
                    ProductName = oh.Auction.Product.ProductName,
                    Amount = oh.TotalAmount,                 // Use TotalAmount from OrderHistory
                    ProductQuantity = oh.Auction.Product.Quantity,
                    ProductCondition = oh.Auction.Product.Condition,
                    ProductDescription = oh.Auction.Product.Description,
                    ProductImage = oh.Auction.Product.ImageUrl,
                    OrderDate = oh.OrderDate                 // Additional field from OrderHistory
                })
                .ToListAsync();

            return Ok(winningBids);
        }
    }
}
