namespace Waracle.HotelBooking.Api.Contracts;

public sealed record CreateBookingRequest(
    int HotelId,
    string GuestName,
    int GuestCount,
    DateOnly CheckInDate,
    DateOnly CheckOutDate);

public sealed record BookingDto(
    string Reference,
    int HotelId,
    string HotelName,
    int RoomId,
    string RoomNumber,
    string RoomType,
    string GuestName,
    int GuestCount,
    DateOnly CheckInDate,
    DateOnly CheckOutDate,
    DateTimeOffset CreatedAtUtc);
