using System;

namespace RizenSoftApiV2.Models
{
    public partial class RefreshToken
    {
        public int Id { get; set; }


        public int UserId { get; set; }


        public string TokenHash { get; set; }


        public string TokenSalt { get; set; }


        public DateTime Ts { get; set; }


        public DateTime ExpiryDate { get; set; }

    }
}

