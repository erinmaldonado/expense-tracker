using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class FinanceDbContext : IdentityDbContext<IdentityUser>
{
    public FinanceDbContext()
    {
    }

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Expense> Expenses { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json", optional:true);
        IConfigurationRoot configuration = configurationBuilder.Build();
        
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        }
        {
            
        }
    }

protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. IMPORTANT: Call the base method first for Identity configuration
        base.OnModelCreating(modelBuilder); 

        // 2. Custom Model Configuration (Expense and Category)
        modelBuilder.Entity<Expense>(entity =>
        {
            // Example of how to define the relationship
            entity.HasOne(d => d.Category)
                  .WithMany(p => p.Expenses)
                  .HasForeignKey(d => d.CategoryId)
                  .OnDelete(DeleteBehavior.Cascade); // Use Cascade or ClientSetNull
        });
    }
}
