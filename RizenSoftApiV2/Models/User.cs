using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RizenSoftApiV2.Models
{
    public partial class User : ModelBase
    {
        public User() => RefreshTokens = new HashSet<RefreshToken>();

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string DateOfBirth { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        public string PasswordSalt { get; set; }

        public bool Active { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

        [Required]
        public string IdNumber { get; set; }

        [Required]
        public string AddressId { get; set; }
    }
}

