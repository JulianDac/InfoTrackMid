using SettlementServiceWebAPI.Models;

namespace SettlementServiceWebAPI.Services
{
    public interface IBookingService
    {
        Task<List<TimeSlot>> GetAvailableSlotsAsync();
        Task<Booking> GetBookingByNameAsync(string name);
        Task<BookingResponse> CreateBookingAsync(BookingRequest request);
        Task<BookingResponse> AmendBookingAsync(string name, string newBookingTime);
        Task DeleteBookingAsync(string name);
    }
}
