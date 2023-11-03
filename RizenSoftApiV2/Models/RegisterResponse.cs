namespace RizenSoftApiV2.Models
{
    public class RegisterResponse : BaseResponse
	{
        public User User { get; set; }

        public string? AccessToken { get; set; }
    }
}

