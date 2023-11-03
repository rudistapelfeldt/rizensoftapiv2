using Microsoft.EntityFrameworkCore;
using RizenSoftApiV2.Helper;
using RizenSoftApiV2.Interfaces;
using RizenSoftApiV2.Models;

namespace RizenSoftApiV2.Domains
{
    public class TokenDomain
	{
        RizenSoftDBContext _context;

        ITokenService _tokenService;

        public TokenDomain(RizenSoftDBContext context, ITokenService tokenService)
		{
			_context = context;
            _tokenService = tokenService;
		}

        public async Task<Tuple<string, string>?> GenerateTokensAsync(int userId)
        {
            var userRecord = await _context.User.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == userId);
            if (userRecord == null)
                return null;
            else
            {
                var accessToken = _tokenService.CreateToken(userRecord);

                var refreshToken = await TokenHelper.GenerateRefreshToken();

                var salt = PasswordEncoder.GetSecureSalt();

                var refreshTokenHashed = PasswordEncoder.HashUsingPbkdf2(refreshToken, salt);

                if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
                    await RemoveRefreshTokenAsync(userRecord);

                userRecord.RefreshTokens?.Add(new RefreshToken
                {
                    ExpiryDate = DateTime.Now.AddDays(1).ToUniversalTime(),

                    Ts = DateTime.Now.ToUniversalTime(),

                    UserId = userId,

                    TokenHash = refreshTokenHashed,

                    TokenSalt = Convert.ToBase64String(salt)
                });

                await _context.SaveChangesAsync();

                var token = new Tuple<string, string>(accessToken, refreshToken);

                return token;
            }
        }

        public async Task<bool> RemoveRefreshTokenAsync(User user)
        {
            var userRecord = await _context.User.Include(o => o.RefreshTokens).FirstOrDefaultAsync(e => e.Id == user.Id);

            if (userRecord == null)
                return false;

            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                var currentRefreshToken = userRecord.RefreshTokens.First();

                _context.RefreshTokens.Remove(currentRefreshToken);
            }

            return false;
        }


        public async Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(o => o.UserId == refreshTokenRequest.UserId);

            var response = new ValidateRefreshTokenResponse();

            if (refreshToken == null)
            {
                response.Success = false;

                response.Error = "Invalid session or user is already logged out";

                response.ErrorCode = "R02";

                return response;
            }

            var refreshTokenToValidateHash = PasswordEncoder.HashUsingPbkdf2(refreshTokenRequest.RefreshToken, Convert.FromBase64String(refreshToken.TokenSalt));

            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                response.Success = false;

                response.Error = "Invalid refresh token";

                response.ErrorCode = "R03";

                return response;
            }

            if (refreshToken.ExpiryDate < DateTime.Now.ToUniversalTime())
            {
                response.Success = false;

                response.Error = "Refresh token has expired";

                response.ErrorCode = "R04";

                return response;
            }

            response.Success = true;

            response.UserId = refreshToken.UserId;

            return response;
        }
    }
}

