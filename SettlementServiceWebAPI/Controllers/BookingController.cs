using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SettlementServiceWebAPI.Models;
using SettlementServiceWebAPI.Services;

namespace SettlementServiceWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;
        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots()
        {
            try
            {
                var slots = await _bookingService.GetAvailableSlotsAsync();
                return Ok(slots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available slots");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetBookingByName(string name)
        {
            try
            {
                var booking = await _bookingService.GetBookingByNameAsync(name);
                return Ok(booking);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving booking");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                var response = await _bookingService.CreateBookingAsync(request);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors.Select(e => e.ErrorMessage));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> AmendBooking(string name, [FromBody] BookingRequest request)
        {
            try
            {
                var response = await _bookingService.AmendBookingAsync(name, request.BookingTime);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors.Select(e => e.ErrorMessage));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error amending booking");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> DeleteBooking(string name)
        {
            try
            {
                await _bookingService.DeleteBookingAsync(name);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting booking");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
