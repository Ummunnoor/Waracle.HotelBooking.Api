# Waracle Hotel Booking API

ASP.NET Core and EF Core solution for the hotel room booking challenge.

## Run locally

```powershell
dotnet run --project src/Waracle.HotelBooking.Api
```

Swagger UI and OpenAPI are exposed in development at:

```text
http://localhost:5137/swagger
http://localhost:5137/swagger/v1/swagger.json
```

The API uses SQLite by default and creates `hotel-booking.db` automatically.

## Useful endpoints

- `POST /api/test-data/seed` - creates one hotel with six rooms.
- `DELETE /api/test-data/reset` - clears hotels, rooms and bookings.
- `GET /api/hotels?name=Waracle` - finds hotels by name.
- `GET /api/hotels/{hotelId}/rooms/available?checkInDate=2026-06-01&checkOutDate=2026-06-03&guests=2` - finds rooms available for the whole stay.
- `POST /api/bookings` - books the smallest suitable available room.
- `GET /api/bookings/{reference}` - retrieves booking details.

Example booking request:

```json
{
  "hotelId": 1,
  "guestName": "Ada Lovelace",
  "guestCount": 2,
  "checkInDate": "2026-06-01",
  "checkOutDate": "2026-06-03"
}
```

## Business rules covered

- Seed data creates three room types: single, double and deluxe.
- Each seeded hotel has exactly six rooms.
- Availability checks use date range overlap logic for nightly stays: `existing.CheckIn < requested.CheckOut && requested.CheckIn < existing.CheckOut`.
- A booking allocates one room for the full stay, so guests never need to change rooms.
- Booking references are unique and enforced by a database unique index.
- Guests cannot book rooms below their party size.

## Tests

```powershell
dotnet test Waracle.HotelBooking.sln -m:1
```
