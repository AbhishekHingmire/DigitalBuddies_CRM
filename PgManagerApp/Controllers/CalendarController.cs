using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PgManagerApp.Models;

namespace PgManagerApp.Controllers
{
    [Authorize(Roles = "User, Admin")]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var events = _context.CalendarEvents.ToList() ?? new List<CalendarEvent>(); // Get all events from the database
            return View(events);
        }

        [HttpPost]
        public IActionResult AddEvent([FromBody] CalendarEvent eventData)
        {
            if (eventData == null)
            {
                return BadRequest("Invalid event data.");
            }

            if (eventData.Id == 0)
            {
                eventData.Date = eventData.Date.AddDays(1); // Adjust the date increment if needed
                _context.CalendarEvents.Add(eventData);
            }
            else
            {
                var existingEvent = _context.CalendarEvents.Find(eventData.Id);
                if (existingEvent != null)
                {
                    existingEvent.EventName = eventData.EventName;
                    eventData.Date = eventData.Date.AddDays(1); // Adjust the date increment if needed
                    _context.CalendarEvents.Update(existingEvent);
                }
                else
                {
                    return NotFound("Event not found.");
                }
            }

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteEvent(int id)
        {
            var existingEvent = _context.CalendarEvents.Find(id);
            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            _context.CalendarEvents.Remove(existingEvent);
            _context.SaveChanges();
            return Ok(); // Or return some data if needed
        }
    }

}
