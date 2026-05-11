using Microsoft.EntityFrameworkCore;
using Waracle.HotelBooking.Api.Domain;

namespace Waracle.HotelBooking.Api.Data;

public sealed class HotelBookingDbContext(DbContextOptions<HotelBookingDbContext> options) : DbContext(options)
{
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.Property(hotel => hotel.Name).HasMaxLength(160).IsRequired();
            entity.HasIndex(hotel => hotel.Name).IsUnique();
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.Property(room => room.Number).HasMaxLength(20).IsRequired();
            entity.Property(room => room.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.HasIndex(room => new { room.HotelId, room.Number }).IsUnique();
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(booking => booking.Reference).HasMaxLength(16).IsRequired();
            entity.Property(booking => booking.GuestName).HasMaxLength(120).IsRequired();
            entity.HasIndex(booking => booking.Reference).IsUnique();
            entity.HasIndex(booking => new { booking.RoomId, booking.CheckInDate, booking.CheckOutDate });
        });
    }
}
