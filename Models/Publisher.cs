using System.ComponentModel.DataAnnotations;

namespace Book_kharido.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        public string PublisherName { get; set; }
    }
}
