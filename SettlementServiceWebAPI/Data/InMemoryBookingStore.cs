using SettlementServiceWebAPI.Models;

namespace SettlementServiceWebAPI.Data
{
    public class InMemoryBookingStore
    {
        public List<Booking> Bookings { get; set; }

        public InMemoryBookingStore()
        {
            Bookings = new List<Booking>
        {
            new Booking
            {
                BookingId = Guid.NewGuid(),
                BookingTime = new TimeOnly(9, 0),
                Name = "Alice"
            },
            new Booking
            {
                BookingId = Guid.NewGuid(),
                BookingTime = new TimeOnly(9, 0),
                Name = "Bob"
            },
            new Booking
            {
                BookingId = Guid.NewGuid(),
                BookingTime = new TimeOnly(9, 0),
                Name = "Charlie"
            }
        };
        }
    }
}
