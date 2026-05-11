using Waracle.HotelBooking.Api.Contracts;
using Waracle.HotelBooking.Api.Services;

namespace Waracle.HotelBooking.Api.Endpoints;

public static class BookingEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/bookings")
            .WithTags("Bookings");

        group.MapPost("/", async (
                CreateBookingRequest request,
                IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(request.GuestName))
                {
                    return Results.BadRequest(new { error = "guestName is required." });
                }

                var result = await bookingService.BookRoomAsync(request, cancellationToken);

                return result.Failure switch
                {
                    null => Results.Created($"/api/bookings/{result.Booking!.Reference}", result.Booking),
                    BookingFailure.HotelNotFound => Results.NotFound(new { error = "Hotel not found." }),
                    BookingFailure.InvalidDateRange => Results.BadRequest(new { error = "checkInDate must be before checkOutDate." }),
                    BookingFailure.InvalidGuestCount => Results.BadRequest(new { error = "guestCount must be at least 1." }),
                    BookingFailure.NoAvailableRoom => Results.Conflict(new { error = "No room is available for the requested stay and party size." }),
                    _ => Results.Problem("Unable to create booking.")
                };
            })
            .WithName("BookRoom")
            .WithSummary("Book one room for the whole stay.");

        group.MapGet("/{reference}", async (
                string reference,
                IBookingService bookingService,
                CancellationToken cancellationToken) =>
            {
                var booking = await bookingService.FindBookingAsync(reference, cancellationToken);

                return booking is null
                    ? Results.NotFound(new { error = "Booking not found." })
                    : Results.Ok(booking);
            })
            .WithName("FindBooking")
            .WithSummary("Find booking details by booking reference.");

        return app;
    }
}
