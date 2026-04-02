using Microsoft.EntityFrameworkCore;
using RentACar.Data.Data;
using RentACar.Data.Models;
using RentACar.Services.Interfaces;

namespace RentACar.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarService _carService;

        public ReservationService(ApplicationDbContext context, ICarService carService)
        {
            _context = context;
            _carService = carService;
        }

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByUserAsync(string userId)
        {
            return await _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateReservationAsync(Reservation reservation)
        {
            if (!await IsCarAvailableForPeriodAsync(reservation.CarId, reservation.StartDate, reservation.EndDate))
                throw new InvalidOperationException("Car is not available for the selected dates.");

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            var original = await _context.Reservations.AsNoTracking().FirstOrDefaultAsync(r => r.Id == reservation.Id);
            if (original != null && (original.StartDate != reservation.StartDate || original.EndDate != reservation.EndDate))
            {
                if (!await IsCarAvailableForPeriodAsync(reservation.CarId, reservation.StartDate, reservation.EndDate, reservation.Id))
                    throw new InvalidOperationException("Car is not available for the selected dates.");
            }

            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservationAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsCarAvailableForPeriodAsync(int carId, DateTime startDate, DateTime endDate, int? excludeReservationId = null)
        {
            var query = _context.Reservations.Where(r => r.CarId == carId &&
                                                         r.StartDate < endDate &&
                                                         r.EndDate > startDate);
            if (excludeReservationId.HasValue)
                query = query.Where(r => r.Id != excludeReservationId.Value);

            return !await query.AnyAsync();
        }
    }
}