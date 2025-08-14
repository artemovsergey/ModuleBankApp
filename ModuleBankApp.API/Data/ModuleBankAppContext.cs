using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Data.Configurations;
using ModuleBankApp.API.Features.Accounts;
using ModuleBankApp.API.Features.Transactions;

namespace ModuleBankApp.API.Data;

public class ModuleBankAppContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public ModuleBankAppContext(DbContextOptions<ModuleBankAppContext> opt)
        : base(opt)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    // ReSharper disable once UnusedMember.Global
    public async Task AccrueInterestAsync(Guid accountId)
    {
        const string sql = "CALL public.accrue_interest({0})";
        await Database.ExecuteSqlRawAsync(sql, accountId);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("btree_gist");

        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}

// +