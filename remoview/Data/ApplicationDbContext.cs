using Microsoft.EntityFrameworkCore;
using remoview.Models; // Models klasörümüzü tanıttık


namespace remoview.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Veritabanında bu isimlerle tablolar oluşturulacak
        public DbSet<User> Users { get; set; }
        public DbSet<Film> Films { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Genre> Genres { get; set; } // <-- YENİ SATIR
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Friendship>()
                .HasOne(friendship => friendship.Requester)
                .WithMany(user => user.SentFriendRequests)
                .HasForeignKey(friendship => friendship.RequesterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friendship>()
                .HasOne(friendship => friendship.Addressee)
                .WithMany(user => user.ReceivedFriendRequests)
                .HasForeignKey(friendship => friendship.AddresseeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Friendship>()
                .HasIndex(friendship => new { friendship.RequesterId, friendship.AddresseeId })
                .IsUnique();
        }
    }
}
