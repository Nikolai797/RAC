using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentACar.Data.Models;
using RentACar.Services.Interfaces;
using RentACar.Web1.ViewModels;

namespace RentACar.Web1.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly ICarService _carService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReservationsController(IReservationService reservationService, ICarService carService, UserManager<ApplicationUser> userManager)
        {
            _reservationService = reservationService;
            _carService = carService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var reservations = await _reservationService.GetReservationsByUserAsync(userId);
            return View(reservations);
        }

        public IActionResult Create()
        {
            var model = new ReservationCreateViewModel
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3)
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetAvailableCars(ReservationCreateViewModel model)
        {
            if (model.StartDate >= model.EndDate)
            {
                ModelState.AddModelError("", "End date must be after start date.");
                return View("Create", model);
            }

            model.AvailableCars = (await _carService.GetAvailableCarsAsync(model.StartDate, model.EndDate)).ToList();
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReservation(int carId, DateTime startDate, DateTime endDate)
        {
            var userId = _userManager.GetUserId(User);
            var reservation = new Reservation
            {
                CarId = carId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate
            };

            try
            {
                await _reservationService.CreateReservationAsync(reservation);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                var model = new ReservationCreateViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    AvailableCars = (await _carService.GetAvailableCarsAsync(startDate, endDate)).ToList()
                };
                return View("Create", model);
            }
        }
    }
}