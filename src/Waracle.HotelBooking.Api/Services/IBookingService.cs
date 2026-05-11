using Waracle.HotelBooking.Api.Contracts;

namespace Waracle.HotelBooking.Api.Services;

public interface IBookingService
{
    Task<IReadOnlyList<RoomAvailabilityDto>> FindAvailableRoomsAsync(
        int hotelId,
        DateOnly checkInDate,
        DateOnly checkOutDate,
        int guestCount,
        CancellationToken cancellationToken);

    Task<BookingResult> BookRoomAsync(CreateBookingRequest request, CancellationToken cancellationToken);

    Task<BookingDto?> FindBookingAsync(string reference, CancellationToken cancellationToken);
}
