using System.ComponentModel.DataAnnotations;


namespace RizenSoftApiV2.Models
{
    public partial class Address : ModelBase
	{
        [Required]
        public string AddressId { get; set; }

        [Required]
		public string AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

		[Required]
        public string Suburb { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string PostalCode { get; set; }
    }
}

