using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
   
    public class TownsAdress
    {
        [Key]
        public int TownsAId { get; set; }

        [Required]
        public string? TownName { get; set; }
        [Required]
        public double KoloMeter { get; set; }
        [Required]
        public int DeliveryFee { get; set; }

        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City? City { get; set; }
        public int CountryId { get; set; }
        [ForeignKey("CountryId")]
        public Country? Country { get; set; }

    }
}
