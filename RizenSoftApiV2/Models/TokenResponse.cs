namespace RizenSoftApiV2.Models
{
    public class TokenResponse : BaseResponse
	{
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}

