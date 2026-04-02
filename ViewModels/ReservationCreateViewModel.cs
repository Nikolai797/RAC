using RentACar.Data.Models;

namespace RentACar.Web1.ViewModels
{
    public class ReservationCreateViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Car> AvailableCars { get; set; } = new();
    }
}