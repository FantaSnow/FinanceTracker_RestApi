using Domain.Banks;
using Domain.BankTransactions;
using Domain.Categorys;
using Domain.Users;
using Domain.Transactions;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BankTransactionConfiguration : IEntityTypeConfiguration<BankTransaction>
{
    public void Configure(EntityTypeBuilder<BankTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new BankTransactionId(x));

        builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18, 2)");
        
        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.BankId)
            .HasConversion(x => x.Value, x => new BankId(x))
            .IsRequired();
        builder.HasOne(x => x.Bank)
            .WithMany()
            .HasForeignKey(x => x.BankId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}
