namespace Waracle.HotelBooking.Api.Domain;

public sealed class Booking
{
    public int Id { get; set; }
    public required string Reference { get; set; }
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public required string GuestName { get; set; }
    public int GuestCount { get; set; }
    public DateOnly CheckInDate { get; set; }
    public DateOnly CheckOutDate { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
