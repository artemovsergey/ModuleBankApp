using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModuleBankApp.API.Features.Accounts;

namespace ModuleBankApp.API.Data.Configurations;

public class AccountMapping : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts").HasKey(a => a.Id);
        
        builder.Property(a => a.CreatedAt)
                .HasColumnType("timestamp with time zone");
        //.HasConversion(new DateTimeToCharConverter());

        //enum
        // builder.Property(a => a.Status).HasConversion<string>();
        
        //relation
        // builder.HasOne(g => g.User)
        //        .WithMany(user => user.Games)
        //        .HasPrincipalKey(g =>g.Id)
        //        .HasForeignKey(u => u.UserId);

        // Seed only static data - no generated data
         builder.HasData();
    }
}