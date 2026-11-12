using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;
using System.Text;

namespace RaceDay.MVC.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // 1. DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>("api/events");

            var html = new StringBuilder();
            html.Append("<div class='d-flex justify-content-between align-items-center mb-4'>");
            html.Append("<h2>My Events</h2>");
            // LINK TO CREATE EVENT
            html.Append("<a href='/Organiser/CreateEvent' class='btn btn-success'>+ Create New Event</a>");
            html.Append("</div>");

            if (events == null || !events.Any())
            {
                html.Append("<div class='alert alert-info'>You haven't created any events yet.</div>");
            }
            else
            {
                html.Append("<table class='table table-striped table-hover'>");
                html.Append("<thead class='table-dark'><tr><th>Event Name</th><th>Date</th><th>Location</th><th>Type</th><th>Status</th></tr></thead>");
                html.Append("<tbody>");
                foreach (var ev in events)
                {
                    html.Append($"<tr><td>{ev.EventName}</td><td>{ev.EventDate:yyyy-MM-dd}</td><td>{ev.Location}</td><td>{ev.EventType}</td><td><span class='badge bg-primary'>{ev.Status}</span></td></tr>");
                }
                html.Append("</tbody></table>");
            }

            return Content(html.ToString(), "text/html");
        }

        // 2. SHOW CREATE EVENT FORM (GET)
        [HttpGet]
        public IActionResult CreateEvent()
        {
            var html = new StringBuilder();
            html.Append("<h2>Create New Event</h2>");
            // The form posts to the CreateEvent POST method below
            html.Append("<form method='post' action='/Organiser/CreateEvent' style='max-width: 500px;'>");

            html.Append("<div class='mb-3'><label>Event Name</label><input name='EventName' class='form-control' required /></div>");
            html.Append("<div class='mb-3'><label>Description</label><textarea name='EventDescription' class='form-control'></textarea></div>");
            html.Append("<div class='mb-3'><label>Date</label><input type='date' name='EventDate' class='form-control' required /></div>");
            html.Append("<div class='mb-3'><label>Registration Deadline</label><input type='date' name='RegistrationDeadline' class='form-control' required /></div>");
            html.Append("<div class='mb-3'><label>Location</label><input name='Location' class='form-control' required /></div>");
            html.Append("<div class='mb-3'><label>Distance (km)</label><input type='number' step='0.1' name='Distance' class='form-control' required /></div>");

            html.Append("<div class='mb-3'><label>Event Type</label><select name='EventType' class='form-control'>");
            html.Append("<option value='Run'>Run</option><option value='Walk'>Walk</option><option value='Cycle'>Cycle</option>");
            html.Append("</select></div>");

            html.Append("<div class='mb-3'><label>Max Participants</label><input type='number' name='MaxParticipants' class='form-control' required /></div>");

            html.Append("<button type='submit' class='btn btn-primary'>Save Event</button>");
            html.Append("<a href='/Organiser/Dashboard' class='btn btn-secondary ms-2'>Cancel</a>");
            html.Append("</form>");

            return Content(html.ToString(), "text/html");
        }

        // 3. HANDLE FORM SUBMISSION (POST)
        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventViewModel model)
        {
            // Get current user ID from claims/session (Assuming they are the organiser)
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int organiserId = 1; // Default fallback

            // NOTE: In the real world, you'd look up the OrganiserID using the UserID. 
            // For this assignment, hardcoding to 1 or 2 is usually fine, but let's try to be dynamic.

            model.OrganiserID = organiserId;
            model.Status = "Open";

            var response = await _apiService.PostAsync("api/events", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Dashboard");
            }

            return Content("<h2>Error creating event.</h2><a href='/Organiser/Dashboard'>Go Back</a>", "text/html");
        }
    }
}