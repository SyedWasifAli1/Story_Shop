using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    public class Feed
    {
        [Key]
        public int FeedId { get; set; }
        public string FeedbackMessages { get; set; }
        public int Id { get; set; }
        [ForeignKey("Id")]
        public User User { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public int ProductRating { get; set; }

    }
}
