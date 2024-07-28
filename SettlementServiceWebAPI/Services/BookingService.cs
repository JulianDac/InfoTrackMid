using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SettlementServiceWebAPI.Data;
using SettlementServiceWebAPI.Models;

namespace SettlementServiceWebAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IValidator<BookingRequest> _validator;
        private const int MAX_CONCURRENT_BOOKINGS = 4;
        private const int BOOKING_DURATION_HOURS = 1;

        public BookingService(ApplicationDbContext context, IValidator<BookingRequest> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<List<TimeSlot>> GetAvailableSlotsAsync()
        {
            var slots = new List<TimeSlot>();
            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(16, 0);

            while (startTime < endTime)
            {
                var bookingsCount = await _context.Bookings
                    .CountAsync(b => b.BookingTime == startTime);

                slots.Add(new TimeSlot
                {
                    StartTime = startTime,
                    EndTime = startTime.AddHours(BOOKING_DURATION_HOURS),
                    IsAvailable = bookingsCount < MAX_CONCURRENT_BOOKINGS
                });

                startTime = startTime.AddHours(BOOKING_DURATION_HOURS);
            }

            return slots;
        }

        public async Task<Booking> GetBookingByNameAsync(string name)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Name.ToLower() == name.ToLower());

            if (booking == null)
            {
                throw new KeyNotFoundException($"No booking found for {name}");
            }

            return booking;
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
            var existingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Name.ToLower() == request.Name.ToLower());

            if (existingBooking != null)
            {
                throw new InvalidOperationException("A booking already exists for this name already.");
            }

            var existingBookingsCount = await _context.Bookings
                .CountAsync(b => b.BookingTime == bookingTime);

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

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return new BookingResponse { BookingId = booking.BookingId };
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

            var existingBookingsCount = await _context.Bookings
                .CountAsync(b => b.BookingTime == newTime && b.BookingId != booking.BookingId);

            if (existingBookingsCount >= MAX_CONCURRENT_BOOKINGS)
            {
                throw new InvalidOperationException("No available slots for the requested time");
            }

            booking.BookingTime = newTime;
            await _context.SaveChangesAsync();

            return new BookingResponse { BookingId = booking.BookingId };
        }

        public async Task DeleteBookingAsync(string name)
        {
            var booking = await GetBookingByNameAsync(name);

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
