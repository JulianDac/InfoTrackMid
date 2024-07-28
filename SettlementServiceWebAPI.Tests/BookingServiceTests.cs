using Microsoft.AspNetCore.Mvc.Testing;
using SettlementServiceWebAPI.Models;
using System.Net.Http.Json;

namespace SettlementServiceWebAPI.Tests
{
    public class BookingServiceTests : IClassFixture<WebApplicationFactory<Program>>
    {
        // DOC: https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#a-simple-test
        private readonly WebApplicationFactory<Program> _factory;

        public BookingServiceTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateBookingAsync_ValidRequest_ReturnsBookingResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new BookingRequest { Name = "John Doe", BookingTime = "11:00" };

            // Act
            var response = await client.PostAsJsonAsync("/Booking", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var bookingResponse = await response.Content.ReadFromJsonAsync<BookingResponse>();
            Assert.NotNull(bookingResponse);
            Assert.NotEqual(Guid.Empty, bookingResponse.BookingId);
        }

        [Fact]
        public async Task CreateBookingAsync_InvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new BookingRequest { Name = "", BookingTime = "invalid" };

            // Act
            var response = await client.PostAsJsonAsync("/Booking", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateBookingAsync_NoAvailableSlots_ReturnsConflict()
        {
            // Arrange
            var client = _factory.CreateClient();
            var request = new BookingRequest { Name = "John Doe", BookingTime = "11:00" };

            // Book 4 slots
            for (int i = 0; i < 4; i++)
            {
                await client.PostAsJsonAsync("/Booking", request);
            }

            // Act
            var response = await client.PostAsJsonAsync("/Booking", request);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task GetAvailableSlotsAsync_ReturnsCorrectSlots()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Booking/available-slots");

            // Assert
            response.EnsureSuccessStatusCode();
            var slots = await response.Content.ReadFromJsonAsync<List<TimeSlot>>();
            Assert.NotNull(slots);
            Assert.Equal(8, slots.Count); // Assuming 7 slots (9:00 to 16:00, excluding 12:00)
            Assert.All(slots, slot => Assert.True(slot.IsAvailable));
        }

        [Fact]
        public async Task GetBookingByNameAsync_ExistingBooking_ReturnsBooking()
        {
            // Arrange
            var client = _factory.CreateClient();
            var bookingRequest = new BookingRequest { Name = "Test User", BookingTime = "10:00" };
            await client.PostAsJsonAsync("/Booking", bookingRequest);

            // Act
            var response = await client.GetAsync($"/Booking/{bookingRequest.Name}");

            // Assert
            response.EnsureSuccessStatusCode();
            var booking = await response.Content.ReadFromJsonAsync<Booking>();
            Assert.NotNull(booking);
            Assert.Equal(bookingRequest.Name, booking.Name);
            Assert.Equal(TimeOnly.Parse(bookingRequest.BookingTime), booking.BookingTime);
        }

        [Fact]
        public async Task GetBookingByNameAsync_NonExistentBooking_ReturnsNotFound()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Booking/NonExistentUser");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}
