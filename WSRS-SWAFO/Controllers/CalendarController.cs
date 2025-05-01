using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WSRS_SWAFO.Controllers
{
    public class CalendarController : Controller
    {
        // For demonstration: using static list (replace with database in real apps)
        private static List<CalendarEvent> _events = new List<CalendarEvent>();

        public IActionResult CalendarView()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            return Json(_events);
        }

        [HttpPost]
        public IActionResult SaveEvent([FromBody] CalendarEvent newEvent)
        {
            newEvent.Id = Guid.NewGuid().ToString(); // Generate new ID
            _events.Add(newEvent);
            return Json(newEvent);
        }

        [HttpPost]
        public IActionResult DeleteEvent([FromBody] EventDeleteModel model)
        {
            var eventToRemove = _events.FirstOrDefault(e => e.Id == model.Id);
            if (eventToRemove != null)
            {
                _events.Remove(eventToRemove);
                return Json(true);
            }
            return Json(false);
        }
    }

    public class CalendarEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Start { get; set; } // ISO format date string
        public string End { get; set; }   // ISO format date string
    }

    public class EventDeleteModel
    {
        public string Id { get; set; }
    }
}
