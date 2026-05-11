using Waracle.HotelBooking.Api.Services;

namespace Waracle.HotelBooking.Api.Endpoints;

public static class RoomEndpoints
{
    public static IEndpointRouteBuilder MapRoomEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hotels/{hotelId:int}/rooms")
            .WithTags("Rooms");

        group.MapGet("/available", async (
                int hotelId,
                DateOnly checkInDate,
                DateOnly checkOutDate,
                int guests,
                IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                if (checkInDate >= checkOutDate)
                {
                    return Results.BadRequest(new { error = "checkInDate must be before checkOutDate." });
                }

                if (guests < 1)
                {
                    return Results.BadRequest(new { error = "guests must be at least 1." });
                }

                var rooms = await bookingService.FindAvailableRoomsAsync(
                    hotelId,
                    checkInDate,
                    checkOutDate,
                    guests,
                    cancellationToken);

                return Results.Ok(rooms);
            })
            .WithName("FindAvailableRooms")
            .WithSummary("Find rooms that can hold the whole stay without requiring a room change.");

        return app;
    }
}
