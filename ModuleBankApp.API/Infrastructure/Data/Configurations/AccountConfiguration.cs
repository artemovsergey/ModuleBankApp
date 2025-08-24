using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuleBankApp.API.Domen;

namespace ModuleBankApp.API.Infrastructure.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts", schema: "public");
        
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(a => a.Balance)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(a => a.InterestRate)
            .HasColumnType("decimal(5,2)");

        builder.Property(a => a.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(a => a.ClosedAt)
            .HasColumnType("timestamp with time zone");

        builder.Property(a => a.OwnerId)
            .IsRequired();

        builder.Property<uint>("xmin")
            .IsRowVersion()
            .HasColumnName("xmin");

        builder.HasMany(a => a.Transactions)
            .WithOne()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Hash-индекс для OwnerId (точечные запросы)
        builder.HasIndex(a => a.OwnerId)
            .HasDatabaseName("IX_Accounts_OwnerId_Hash")
            .HasMethod("HASH");
        
        builder.HasIndex(a => new { a.Type, a.Currency })
            .HasDatabaseName("IX_Accounts_Type_Currency");

        //builder.HasData(GetSampleAccounts());
    }

    // ReSharper disable once UnusedMember.Local
    private static List<Account> GetSampleAccounts()
    {
        return
        [
            new Account
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Type = AccountType.Checking,
                Currency = "USD",
                Balance = 1500.00m,
                InterestRate = null,
                OwnerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
            },

            new Account
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Type = AccountType.Deposit,
                Currency = "EUR",
                Balance = 5000.00m,
                InterestRate = 3.5m,
                OwnerId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")
            },

            new Account
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Type = AccountType.Credit,
                Currency = "USD",
                Balance = 250.00m,
                InterestRate = 15.0m,
                OwnerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            },

            new Account
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Type = AccountType.Checking,
                Currency = "GBP",
                Balance = 3200.50m,
                InterestRate = null,
                OwnerId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")
            },

            new Account
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Type = AccountType.Deposit,
                Currency = "USD",
                Balance = 10000.00m,
                InterestRate = 4.2m,
                OwnerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")
            }
        ];
    }
    
    
}