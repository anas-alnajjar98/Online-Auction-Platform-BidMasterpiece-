using MasterPiece.Data;
using MasterPiece.Helpers;
using MasterPiece.Models.DTOs;
using MasterPiece.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MasterPiece.Dtos;
using MimeKit;
using System.Data;
using MasterPiece.Helper;

namespace MasterPiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EmailHelper _emailHelper;
        private readonly AuctionDbContext _context;
        private readonly TokenGenerator _tokenGenerator;

        public UserController(AuctionDbContext auctionDbContext, TokenGenerator tokenGenerator, EmailHelper emailHelper)
        {
            _context = auctionDbContext;
            _tokenGenerator = tokenGenerator;
            _emailHelper = emailHelper;
        }
        [HttpGet("GetUserByID/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
          
            var user = await _context.Users
                .Where(u => u.UserId == id)
                .Select(a => new
                {
                    ImageUrl = a.ImageUrl,
                    Username = a.Username,
                    Email = a.Email,
                    UserId = a.UserId
                })
                .FirstOrDefaultAsync();

            
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

           
            return Ok(user);
        }


        /// <summary>
        /// ////////
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("User with this email already exists.");
            }


            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Gender = "",
                ImageUrl = "default.png",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };


            PasswordHelper.CreatePasswordHash(request.Password, out string passwordHash, out string passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Password = request.Password;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
        /// <summary>
        /// /////////////
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email&&u.IsDeleted==false);

            if (user == null)
            {

                return NotFound("User not found.");
            }
            if (!PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid password.");
            }
            var token = _tokenGenerator.GenerateToken(user.Username);
            return Ok(new
            {
                user.UserId,
                user.Username,
                user.Email,
                user.IsAdmin,
                token
            });
        }
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == forgotPasswordDto.Email);
            if (user != null)
            {
                // Store user ID and OTP in session
                //HttpContext.Session.SetInt32("UserID", user.UserId);

                // Generate a random OTP
                Random rand = new Random();
                string otp = rand.Next(100000, 1000000).ToString();
                //HttpContext.Session.SetString("otp", otp);

                // Prepare email content
                string fromEmail = "techlearnhub.contact@gmail.com";
                string fromName = "Support Team";
                string subjectText = "Your OTP Code";
                string messageText = $@"
<html>
<body dir='rtl'>
    <h2>Hello {user.Username}</h2>
    <p><strong>Your OTP code is {otp}. This code is valid for a short period of time.</strong></p>
    <p>If you have any questions or need additional assistance, please feel free to contact our support team.</p>
    <p>Best wishes,<br>Support Team</p>
</body>
</html>";

                try
                {
                    // Send email using MailKit
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(fromName, fromEmail));
                    message.To.Add(new MailboxAddress("", user.Email));
                    message.Subject = subjectText;
                    message.Body = new TextPart("html") { Text = messageText };

                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        await client.ConnectAsync("smtp.gmail.com", 465, true);
                        await client.AuthenticateAsync("techlearnhub.contact@gmail.com", "lyrlogeztsxclank");
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }
                    user.otp = otp;

                    _context.Update(user);
                    _context.SaveChanges();
                    return Ok(new { otp, user.UserId });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Message = "Failed to send email. Please try again later.", Error = ex.Message });
                }
            }
            else
            {
                return NotFound(new { Message = "Email not found." });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resset"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] RessetPasswordDto resset ) { 
            var user= await _context.Users.Where(p=>p.otp==resset.Otp).FirstAsync();
            if (user != null) {
              


                PasswordHelper.CreatePasswordHash(resset.Password, out string passwordHash, out string passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Password = resset.Password;
                user.UpdatedAt = DateTime.Now;
                user.otp=null;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Ok();

            }
            return BadRequest("Otp not match");
        
        }
        /// <summary>
        /// //////
        /// </summary>
        /// <param name="googleSignUpDto"></param>
        /// <returns></returns>
        [HttpPost("GoogleSignUp")]
        public async Task<IActionResult> GoogleSignUp([FromBody] GoogleSignUpDto googleSignUpDto)
        {
            if (googleSignUpDto == null || string.IsNullOrEmpty(googleSignUpDto.Email))
            {
                return BadRequest("Invalid user data.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == googleSignUpDto.Email))
            {
                return BadRequest("User with this email already exists.");
            }

            var user = new User
            {
                Username = googleSignUpDto.Username ?? "Unknown",
                Email = googleSignUpDto.Email,
                Gender = "", 
                ImageUrl = googleSignUpDto.ImageUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            
            PasswordHelper.CreatePasswordHash(googleSignUpDto.Password, out string passwordHash, out string passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Password = googleSignUpDto.Password;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            
            var token = _tokenGenerator.GenerateToken(user.Username);  

            
            return Ok(new
            {
                userId = user.UserId,
                token = token,
                message = "User registered successfully."
            });
        }
        [HttpPost("Contact")]
        public async Task<IActionResult> Contact(ContactDto contact)
        {
            
            var user = await _context.Users.FindAsync(contact.UserId);
            if (user == null)
            {
                return BadRequest("You need to be logged in before submitting any comment.");
            }

            
            if (string.IsNullOrWhiteSpace(contact.Name) || string.IsNullOrWhiteSpace(contact.Email) || string.IsNullOrWhiteSpace(contact.Message))
            {
                return BadRequest("Name, Email, and Message are required fields.");
            }

           
            var newContact = new Contact
            {
                UserId = contact.UserId,
                Name = contact.Name,
                Subject = contact.Subject,
                Email = contact.Email,
                SubmittedAt = DateTime.Now,
                Message = contact.Message,
            };

           
            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();

            try
            {
               
                _emailHelper.SendMessage(
                    contact.Name,
                    contact.Email,
                    contact.Subject ?? "New Contact Message",
                    contact.Message
                );
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, new { message = "Error occurred while sending the email.", error = ex.Message });
            }

           
            return Ok(new { message = "Your contact message was submitted successfully!", contact = newContact });
        }

        [HttpGet("GetUserDashboard")]
        public async Task<IActionResult> GetUserDashboard(int userId)
        {
            var userBids = await _context.Bids
                .Where(bid => bid.UserId == userId)
                .Select(bid => new {
                    ItemName = bid.Auction.Product.ProductName,  
                    LastBid = bid.BidAmount,  
                    OpeningBid = bid.Auction.Product.StartingPrice, 
                    EndTime = bid.Auction.EndTime,  
                    ItemId = bid.Auction.ProductId   
                })
                .ToListAsync();

           
            var activeBids = userBids.Count();
            var winningBids = await _context.Auctions
                .Where(a => a.CurrentHighestBidderId == userId)
                .CountAsync();
            var favorites = await _context.Favorites
                .Where(f => f.UserId == userId)
                .CountAsync();

            return Ok(new
            {
                ActiveBids = activeBids,          
                WinningBids = winningBids,        
                Favorites = favorites,            
                BidHistory = userBids             
            });
        }
        [HttpPost("UpdateUserInfoWithImage/{id:int}")]
        public async Task<IActionResult> UpdateUserInfoWithImage(int id, [FromForm] EditUserInfoDto edit)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            if (edit.Image != null && edit.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                var externalFolder = @"C:\Users\Orange\Desktop\masterpiece\assets\images";

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!Directory.Exists(externalFolder))
                {
                    Directory.CreateDirectory(externalFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + edit.Image.FileName;

                var filePathWwwroot = Path.Combine(uploadsFolder, uniqueFileName);
                var filePathExternal = Path.Combine(externalFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePathWwwroot, FileMode.Create))
                {
                    await edit.Image.CopyToAsync(fileStream);
                }

                using (var fileStream = new FileStream(filePathExternal, FileMode.Create))
                {
                    await edit.Image.CopyToAsync(fileStream);
                }

                user.ImageUrl = $"/images/{uniqueFileName}";
            }

            user.Username = edit.Username ?? user.Username;
            user.Address = edit.Address ?? user.Address;
            user.Gender = edit.Gender ?? user.Gender;

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "User info updated successfully", ImageUrl = user.ImageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating user info", Error = ex.Message });
            }
        }

        [HttpPost("EditUserResetPassword")]
        public async Task<IActionResult> EditUserResetPassword([FromBody] EditUserResetPasswordDto resset)
        {
            var user = await _context.Users.Where(p => p.UserId == resset.UserID&&p.Password==resset.Password).FirstAsync();
            if (user != null)
            {
                PasswordHelper.CreatePasswordHash(resset.ConfirmPassword, out string passwordHash, out string passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Password = resset.ConfirmPassword;
                user.UpdatedAt = DateTime.Now;
                user.otp = null;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return Ok();

            }
            return BadRequest("password not match");

        }


    }
}