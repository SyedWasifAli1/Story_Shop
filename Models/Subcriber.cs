using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
    public class Subcriber
    {
        [Key]
        public int SubId { get; set; }
        [Required]
        public string SubName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
