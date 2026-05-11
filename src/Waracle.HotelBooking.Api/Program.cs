using Microsoft.EntityFrameworkCore;
using Waracle.HotelBooking.Api.Data;
using Waracle.HotelBooking.Api.Endpoints;
using Waracle.HotelBooking.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HotelBookingDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("HotelBooking")
        ?? "Data Source=hotel-booking.db";

    options.UseSqlite(connectionString);
});

builder.Services.AddSingleton<BookingWriteLock>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISeedDataService, SeedDataService>();

var app = builder.Build();

app.UseExceptionHandler();

    app.UseSwagger();
    app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<HotelBookingDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapHotelEndpoints();
app.MapRoomEndpoints();
app.MapBookingEndpoints();
app.MapTestDataEndpoints();
app.Urls.Add("http://0.0.0.0:8080");
app.Run();

public partial class Program;
