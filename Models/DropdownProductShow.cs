using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    [NotMapped]
    public class DropdownProductShow
    {

        public List<Product> ProductList { get; set; }

        public List<SelectListItem> PublisherName { get; set; }
        public List<SelectListItem> ProductCategoryName { get; set; }
        public List<SelectListItem> WriteName { get; set; }
        public List<SelectListItem> CountryName { get; set; }
        public List<SelectListItem> CityName { get; set; }
        public List<SelectListItem> TownName { get; set; }
    }
}
