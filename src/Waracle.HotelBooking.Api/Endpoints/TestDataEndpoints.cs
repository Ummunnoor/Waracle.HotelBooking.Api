using Waracle.HotelBooking.Api.Services;

namespace Waracle.HotelBooking.Api.Endpoints;

public static class TestDataEndpoints
{
    public static IEndpointRouteBuilder MapTestDataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/test-data")
            .WithTags("Test Data");

        group.MapPost("/seed", async (ISeedDataService seedDataService, CancellationToken cancellationToken) =>
            {
                await seedDataService.SeedAsync(cancellationToken);
                return Results.NoContent();
            })
            .WithName("SeedTestData")
            .WithSummary("Populate the database with a hotel and six rooms.");

        group.MapDelete("/reset", async (ISeedDataService seedDataService, CancellationToken cancellationToken) =>
            {
                await seedDataService.ResetAsync(cancellationToken);
                return Results.NoContent();
            })
            .WithName("ResetTestData")
            .WithSummary("Remove all bookings, rooms and hotels.");

        return app;
    }
}
