using FluentValidation;
using SettlementServiceWebAPI.Models;

namespace SettlementServiceWebAPI.Validators
{
    public class BookingRequestValidator : AbstractValidator<BookingRequest>
    {
        public BookingRequestValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.BookingTime)
                .NotEmpty().WithMessage("Booking time is required")
                .Must(BeValidBookingTime).WithMessage("Booking time must be on the hour between 9:00 and 16:00, excluding 12:00.");
        }

        private bool BeValidBookingTime(string bookingTime)
        {
            if (!TimeOnly.TryParse(bookingTime, out TimeOnly time))
                return false;

            return time.Minute == 0 &&
                   time >= new TimeOnly(9, 0) &&
                   time <= new TimeOnly(16, 0) &&
                   time != new TimeOnly(12, 0);
        }
    }
}
