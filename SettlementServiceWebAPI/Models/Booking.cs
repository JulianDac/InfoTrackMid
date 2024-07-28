using System;

namespace SettlementServiceWebAPI.Models
{
    public class Booking
    {
        public Guid BookingId { get; set; }
        public TimeOnly BookingTime { get; set; }
        public string Name { get; set; }
    }
}
