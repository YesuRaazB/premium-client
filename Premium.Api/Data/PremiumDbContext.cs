using premium.Api.Models;
using Microsoft.EntityFrameworkCore;
//using premium.Api.Models;

namespace premium.Api.Data
{
    public class premiumDbContext : DbContext
    {
        public premiumDbContext(DbContextOptions<premiumDbContext> options) : base(options) { }
        public DbSet<Member> Members => Set<Member>();
        public DbSet<Occupation> Occupations => Set<Occupation>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Member>(b =>
            //{
            //    b.Property(m => m.MonthlyPremium).HasPrecision(18, 2); // or .HasColumnType("decimal(18,2)")
            //});

            //modelBuilder.Entity<Occupation>(b =>
            //{
            //    b.Property(o => o.Factor).HasPrecision(18, 4); // or .HasColumnType("decimal(18,4)")
            //});


            modelBuilder.Entity<Occupation>().HasData(
                new Occupation { Code = "Cleaner", DisplayName = "Cleaner", Rating = "Light Manual", Factor = 11.50m },
                new Occupation { Code = "Doctor", DisplayName = "Doctor", Rating = "Professional", Factor = 1.50m },
                new Occupation { Code = "Author", DisplayName = "Author", Rating = "White Collar", Factor = 2.25m },
                new Occupation { Code = "Farmer", DisplayName = "Farmer", Rating = "Heavy Manual", Factor = 31.75m },
                new Occupation { Code = "Mechanic", DisplayName = "Mechanic", Rating = "Heavy Manual", Factor = 31.75m },
                new Occupation { Code = "Florist", DisplayName = "Florist", Rating = "Light Manual", Factor = 11.50m },
                new Occupation { Code = "Other", DisplayName = "Other", Rating = "Heavy Manual", Factor = 31.75m }
            );
        }
    }
}
