using Microsoft.EntityFrameworkCore;
using PaymentGateway.Core;

namespace PaymentGateway.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Payment>()
            .Ignore(p => p.Cvv)
            .Ignore(p => p.ExpMonth)
            .Ignore(p => p.ExpYear);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Payment> Payments => Set<Payment>();
}