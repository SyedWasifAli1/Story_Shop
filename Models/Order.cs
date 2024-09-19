using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    public class Order { 


        [Key]
        public int OrderId { get; set; }
        [Required]
        public string OrderCode { get; set; }

        [Required]
        public int Type { get; set; }
        [Required]
        public int Total { get; set; }
        [Required]
        public int Quantity { get; set; }

        public int Id { get; set; }
        [ForeignKey("Id")]
        public User User { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public DateTime Ordertime { get; set; }

    }
}
