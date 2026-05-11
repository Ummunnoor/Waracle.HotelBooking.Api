namespace Waracle.HotelBooking.Api.Domain;

public sealed class Hotel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Room> Rooms { get; set; } = [];
}
