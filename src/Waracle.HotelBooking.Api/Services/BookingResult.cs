using Waracle.HotelBooking.Api.Contracts;

namespace Waracle.HotelBooking.Api.Services;

public enum BookingFailure
{
    HotelNotFound,
    InvalidDateRange,
    InvalidGuestCount,
    NoAvailableRoom
}

public sealed record BookingResult(BookingDto? Booking, BookingFailure? Failure)
{
    public static BookingResult Success(BookingDto booking) => new(booking, null);
    public static BookingResult Failed(BookingFailure failure) => new(null, failure);
}
