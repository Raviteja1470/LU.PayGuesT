using PaYG_API.LU.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PayingG.LU.API.Data
{
   

    namespace PayingGuest.Core.Models
    {

        public class UserAccount
        {
            [Key]
            [Column("GuestId")]
            public int Id { get; set; }

            [Required]
            [MaxLength(255)]
            public string Email { get; set; } = default!;

            [Required]
            public string Password { get; set; } = default!;

            [Required]
            public string Role { get; set; } = default!;

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            // Navigation: one user → many refresh tokens
            public ICollection<RefreshToken>? RefreshTokens { get; set; }
        }

        //public class UserAccount
        //{
        //    [Key]
        //    [Column("GuestId")]   // <-- FIX: map to actual DB column
        //    public int Id { get; set; }


        //    [Required]
        //    public string Email { get; set; } = default!;
        //    [Required]
        //    public string Password { get; set; } = default!;
        //    [Required]
        //    public string Role { get; set; } = default!;

        //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        //    //public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();


        //}

    }

}
