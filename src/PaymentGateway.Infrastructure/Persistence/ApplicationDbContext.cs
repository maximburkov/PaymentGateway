using Microsoft.EntityFrameworkCore;
using PaymentGateway.Core;

namespace PaymentGateway.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments => Set<Payment>();
}