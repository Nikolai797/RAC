using System.ComponentModel.DataAnnotations;

namespace RentACar.Data.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; } = string.Empty;

        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        public int Seats { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal PricePerDay { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}