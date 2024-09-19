using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_kharido.Models
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }
        [Required]
       public string BillCode { get; set;}
        [Required]
       public int Status { get; set;}

        public int Id { get; set; }
        [ForeignKey("Id")]
        public User User { get; set; }

        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}")]
        public DateTime Ordertime { get; set; }

    }
}
