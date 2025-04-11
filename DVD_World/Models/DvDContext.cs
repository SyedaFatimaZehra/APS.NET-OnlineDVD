using DVD_World.Models;
using Microsoft.EntityFrameworkCore;

public class DvdContext : DbContext
{
    public DvdContext(DbContextOptions options) : base(options) { }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Song> Songs { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Producer> Producers { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Advertisement> Advertisements { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Promotion> Promotions { get; set; }

    public DbSet<MovieTrailer> MovieTrailers { get; set; }
    public DbSet<GameTrailer> GameTrailers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; } 

    public DbSet<PurchasingInvoice> PurchasingInvoices { get; set; }
    public DbSet<Contact> Contacts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Artist>()
              .HasMany(a => a.Songs)   
              .WithOne(s => s.Artist)
              .HasForeignKey(s => s.ArtistId)
              .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<Song>()
            .HasOne(s => s.Album)
            .WithMany()
            .HasForeignKey(s => s.AlbumId)
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Song>()
            .HasOne(s => s.Artist)
            .WithMany()
            .HasForeignKey(s => s.ArtistId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Album>()
            .HasMany(a => a.Songs)    // ✅ `HasMany` (Album has many Songs)
            .WithOne(s => s.Album)    // ✅ `WithOne` (Each Song belongs to one Album)
            .HasForeignKey(s => s.AlbumId)  // ✅ Foreign Key in Song table
            .OnDelete(DeleteBehavior.Cascade);



        modelBuilder.Entity<Feedback>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<MovieTrailer>()
            .HasOne(mt => mt.Movie)
            .WithMany()
            .HasForeignKey(mt => mt.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GameTrailer>()
            .HasOne(gt => gt.Game)
            .WithMany()
            .HasForeignKey(gt => gt.GameId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Product>()
           .HasOne(p => p.Album)
           .WithMany()
           .HasForeignKey(p => p.AlbumId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Movie)
            .WithMany()
            .HasForeignKey(p => p.MovieId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Game)
            .WithMany()
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Order>()
        //    .HasOne(o => o.Product)
        //    .WithMany()
        //    .HasForeignKey(o => o.ProductId)
        //    .OnDelete(DeleteBehavior.Restrict);

        // Define the relationship between Order and OrderItem
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)  // One Order has many OrderItems
            .WithOne(oi => oi.Order)     // One OrderItem belongs to one Order
            .HasForeignKey(oi => oi.OrderId)  // Foreign key reference
            .OnDelete(DeleteBehavior.Cascade); // If an Order is deleted, delete its OrderItems

        // Define the relationship between OrderItem and Product
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)  // One OrderItem has one Product
            .WithMany()   // No navigation property in Product (optional)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PurchasingInvoice>()
            .HasOne(pi => pi.Supplier)
            .WithMany()
            .HasForeignKey(pi => pi.SupplierId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PurchasingInvoice>()
            .HasOne(pi => pi.Product)
            .WithMany()
            .HasForeignKey(pi => pi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}



