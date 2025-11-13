using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ticklette.Domain.Models;

namespace Ticklette.Domain.Data;

public class TickletteContext : IdentityDbContext<User>
{
    public TickletteContext(DbContextOptions<TickletteContext> options)
        : base(options)
    {
    }
    // DbSets para Identity ya están incluidos
    public DbSet<Organizer> Organizers { get; set; }
    public DbSet<Attendee> Attendees { get; set; }
    public DbSet<OrganizingHouse> OrganizingHouses { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<TicketType> TicketTypes { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Entry> Entries { get; set; }
    public DbSet<VirtualCurrency> VirtualCurrencies { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User 1:1 with Organizer
        modelBuilder.Entity<User>()
            .HasOne(u => u.Organizer)
            .WithOne(o => o.User)
            .HasForeignKey<Organizer>(o => o.UserId);

        // User 1:1 with Attendee
        modelBuilder.Entity<User>()
            .HasOne(u => u.Attendee)
            .WithOne(a => a.User)
            .HasForeignKey<Attendee>(a => a.UserId);

        // User 1:1 with VirtualCurrency
        modelBuilder.Entity<User>()
            .HasOne(u => u.VirtualCurrency)
            .WithOne(v => v.User)
            .HasForeignKey<VirtualCurrency>(v => v.UserId);

        // Organizer 1:N with OrganizingHouse
        modelBuilder.Entity<Organizer>()
            .HasMany(o => o.OrganizingHouses)
            .WithOne(h => h.Organizer)
            .HasForeignKey(h => h.OrganizerId);

        // OrganizingHouse 1:N with Events
        modelBuilder.Entity<OrganizingHouse>()
            .HasMany(h => h.Events)
            .WithOne(e => e.OrganizingHouse)
            .HasForeignKey(e => e.OrganizingHouseId);

        // OrganizingHouse 1:N with Subscriptions
        modelBuilder.Entity<OrganizingHouse>()
            .HasMany(h => h.Subscriptions)
            .WithOne(s => s.OrganizingHouse)
            .HasForeignKey(s => s.OrganizingHouseId);

        // Event 1:N with TicketTypes
        modelBuilder.Entity<Event>()
            .HasMany(e => e.TicketTypes)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId);

        // Event 1:N with Products
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Products)
            .WithOne(p => p.Event)
            .HasForeignKey(p => p.EventId);

        // TicketType 1:N with Tickets
        modelBuilder.Entity<TicketType>()
            .HasMany(t => t.Tickets)
            .WithOne(t => t.TicketType)
            .HasForeignKey(t => t.TicketTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        // User 1:N with Tickets
        modelBuilder.Entity<User>()
            .HasMany(u => u.Tickets)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);

        // Ticket 1:1 with Entry
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Entry)
            .WithOne(e => e.Ticket)
            .HasForeignKey<Entry>(e => e.TicketId);

        // User 1:N with Sales
        modelBuilder.Entity<User>()
            .HasMany(u => u.Sales)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId);

         // Product 1:N with Sales - ESTA ES LA RELACIÓN PROBLEMÁTICA
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Sales)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        // Unique indexes
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.UniqueCode)
            .IsUnique();

        modelBuilder.Entity<Entry>()
            .HasIndex(e => e.TicketId)
            .IsUnique();
    }
}