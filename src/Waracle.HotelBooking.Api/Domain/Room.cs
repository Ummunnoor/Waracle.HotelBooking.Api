namespace Waracle.HotelBooking.Api.Domain;

public sealed class Room
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public required string Number { get; set; }
    public RoomType Type { get; set; }
    public int Capacity { get; set; }
    public List<Booking> Bookings { get; set; } = [];
}
