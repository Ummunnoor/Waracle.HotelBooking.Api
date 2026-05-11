using Microsoft.EntityFrameworkCore;
using Waracle.HotelBooking.Api.Data;
using Waracle.HotelBooking.Api.Domain;

namespace Waracle.HotelBooking.Api.Services;

public sealed class SeedDataService(HotelBookingDbContext dbContext) : ISeedDataService
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Hotels.AnyAsync(cancellationToken))
        {
            return;
        }

        dbContext.Hotels.Add(new Hotel
        {
            Name = "Waracle Grand Hotel",
            Rooms =
            [
                new Room { Number = "101", Type = RoomType.Single, Capacity = 1 },
                new Room { Number = "102", Type = RoomType.Single, Capacity = 1 },
                new Room { Number = "201", Type = RoomType.Double, Capacity = 2 },
                new Room { Number = "202", Type = RoomType.Double, Capacity = 2 },
                new Room { Number = "301", Type = RoomType.Deluxe, Capacity = 4 },
                new Room { Number = "302", Type = RoomType.Deluxe, Capacity = 4 }
            ]
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetAsync(CancellationToken cancellationToken)
    {
        await dbContext.Bookings.ExecuteDeleteAsync(cancellationToken);
        await dbContext.Rooms.ExecuteDeleteAsync(cancellationToken);
        await dbContext.Hotels.ExecuteDeleteAsync(cancellationToken);
    }
}
