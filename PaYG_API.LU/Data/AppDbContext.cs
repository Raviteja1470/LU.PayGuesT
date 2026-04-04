using Microsoft.EntityFrameworkCore;
using PaYG_API.LU.Auth;
using PayingG.LU.API.Data.PayingGuest.Core.Models;

namespace PayingG.LU.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
        {

        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // USERACCOUNT
            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("UserAccount");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Id)
                      .HasColumnName("GuestId");

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            // REFRESHTOKEN
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken");

                entity.HasKey(t => t.TokenId);

                entity.HasOne(t => t.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(t => t.GuestId)
                      .HasPrincipalKey(u => u.Id)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }



    }
}
