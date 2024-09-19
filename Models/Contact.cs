using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    public class Contact
    {
        [Key]
        public int ContactId { get; set; }
        [NotMapped]
        public string? Name { get; set; } 
        [Required]
        public string? OrderCode { get; set; }
        [Required(ErrorMessage = "Answer is required")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 20 and 1000 characters")]
        public string? Message { get; set; }
        public int FAQsId { get; set; }
        [ForeignKey("FAQsId")]
        public FAQ FAQ { get; set; }
        public int Id { get; set; }
        [ForeignKey("Id")]
        public User User { get; set; }


        [Required]  
        public int Type { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public DateTime Ordertime { get; set; }


    }
}
