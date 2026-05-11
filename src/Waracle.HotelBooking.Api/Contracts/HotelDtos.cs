namespace Waracle.HotelBooking.Api.Contracts;

public sealed record HotelDto(int Id, string Name);

public sealed record RoomAvailabilityDto(
    int RoomId,
    string RoomNumber,
    string RoomType,
    int Capacity);
