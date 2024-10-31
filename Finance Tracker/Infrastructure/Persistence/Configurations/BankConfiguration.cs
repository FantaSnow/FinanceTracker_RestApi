using Domain.Banks;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BankConfiguration : IEntityTypeConfiguration<Bank>
{
    public void Configure(EntityTypeBuilder<Bank> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new BankId(x));

        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");

        builder.Property(x => x.Balance).IsRequired().HasColumnType("decimal(18, 2)");

        builder.Property(x => x.BalanceGoal).IsRequired().HasColumnType("decimal(18, 2)");

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.User).AutoInclude();
    }
}