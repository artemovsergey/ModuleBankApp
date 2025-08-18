using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuleBankApp.API.Domen;

namespace ModuleBankApp.API.Infrastructure.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        
        builder.ToTable("Transactions", schema: "public");

        
        builder.HasKey(t => t.Id);

        builder.Property(t => t.AccountId)
            .IsRequired();

        builder.Property(t => t.CounterPartyAccountId);

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(10);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Составной индекс (AccountId + CreatedAt)
        builder.HasIndex(t => new { t.AccountId, t.CreatedAt })
            .HasDatabaseName("IX_Transactions_AccountId_CreatedAt")
            .IsDescending(false, true);

        // GiST-индекс для диапазонных запросов по дате
        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Transactions_CreatedAt_Gist")
            .HasMethod("GIST");
        
        builder.HasIndex(t => new { t.Type, t.Currency })
            .HasDatabaseName("IX_Transactions_Type_Currency");

        //builder.HasData(GetSampleTransactions());
    }

    // ReSharper disable once UnusedMember.Local
    private static List<Transaction> GetSampleTransactions()
    {
        return
        [
            new Transaction
            {
                Id = Guid.Parse("01010101-0101-0101-0101-010101010101"),
                AccountId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CounterPartyAccountId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Currency = "USD",
                Amount = 100.00m,
                Type = TransactionType.Debit,
                Description = "Перевод на депозитный счет"
            },

            new Transaction
            {
                Id = Guid.Parse("02020202-0202-0202-0202-020202020202"),
                AccountId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CounterPartyAccountId = null,
                Currency = "EUR",
                Amount = 50.00m,
                Type = TransactionType.Credit,
                Description = "Пополнение через терминал"
            },

            new Transaction
            {
                Id = Guid.Parse("03030303-0303-0303-0303-030303030303"),
                AccountId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CounterPartyAccountId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Currency = "USD",
                Amount = 200.00m,
                Type = TransactionType.Credit,
                Description = "Погашение кредита"
            },

            new Transaction
            {
                Id = Guid.Parse("04040404-0404-0404-0404-040404040404"),
                AccountId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                CounterPartyAccountId = null,
                Currency = "GBP",
                Amount = 75.50m,
                Type = TransactionType.Debit,
                Description = "Оплата услуг"
            },

            new Transaction
            {
                Id = Guid.Parse("05050505-0505-0505-0505-050505050505"),
                AccountId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                CounterPartyAccountId = null,
                Currency = "USD",
                Amount = 500.00m,
                Type = TransactionType.Credit,
                Description = "Начисление процентов"
            }
        ];
    }
}

// +