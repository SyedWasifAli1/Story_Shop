using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace Book_kharido.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public int ProductCode { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string ProductIamge { get; set; }
        [NotMapped]
        public IFormFile? ProductFile { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ProductDescription { get; set; }

        [Required]
        public int ProductPrice { get; set; }
        [Required]
        public int Page { get; set; }
        [Required]
        public int ProductQuantity { get; set;}
        [Required]
        public int ProductDiscount { get; set; }
        [Required]
        public int PublisherID { get; set; }
        [ForeignKey("PublisherID")]
        public Publisher Publisher { get; set; }
        public int WriterID { get; set; }
        [ForeignKey("WriterID")]
        public Writer Writer { get; set; }
        public int ProductCategoryId { get; set; }
        [ForeignKey("ProductCategoryId")]
        public ProductCategory ProductCategory { get; set; }


        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public DateTime CreatedAt { get; set; }

    


    }
}
