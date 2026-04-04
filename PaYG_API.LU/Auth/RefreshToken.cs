using PayingG.LU.API.Data.PayingGuest.Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaYG_API.LU.Auth
{
    public class RefreshToken
    {
        [Key]
        public int TokenId { get; set; }

        [Required]
        public string Token { get; set; } = default!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime Expires { get; set; }

        public bool Enabled { get; set; }

        // Foreign key to UserAccount.GuestId
        public int GuestId { get; set; }

        public UserAccount User { get; set; } = default!;
    }

    //public class RefreshToken
    //{
    //    [Column("GuestId")]
    //    [Key]
    //    public int TokenId { get; set; }
    //    public string Token { get; set; } = default!;
    //    public DateTime CratedDate { get; set; } = DateTime.UtcNow; 

    //    public DateTime Expires { get; set; }
    //    public bool  Enabled { get; set; }
    //    public string ?Email { get; set; }
    //    public UserAccount User { get; set; } = default!;


    //}
}
