using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
    public class Writer
    {
        [Key]
        public int WritetId { get; set; }
        public string WriteName { get; set; }
    }
}
