using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
    public class FAQ
    {
        [Key]
        public int FAQsId { get; set; }
        [Required(ErrorMessage = "Question is required")]
    [StringLength(50, MinimumLength = 10, ErrorMessage = "Question must be between 10 and 50 characters")]
        public string? Question { get; set; }

        [Required(ErrorMessage = "Answer is required")]
        [StringLength(1000, MinimumLength = 20, ErrorMessage = "Answer must be between 20 and 1000 characters")]
        public string? Answer { get; set; }


    }
}
