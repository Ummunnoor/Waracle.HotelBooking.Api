using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Waracle.HotelBooking.Api.Contracts;
using Waracle.HotelBooking.Api.Data;
using Waracle.HotelBooking.Api.Domain;
using Waracle.HotelBooking.Api.Services;

namespace Waracle.HotelBooking.Api.Tests;

public sealed class BookingServiceTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly HotelBookingDbContext _dbContext;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _connection.Open();

        var options = new DbContextOptionsBuilder<HotelBookingDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new HotelBookingDbContext(options);
        _dbContext.Database.EnsureCreated();
        _bookingService = new BookingService(_dbContext, new BookingWriteLock());

        _dbContext.Hotels.Add(new Hotel
        {
            Name = "Waracle Grand Hotel",
            Rooms =
            [
                new Room { Number = "101", Type = RoomType.Single, Capacity = 1 },
                new Room { Number = "201", Type = RoomType.Double, Capacity = 2 },
                new Room { Number = "301", Type = RoomType.Deluxe, Capacity = 4 }
            ]
        });
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task BookRoomAsync_AllocatesSmallestRoomThatFitsParty()
    {
        var result = await _bookingService.BookRoomAsync(
            new CreateBookingRequest(1, "Ada Lovelace", 2, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
            CancellationToken.None);

        Assert.Null(result.Failure);
        Assert.Equal("201", result.Booking!.RoomNumber);
        Assert.Equal("double", result.Booking.RoomType);
    }

    [Fact]
    public async Task FindAvailableRoomsAsync_ExcludesRoomsWithOverlappingBookings()
    {
        await _bookingService.BookRoomAsync(
            new CreateBookingRequest(1, "Ada Lovelace", 2, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
            CancellationToken.None);

        var rooms = await _bookingService.FindAvailableRoomsAsync(
            1,
            new DateOnly(2026, 6, 2),
            new DateOnly(2026, 6, 4),
            2,
            CancellationToken.None);

        Assert.DoesNotContain(rooms, room => room.RoomNumber == "201");
        Assert.Contains(rooms, room => room.RoomNumber == "301");
    }

    [Fact]
    public async Task FindAvailableRoomsAsync_AllowsCheckoutAndCheckinOnSameDay()
    {
        await _bookingService.BookRoomAsync(
            new CreateBookingRequest(1, "Ada Lovelace", 2, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3)),
            CancellationToken.None);

        var rooms = await _bookingService.FindAvailableRoomsAsync(
            1,
            new DateOnly(2026, 6, 3),
            new DateOnly(2026, 6, 5),
            2,
            CancellationToken.None);

        Assert.Contains(rooms, room => room.RoomNumber == "201");
    }

    [Fact]
    public async Task BookRoomAsync_RejectsPartiesAboveRoomCapacity()
    {
        var result = await _bookingService.BookRoomAsync(
            new CreateBookingRequest(1, "Grace Hopper", 5, new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 2)),
            CancellationToken.None);

        Assert.Equal(BookingFailure.NoAvailableRoom, result.Failure);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
