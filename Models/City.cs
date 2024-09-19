using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string CityName { get; set; }
    }
}
