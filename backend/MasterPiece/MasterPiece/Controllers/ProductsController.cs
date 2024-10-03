using MasterPiece.Data;
using MasterPiece.Dtos;
using MasterPiece.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AuctionDbContext _context;

        public ProductsController(AuctionDbContext auctionDbContext)
        {
            _context = auctionDbContext;
        }
        [HttpGet("GetAuctionProductsForHomePage")]
        public async Task<IActionResult> GetAuctionProducts()
        {
            var auctionProducts = await _context.Auctions
                .Include(a => a.Product) 
                .Include(a => a.CurrentHighestBidder)
                  .Where(a => a.Product.ApprovalStatus == "Accepted" &&  a.EndTime > DateTime.Now && a.AuctionStatus == "ongoing")
                .OrderBy(a => Guid.NewGuid()) 
                .Take(4) 
                .Select(a => new {
                    a.AuctionId,
                    a.EndTime,
                    a.CurrentHighestBid,
                    a.Product.ProductName,
                    a.Product.Description,
                    a.Product.ImageUrl,
                    a.Product.StartingPrice,
                    HighestBidderName = a.CurrentHighestBidder != null ? a.CurrentHighestBidder.Username : "No bids yet"
                })
                .ToListAsync();

            return Ok(auctionProducts);
        }
        [HttpGet("GetAuctionProductsForHomePageForLargeCard")]
        public async Task<IActionResult> GetAuctionProductsForHomePageForLargeCard()
        {
            try
            {
                var auctionProduct = await _context.Auctions
                    .Include(a => a.Product)
                    .Include(a => a.CurrentHighestBidder)
                      .Where(a => a.Product.ApprovalStatus == "Accepted" && a.EndTime > DateTime.Now && a.AuctionStatus == "ongoing")
                    .OrderBy(a => Guid.NewGuid()) 
                    .Take(1)
                    .Select(a => new {
                        a.AuctionId,
                        a.EndTime,
                        a.CurrentHighestBid,
                        a.Product.ProductName,
                        a.Product.Description,
                        a.Product.ImageUrl,
                        a.Product.StartingPrice,
                        
                        HighestBidderName = a.CurrentHighestBidder != null ? a.CurrentHighestBidder.Username : "No bids yet"
                    })
                    .FirstOrDefaultAsync(); 

                if (auctionProduct == null)
                {
                    return NotFound("No auction products found.");
                }

                return Ok(auctionProduct);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetProductByAuctionID/{id:int}")]
        public async Task<IActionResult> GetProductByAuctionID(int id)
        {
            var product = await _context.Auctions
                .Include(a => a.Product)
                .Include(a => a.CurrentHighestBidder)
                .Where(a => a.Product.ApprovalStatus == "Accepted" && a.AuctionStatus== "ongoing" && a.AuctionId == id&&a.EndTime>DateTime.Now)
                .Select(a => new
                {
                   
                    a.EndTime,
                    a.CurrentHighestBid,
                    TotalBids = a.Bids.Count(),
                    a.Product.ProductName,
                    a.Product.Description,
                    a.Product.ImageUrl,
                    a.Product.StartingPrice,
                    a.Product.Country,          
                    a.Product.Brand,           
                    a.Product.Quantity,         
                    a.Product.Views,            
                    a.Product.CreatedAt,   
                    HighestBidderName = a.CurrentHighestBidder != null
                        ? a.CurrentHighestBidder.Username
                        : "No bids yet"
                })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound("Product not found or approval status is not accepted.");
            }

            return Ok(product);
        }
        [HttpPost("AuctionBiding")]
        public async Task<IActionResult> AuctionBiding([FromBody] PlaceBids placeBids)
        {

            var auction = await _context.Auctions
        .Include(a => a.Bids)
        .Include(a => a.CurrentHighestBidder)
        .Include(a => a.Product) 
        .SingleOrDefaultAsync(a => a.AuctionId == placeBids.id);


            if (auction == null)
            {
                return NotFound(new { message = "Auction not found" });
            }


            if (placeBids.bidAmount <= auction.CurrentHighestBid || placeBids.bidAmount < auction.Product.StartingPrice)
            {
                return BadRequest(new { message = "Bid must be higher than the current highest bid." });
            }

           
            var newBid = new Bid
            {
                BidAmount = placeBids. bidAmount,
                AuctionId = auction.AuctionId,
                UserId = placeBids.userid, 
                BidTime = DateTime.Now
            };

            
            _context.Bids.Add(newBid);

            
            auction.CurrentHighestBid = placeBids.bidAmount;
            auction.CurrentHighestBidderId = newBid.UserId;

            
            await _context.SaveChangesAsync();

            
            return Ok(new
            {
                message = "Bid placed successfully",
                AuctionId = auction.AuctionId,
                HighestBid = auction.CurrentHighestBid,
                HighestBidderId = auction.CurrentHighestBidderId
            });
        }
        [HttpGet("GetALLCategoryWithTottalProducts")]

        public async Task<IActionResult> GetALLCategoryWithTottalProducts()
        {
            var categories = _context.Categories
        .Where(c => c.Products.Any(p => p.Auctions.Any(a => a.AuctionStatus == "ongoing")))
        .Select(c => new
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            TotalProducts = c.Products.Count(p => p.Auctions.Any(a => a.AuctionStatus == "ongoing")) 
        })
        .ToList();

            return Ok(categories);
        }
            [HttpGet("GetProductsByCategory/{categoryId:int}")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, int pageNumber = 1, int pageSize = 9)
        {
            if (pageNumber <= 0)
            {
                return BadRequest(new { message = "Invalid page number." });
            }
            if (pageSize <= 0)
            {
                return BadRequest(new { message = "Invalid page size." });
            }
            var totalProducts = await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .CountAsync();
            var skip = (pageNumber - 1) * pageSize;

            var products=await _context.Auctions
                .Include(p=>p.Product)
                .Where(p => p.Product.CategoryId == categoryId && p.Product.ApprovalStatus == "Accepted" && p.EndTime>DateTime.Now)
                .Select(p=>new {
                p.AuctionId,
                p.CurrentHighestBid,
                p.EndTime,
                p.StartTime,
                p.ProductId,
                    productDetails = new {
                p.ProductId,
                    p.Product.ProductName,
                    p.Product.Description,
                    p.Product.ImageUrl,
                    p.Product.StartingPrice,
                    p.Product.Category.CategoryName,
            }

        }).Skip(skip) 
                .Take(pageSize)
                .ToListAsync();

            //var products = await _context.Products
            //    .Include(p=>p.Auctions)
            //    .Where(p => p.CategoryId == categoryId&&p.ApprovalStatus== "Accepted")
            //    .Select(p => new
            //    {
            //        p.ProductId,
            //        p.ProductName,
            //        p.Description,
            //        p.ImageUrl,
            //        p.StartingPrice,
            //        p.Category.CategoryName,
            //        AuctionDetails = p.Auctions.Select(a => new
            //        {
            //            a.AuctionId,
            //            a.CurrentHighestBid,
            //            a.EndTime,
            //            a.StartTime
            //        }).FirstOrDefault()
            //    })
            //    .Skip(skip) 
            //    .Take(pageSize)
            //    .ToListAsync();


            if (!products.Any())
            {
                return NotFound(new { message = "No products found under this category." });
            }
            return Ok(new
            {
                TotalProducts = totalProducts,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize),
                Auctions = products
            });
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 9)
        {
           
            var totalAuctions = await _context.Auctions
                .Where(p => p.AuctionStatus == "ongoing" && p.EndTime > DateTime.Now)
                .CountAsync();

           
            var auctions = await _context.Auctions
                .Include(p => p.Product)
                .ThenInclude(p => p.Category)  
                .Where(p => p.AuctionStatus == "ongoing" && p.EndTime > DateTime.Now)  
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.AuctionId,
                    p.CurrentHighestBid,
                    p.EndTime,
                    p.StartTime,

                    productDetails = new
                    {
                        p.ProductId,
                        p.Product.ProductName,
                        p.Product.Description,
                        p.Product.ImageUrl,
                        p.Product.StartingPrice,
                        CategoryName = p.Product.Category.CategoryName
                    }
                })
                .ToListAsync();

           
            var totalPages = (int)Math.Ceiling((double)totalAuctions / pageSize);

           
            return Ok(new
            {
                TotalAuctions = totalAuctions,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                Auctions = auctions
            });
        }

        [HttpPost("PostProduct")]
        public async Task<IActionResult> PostProduct([FromForm] CreateProductDto productDto)
        {
            // Check if the userId is present in the form data
            if (productDto.UserId == 0)
            {
                return BadRequest(new { message = "User ID is required." });
            }

            // Check if the image file is present in the request
            if (productDto.Image == null || productDto.Image.Length == 0)
            {
                return BadRequest(new { message = "Image is required." });
            }

            // Generate a unique file name for the image
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + productDto.Image.FileName;

            // Define the path to save the image in wwwroot/images
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", uniqueFileName);

            // Save the image to the path
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await productDto.Image.CopyToAsync(stream);
            }

            // Map the DTO to the Product model
            var product = new Product
            {
                ProductName = productDto.ProductName,
                Description = productDto.Description,
                StartingPrice = productDto.StartingPrice,
                ImageUrl = "/images/" + uniqueFileName,  // Store the relative path to the image
                Stock = productDto.Stock,
                Condition = productDto.Condition,
                Location = productDto.Location,
                Country = productDto.Country,
                Brand = productDto.Brand,
                Quantity = productDto.Quantity,
                CategoryId = productDto.CategoryId,
                SellerId = productDto.UserId,  // Use the UserId from the form data
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ApprovalStatus = "Pending"  // Default status is pending
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product created successfully", productId = product.ProductId });
        }

    }
}
