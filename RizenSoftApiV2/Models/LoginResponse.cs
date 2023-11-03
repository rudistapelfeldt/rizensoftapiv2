
namespace RizenSoftApiV2.Models
{
    public class LoginResponse : BaseResponse
	{
        public User User { get; set; }

        public string AccessToken { get; set; }
    }
}

