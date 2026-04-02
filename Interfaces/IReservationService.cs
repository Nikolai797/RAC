using RentACar.Data.Models;

namespace RentACar.Services.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<IEnumerable<Reservation>> GetReservationsByUserAsync(string userId);
        Task<Reservation?> GetReservationByIdAsync(int id);
        Task CreateReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
        Task<bool> IsCarAvailableForPeriodAsync(int carId, DateTime startDate, DateTime endDate, int? excludeReservationId = null);
    }
}