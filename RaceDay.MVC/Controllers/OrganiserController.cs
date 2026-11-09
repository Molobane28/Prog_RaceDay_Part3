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

        public async Task<IActionResult> Dashboard()
        {
            var events = await _apiService.GetAsync<List<EventViewModel>>("api/events");

            var html = new StringBuilder();
            html.Append("<div class='d-flex justify-content-between align-items-center mb-4'>");
            html.Append("<h2>My Events</h2>");
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
    }
}