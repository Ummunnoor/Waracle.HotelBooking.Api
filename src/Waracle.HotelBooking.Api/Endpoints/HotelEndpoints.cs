using Microsoft.EntityFrameworkCore;
using Waracle.HotelBooking.Api.Contracts;
using Waracle.HotelBooking.Api.Data;

namespace Waracle.HotelBooking.Api.Endpoints;

public static class HotelEndpoints
{
    public static IEndpointRouteBuilder MapHotelEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/hotels")
            .WithTags("Hotels");

        group.MapGet("/", async (string? name, HotelBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var query = dbContext.Hotels.AsQueryable();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(hotel => EF.Functions.Like(hotel.Name, $"%{name.Trim()}%"));
                }

                var hotels = await query
                    .OrderBy(hotel => hotel.Name)
                    .Select(hotel => new HotelDto(hotel.Id, hotel.Name))
                    .ToListAsync(cancellationToken);

                return Results.Ok(hotels);
            })
            .WithName("FindHotels")
            .WithSummary("Find hotels by name (optional).");

        group.MapGet("/{id:int}", async (int id, HotelBookingDbContext dbContext, CancellationToken cancellationToken) =>
            {
                var hotel = await dbContext.Hotels
                    .Where(hotel => hotel.Id == id)
                    .Select(hotel => new HotelDto(hotel.Id, hotel.Name))
                    .FirstOrDefaultAsync(cancellationToken);

                return hotel is null
                    ? Results.NotFound(new { error = "Hotel not found." })
                    : Results.Ok(hotel);
            })
            .WithName("GetHotel")
            .WithSummary("Get hotel by ID.");

        return app;
    }
}
