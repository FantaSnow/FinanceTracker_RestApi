using System.Reflection;
using Domain.Banks;
using Domain.BankTransactions;
using Domain.Categorys;
using Domain.Transactions;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categorys { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<BankTransaction> BankTransactions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Bank> Banks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}