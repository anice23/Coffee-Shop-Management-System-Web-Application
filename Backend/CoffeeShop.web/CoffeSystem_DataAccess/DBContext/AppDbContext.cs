using CoffeSystem_DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using System;


namespace CoffeSystem_DataAccess.DBContext
{
    public class AppDbContext : DbContext
    {
        private static readonly Lazy<string> _connectionString = new Lazy<string>(() =>
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: false
                )
                .Build();

            string? connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found in appsettings.json."
                );
            }

            return connectionString;
        });
        // Constructors
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // 1- n7adhrou tables eli bech nasn3ouhouf f database
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<CafeSetting> CafeSettings { get; set; }


        // 2- bech n7adhrou l methode eli na3tiha connectionstring w hiya torbet rou7ha bl sql server w tasna3 l database mte3i

       

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _connectionString.Value,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null
                    )
                );
            }
        }




        // 3- bech na7adhrou l method eli bech n7ot fiha l constraint 3la columns bech yet7atou f database ( unique , not null , >.. ...)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.Property(u => u.UserName).HasMaxLength(50);
                entity.Property(u => u.FullName).HasMaxLength(100);
            });

            // 2. Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(50);
            });

            // 3. Products
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(p => p.Name).IsUnique();
                entity.Property(p => p.Name).HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.IsActive).HasDefaultValue(true);

            });

            // 4. Orders
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            });

            // 5. OrderItems
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
            });
        }
        public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

                // هنا السر: نحددوا الـ SqlServer مباشرة في الـ Factory!
                optionsBuilder.UseSqlServer("Server=.;Database=CoffeeShopDB;Trusted_Connection=True;TrustServerCertificate=True;");

                return new AppDbContext(optionsBuilder.Options);
            }
        }
    }
}
