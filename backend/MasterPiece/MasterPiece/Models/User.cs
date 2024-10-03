using System;
using System.Collections.Generic;

namespace MasterPiece.Models
{
    public class User
    {
        public int UserId { get; set; }             // Primary Key
        public string Username { get; set; }        // Username of the user
        public string Email { get; set; } // Email of the user
        public string Password { get; set; } 
        public string PasswordHash { get; set; }    // Hashed password for security
        public string PasswordSalt { get; set; }    // Salt used for hashing the password
        public bool IsAdmin { get; set; } = false;  // Indicates if the user is an admin
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // New fields for Address, Gender, and Image
        public string? Address { get; set; }           // User's address
        public string Gender { get; set; }          // Gender of the user (e.g., Male, Female, etc.)
        public string ImageUrl { get; set; }        // URL or path to the user's profile image
        public string? otp { get; set; }// otpfor reste password
        public bool IsDeleted { get; set; } = false; // New soft delete flag

        // Navigation properties
        public ICollection<Blog> Blogs { get; set; }        // User's blogs
        public ICollection<Bid> Bids { get; set; }          // User's bids
        public ICollection<Payment> Payments { get; set; }  // User's payments
    }
}
