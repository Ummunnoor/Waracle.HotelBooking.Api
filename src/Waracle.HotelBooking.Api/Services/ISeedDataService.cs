namespace Waracle.HotelBooking.Api.Services;

public interface ISeedDataService
{
    Task SeedAsync(CancellationToken cancellationToken);
    Task ResetAsync(CancellationToken cancellationToken);
}
