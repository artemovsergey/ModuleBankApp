using Microsoft.EntityFrameworkCore;
using ModuleBankApp.API.Domen;
using ModuleBankApp.API.Infrastructure.Data.Configurations;
using ModuleBankApp.API.Infrastructure.Messaging.Models;

namespace ModuleBankApp.API.Infrastructure.Data;

public class ModuleBankAppContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<OutboxMessage> Outbox => Set<OutboxMessage>();
    public DbSet<InboxMessage> Inbox => Set<InboxMessage>();
    
    public DbSet<AuditMessage> Audit => Set<AuditMessage>();

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

        modelBuilder.Entity<OutboxMessage>(eb =>
        {
            eb.ToTable("outbox_messages");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Type).IsRequired().HasMaxLength(512);
            eb.Property(x => x.Payload).IsRequired().HasColumnType("jsonb");
            eb.Property(x => x.Status).HasConversion<short>().IsRequired();
       
        });

        modelBuilder.Entity<InboxMessage>(eb =>
        {
            eb.ToTable("inbox_messages");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Type).HasMaxLength(512);
            eb.Property(x => x.Payload).IsRequired().HasColumnType("jsonb");
            eb.Property(x => x.ReceivedAtUtc);
        });
        
        modelBuilder.Entity<AuditMessage>(eb =>
        {
            eb.ToTable("audit_messages");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Payload).IsRequired().HasColumnType("jsonb");
            eb.Property(x => x.ReceivedAtUtc);
        });
    }
}