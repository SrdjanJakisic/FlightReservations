using FlightReservations.Hubs;
using FlightReservations.Models;
using FlightReservations.Services;
using FlightReservations.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FlightReservations.Controllers
{
    [Authorize]
    public class FlightReservationsController : Controller
    {
        private readonly FlightService _flightService;
        private readonly ReservationService _reservationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<ReservationHub> _hub;

        public FlightReservationsController(FlightService flightService,
            ReservationService reservationService,
            UserManager<ApplicationUser> userManager,
            IHubContext<ReservationHub> hub)
        {
            _flightService = flightService;
            _reservationService = reservationService;
            _userManager = userManager;
            _hub = hub;
        }

        [Authorize(Roles = "Agent, Admin")]
        public async Task<IActionResult> Index()
        {
            var flights = await _flightService.GetAllFlightsAsync();

            var items = new List<FlightListItemViewModel>();
            foreach(var flight in flights)
            {
                items.Add(new FlightListItemViewModel
                {
                    Flight = flight,
                    AvailableSeats = await _flightService.GetAvailableSeatsAsync(flight.Id)
                });
            }

            return View(items);
        }

        [Authorize(Roles = "Agent")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Roles = "Agent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFlightViewModel model)
        {
            if (model.Origin == model.Destination)
                ModelState.AddModelError(string.Empty, "Origin and destination must be different.");
            if (!ModelState.IsValid) return View(model);

            var flight = new Flight
            {
                Origin = model.Origin,
                Destination = model.Destination,
                DepartureDate = model.DepartureDate,
                NumberOfStops = model.NumberOfStops,
                TotalSeats = model.TotalSeats,
                Status = FlightStatus.Active
            };

            await _flightService.CreateFlightAsync(flight);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _flightService.CancelFlightAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Visitor")]
        [HttpGet]
        public IActionResult Search() => View(new SearchFlightsViewModel());

        [Authorize(Roles = "Visitor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchFlightsViewModel model)
        {
            var flights = await _flightService.SearchAvailableFlightsAsync(
                model.Origin, model.Destination, model.DirectOnly);

            model.Results = new List<FlightListItemViewModel>();
            foreach(var flight in flights)
            {
                model.Results.Add(new FlightListItemViewModel
                {
                    Flight = flight,
                    AvailableSeats = await _flightService.GetAvailableSeatsAsync(flight.Id)
                });
            }

            return View(model);
        }

        [Authorize(Roles = "Visitor")]
        [HttpGet]
        public async Task<IActionResult> Reserve(int flightId)
        {
            var model = new CreateReservationViewModel
            {
                FlightId = flightId,
                AvailableSeats = await _flightService.GetAvailableSeatsAsync(flightId)
            };

            return View(model);
        }

        [Authorize(Roles = "Visitor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reserve(CreateReservationViewModel model)
        {
            if(!ModelState.IsValid)
            {
                model.AvailableSeats = await _flightService.GetAvailableSeatsAsync(model.FlightId);
                return View(model);
            }

            var visitorId = _userManager.GetUserId(User);
            var (success, error) = await _reservationService.CreateReservationAsync(
                model.FlightId, visitorId!, model.NumberOfSeats);

            if(!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Reservation failed.");
                model.AvailableSeats = await _flightService.GetAvailableSeatsAsync(model.FlightId);
                return View(model);
            }

            await _hub.Clients.Group("Agents").SendAsync("ReservationCreated");

            return RedirectToAction(nameof(MyReservations));
        }

        [Authorize(Roles = "Visitor")]
        public async Task<IActionResult> MyReservations()
        {
            var visitorId = _userManager.GetUserId(User);
            var reservations = await _reservationService.GetReservationForVisitorAsync(visitorId!);
            return View(reservations);
        }

        [Authorize(Roles = "Agent")]
        public async Task<IActionResult> Pending()
        {
            var reservations = await _reservationService.GetPendingReservationsAsync();
            return View(reservations);
        }

        [Authorize(Roles = "Agent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var agentId = _userManager.GetUserId(User)!;
            var visitorId = await _reservationService.ApproveReservationAsync(id, agentId);

            if (visitorId != null)
                await _hub.Clients.User(visitorId).SendAsync("ReservationApproved", id);

            return RedirectToAction(nameof(Pending));
        }
    }
}
