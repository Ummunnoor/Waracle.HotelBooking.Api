namespace Waracle.HotelBooking.Api.Services;

public sealed class BookingWriteLock
{
    public SemaphoreSlim Semaphore { get; } = new(1, 1);
}
