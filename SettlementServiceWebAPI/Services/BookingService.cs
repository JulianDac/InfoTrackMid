using FluentValidation;
using SettlementServiceWebAPI.Data;
using SettlementServiceWebAPI.Models;

namespace SettlementServiceWebAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly InMemoryBookingStore _store;
        private readonly IValidator<BookingRequest> _validator;
        private const int MAX_CONCURRENT_BOOKINGS = 4;
        private const int BOOKING_DURATION_HOURS = 1;

        public BookingService(InMemoryBookingStore store, IValidator<BookingRequest> validator)
        {
            _store = store;
            _validator = validator;
        }

        public async Task<List<TimeSlot>> GetAvailableSlotsAsync()
        {
            var slots = new List<TimeSlot>();
            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(17, 0);

            while (startTime < endTime)
            {
                var bookingsCount = _store.Bookings
                    .Count(b => b.BookingTime == startTime);

                slots.Add(new TimeSlot
                {
                    StartTime = startTime,
                    EndTime = startTime.AddHours(BOOKING_DURATION_HOURS),
                    IsAvailable = bookingsCount < MAX_CONCURRENT_BOOKINGS
                });

                startTime = startTime.AddHours(BOOKING_DURATION_HOURS);
            }

            return await Task.FromResult(slots);
        }

        public async Task<Booking> GetBookingByNameAsync(string name)
        {
            var booking = _store.Bookings
                .FirstOrDefault(b => b.Name.ToLower() == name.ToLower());

            if (booking == null)
            {
                throw new KeyNotFoundException($"No booking found for {name}");
            }

            return await Task.FromResult(booking);
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var bookingTime = TimeOnly.Parse(request.BookingTime);

            // Disallow multiple bookings by same person
            var existingBooking = _store.Bookings.FirstOrDefault(b => b.Name.ToLower() == request.Name.ToLower());

            if (existingBooking != null)
            {
                throw new InvalidOperationException("A booking already exists for this name already.");
            }

            var existingBookingsCount = _store.Bookings
                .Count(b => b.BookingTime == bookingTime);

            if (existingBookingsCount >= MAX_CONCURRENT_BOOKINGS)
            {
                throw new InvalidOperationException("No available slots available for the requested time");
            }

            var booking = new Booking
            {
                BookingId = Guid.NewGuid(),
                BookingTime = bookingTime,
                Name = request.Name
            };

            _store.Bookings.Add(booking);

            return await Task.FromResult(new BookingResponse { BookingId = booking.BookingId });
        }

        public async Task<BookingResponse> AmendBookingAsync(string name, string newBookingTime)
        {
            var booking = await GetBookingByNameAsync(name);
            var validationResult = await _validator.ValidateAsync(new BookingRequest { Name = name, BookingTime = newBookingTime });

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var newTime = TimeOnly.Parse(newBookingTime);

            var existingBookingsCount = _store.Bookings
                .Count(b => b.BookingTime == newTime && b.BookingId != booking.BookingId);

            if (existingBookingsCount >= MAX_CONCURRENT_BOOKINGS)
            {
                throw new InvalidOperationException("No available slots for the requested time");
            }

            booking.BookingTime = newTime;

            return await Task.FromResult(new BookingResponse { BookingId = booking.BookingId });
        }

        public async Task DeleteBookingAsync(string name)
        {
            var booking = await GetBookingByNameAsync(name);
            _store.Bookings.Remove(booking);
        }
    }
}
