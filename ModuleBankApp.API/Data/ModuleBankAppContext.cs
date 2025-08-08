using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data.Configurations;
using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Data;

public class ModuleBankAppContext(IConfiguration config) : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(config.GetConnectionString("PostgreSQL"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountMapping());
        base.OnModelCreating(modelBuilder);
    }
}