using Microsoft.EntityFrameworkCore;
using Waracle.HotelBooking.Api.Contracts;
using Waracle.HotelBooking.Api.Data;
using Waracle.HotelBooking.Api.Domain;

namespace Waracle.HotelBooking.Api.Services;

public sealed class BookingService(HotelBookingDbContext dbContext, BookingWriteLock bookingWriteLock) : IBookingService
{
    public async Task<IReadOnlyList<RoomAvailabilityDto>> FindAvailableRoomsAsync(
        int hotelId,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int guestCount,
        CancellationToken cancellationToken)
    {
        if (!IsValidStay(checkInDate, checkOutDate) || guestCount < 1)
        {
            return [];
        }

        var occupiedRoomIds = dbContext.Bookings
            .Where(booking => booking.HotelId == hotelId)
            .Where(booking => booking.CheckInDate < checkOutDate && checkInDate < booking.CheckOutDate)
            .Select(booking => booking.RoomId);

        return await dbContext.Rooms
            .Where(room => room.HotelId == hotelId)
            .Where(room => room.Capacity >= guestCount)
            .Where(room => !occupiedRoomIds.Contains(room.Id))
            .OrderBy(room => room.Capacity)
            .ThenBy(room => room.Number)
            .Select(room => new RoomAvailabilityDto(
                room.Id,
                room.Number,
                room.Type.ToString().ToLowerInvariant(),
                room.Capacity))
            .ToListAsync(cancellationToken);
    }

    public async Task<BookingResult> BookRoomAsync(CreateBookingRequest request, CancellationToken cancellationToken)
    {
        if (!IsValidStay(request.CheckInDate, request.CheckOutDate))
        {
            return BookingResult.Failed(BookingFailure.InvalidDateRange);
        }

        if (request.GuestCount < 1)
        {
            return BookingResult.Failed(BookingFailure.InvalidGuestCount);
        }

        await bookingWriteLock.Semaphore.WaitAsync(cancellationToken);
        try
        {
            var hotelExists = await dbContext.Hotels
                .AnyAsync(hotel => hotel.Id == request.HotelId, cancellationToken);

            if (!hotelExists)
            {
                return BookingResult.Failed(BookingFailure.HotelNotFound);
            }

            var availableRoom = await FindAvailableRoomQuery(
                    request.HotelId,
                    request.CheckInDate,
                    request.CheckOutDate,
                    request.GuestCount)
                .FirstOrDefaultAsync(cancellationToken);

            if (availableRoom is null)
            {
                return BookingResult.Failed(BookingFailure.NoAvailableRoom);
            }

            var booking = new Booking
            {
                Reference = await GenerateUniqueReferenceAsync(cancellationToken),
                HotelId = request.HotelId,
                RoomId = availableRoom.Id,
                GuestName = request.GuestName.Trim(),
                GuestCount = request.GuestCount,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            dbContext.Bookings.Add(booking);
            await dbContext.SaveChangesAsync(cancellationToken);

            return BookingResult.Success((await FindBookingAsync(booking.Reference, cancellationToken))!);
        }
        finally
        {
            bookingWriteLock.Semaphore.Release();
        }
    }

    public async Task<BookingDto?> FindBookingAsync(string reference, CancellationToken cancellationToken)
    {
        var normalisedReference = reference.Trim().ToUpperInvariant();

        return await dbContext.Bookings
            .Where(booking => booking.Reference == normalisedReference)
            .Select(booking => new BookingDto(
                booking.Reference,
                booking.HotelId,
                booking.Hotel!.Name,
                booking.RoomId,
                booking.Room!.Number,
                booking.Room.Type.ToString().ToLowerInvariant(),
                booking.GuestName,
                booking.GuestCount,
                booking.CheckInDate,
                booking.CheckOutDate,
                booking.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    private IQueryable<Room> FindAvailableRoomQuery(
        int hotelId,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int guestCount)
    {
        var occupiedRoomIds = dbContext.Bookings
            .Where(booking => booking.HotelId == hotelId)
            .Where(booking => booking.CheckInDate < checkOutDate && checkInDate < booking.CheckOutDate)
            .Select(booking => booking.RoomId);

        return dbContext.Rooms
            .Where(room => room.HotelId == hotelId)
            .Where(room => room.Capacity >= guestCount)
            .Where(room => !occupiedRoomIds.Contains(room.Id))
            .OrderBy(room => room.Capacity)
            .ThenBy(room => room.Number);
    }

    private async Task<string> GenerateUniqueReferenceAsync(CancellationToken cancellationToken)
    {
        string reference;

        do
        {
            reference = $"WB{Random.Shared.Next(10000000, 99999999)}";
        }
        while (await dbContext.Bookings.AnyAsync(booking => booking.Reference == reference, cancellationToken));

        return reference;
    }

    private static bool IsValidStay(DateOnly checkInDate, DateOnly checkOutDate) => checkInDate < checkOutDate;
}
