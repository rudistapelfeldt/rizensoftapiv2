using RizenSoftApiV2.Models;

namespace RizenSoftApiV2.Interfaces
{
    public interface ITokenService
	{
		string CreateToken(User user);
	}
}

