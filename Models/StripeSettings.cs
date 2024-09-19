using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentMethodStripe.Models
{
	[NotMapped]
	public class StripeSettings
	{
		public string SecretKey { get; set; }
		public string PublicKey { get; set; }
	}
}
