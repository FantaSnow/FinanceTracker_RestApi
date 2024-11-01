using Domain.Users;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.Login).IsRequired().HasColumnType("varchar(255)");

        builder.Property(x => x.Password).IsRequired().HasColumnType("varchar(255)");

        builder.Property(x => x.Balance).IsRequired().HasColumnType("decimal(18, 2)");

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        
        builder.Property(x => x.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);
    }
}