using AppCore.Models;
using AppCore.Enums;
using AppCore.ValueObjects;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Context;

public class ContactsDbContext : IdentityDbContext<CrmUser, CrmRole, string>
{
    public DbSet<Person> People { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public ContactsDbContext() { }

    public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=contacts.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Identity Configuration
        builder.Entity<CrmUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100);
            entity.Property(u => u.LastName).HasMaxLength(100);
            entity.Property(u => u.Department).HasMaxLength(100);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        builder.Entity<CrmRole>(entity =>
        {
            entity.Property(r => r.Name).HasMaxLength(20);
        });

        // 2. TPH (Table Per Hierarchy) Configuration
        builder.Entity<Contact>()
            .HasDiscriminator<string>("ContactType")
            .HasValue<Person>("Person")
            .HasValue<Company>("Company")
            .HasValue<Organization>("Organization");

        builder.Entity<Contact>(entity =>
        {
            entity.Property(p => p.Email).HasMaxLength(200);
            entity.Property(p => p.Phone).HasMaxLength(20);
        });

        builder.Entity<Person>(entity =>
        {
            entity.Property(p => p.BirthDate).HasColumnType("date");
            entity.Property(p => p.Gender).HasConversion<string>();
            entity.Property(p => p.Status).HasConversion<string>();
            
            entity.HasOne(p => p.Employer)
                  .WithMany(e => e.Employees);
        });

        builder.Entity<Organization>()
            .HasMany(o => o.Members)
            .WithOne(p => p.Organization);

        // 3. Seed Data
        var companyId = Guid.Parse("516A34D7-CCFB-4F20-85F3-62BD0F3AF271");
        var personId1 = Guid.Parse("3d54091d-abc8-49ec-9590-93ad3ed5458f");
        var personId2 = Guid.Parse("B4DCB17C-F875-43F8-9D66-36597895A466");
        var seedDate = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Use Anonymous Object for Company to bypass Address constructor initialization
        builder.Entity<Company>().HasData(new 
        {
            Id = companyId,
            Name = "WSEI",
            Industry = "edukacja",
            Phone = "123567123",
            Email = "biuro@wsei.edu.pl",
            Website = "https://wsei.edu.pl",
            Status = ContactStatus.Active,
            CreatedAt = seedDate
        });

        builder.Entity<Person>().HasData(
            new
            {
                Id = personId1,
                FirstName = "Adam",
                LastName = "Nowak",
                Gender = Gender.Male,
                Status = ContactStatus.Active,
                Email = "adam@wsei.edu.pl",
                Phone = "123456789",
                BirthDate = DateTime.Parse("2001-01-11"),
                Position = "Programista",
                CreatedAt = seedDate
            },
            new
            {
                Id = personId2,
                FirstName = "Ewa",
                LastName = "Kowalska",
                Gender = Gender.Female,
                Status = ContactStatus.Blocked,
                Email = "ewa@wsei.edu.pl",
                Phone = "123123123",
                BirthDate = DateTime.Parse("2001-01-11"),
                Position = "Tester",
                CreatedAt = seedDate
            }
        );

        // Map Address as an Owned Type and seed with integer 'id'
        builder.Entity<Contact>().OwnsOne(c => c.Address).HasData(
            new
            {
                id = 1,
                ContactId = personId1,
                City = "Kraków",
                Country = Country.PL,
                ZipCode = "25-009",
                Street = "ul. Św. Filipa 17",
                Type = AddressType.Correspondence
            },
            new
            {
                id = 2,
                ContactId = companyId,
                City = "Kraków",
                Country = Country.PL,
                ZipCode = "31-150",
                Street = "ul. św. Filipa 17",
                Type = AddressType.Delivery
            },
            new
            {
                id = 3,
                ContactId = personId2,
                City = "Kraków",
                Country = Country.PL,
                ZipCode = "31-150",
                Street = "ul. św. Filipa 17",
                Type = AddressType.Correspondence
            }
        );
    }
}