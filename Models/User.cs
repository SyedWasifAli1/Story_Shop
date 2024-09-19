using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DefaultValue(null)]
        public string Address { get; set; }

        
        [Required]
        [DefaultValue(0)]
        public int PhoneNumber { get; set; }
        [Required]
        [DefaultValue(0)]
        public int CountryId { get; set; }
        [ForeignKey("CountryId")]
        public Country? Country { get; set; }
        [Required]
        [DefaultValue(0)]
        public int TownsAId { get; set; }
        [ForeignKey("TownsAId")]
        public TownsAdress? TownsAdress { get; set; }

        [DefaultValue(0)]

        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City? City { get; set; }


        [Required]
        public string? Roll { get; set; }
        [Required]
        public string Picture { get; set; }     
        public int OTP { get; set; }
        [NotMapped]
        public FormFile? FormFile { get; set; }  
        [NotMapped]
        public int Amount { get; set; }

    }
}
