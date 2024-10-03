using MasterPiece.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AuctionDbContext _context;

        public HomeController(AuctionDbContext auctionDbContext) {
            _context = auctionDbContext;
        }
        /// <summary>
        /// //
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("GetAllBlogs")]
        public async Task<IActionResult> GetAllBlogs(int pageNumber = 1, int pageSize = 6)
        
        {
            try
            {
                var totalBlogs = await _context.Blogs.CountAsync();
                var totalPages = (int)Math.Ceiling(totalBlogs / (double)pageSize);

                var blogs = await _context.Blogs
                    .Include(b => b.Author)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(blog => new
                    {
                        blog.BlogId,
                        blog.Title,
                        blog.Content,
                        blog.PublishedAt,
                        blog.ViewCount,
                        blog.ImageUrl,
                        blog.ApprovalStatus,
                        AuthorName = blog.Author.Username,
                        AuthorAvatar = blog.Author.ImageUrl
                    })
                    .ToListAsync();

                return Ok(new
                {
                    blogs,
                    totalPages
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetBlogById/{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            try
            {
                var blog = await _context.Blogs
                    .Include(b => b.Author)
                    .Where(b => b.BlogId == id)
                    .Select(blog => new
                    {
                        blog.BlogId,
                        blog.Title,
                        blog.Content,
                        blog.PublishedAt,
                        blog.ViewCount,
                        blog.ImageUrl,
                        AuthorName = blog.Author.Username,
                        AuthorAvatar = blog.Author.ImageUrl
                    })
                    .FirstOrDefaultAsync();

                if (blog == null)
                {
                    return NotFound("Blog not found.");
                }

                return Ok(blog);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
