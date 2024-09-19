using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string ConutryName { get; set; }
    }
}
