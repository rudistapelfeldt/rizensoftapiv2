using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RizenSoftApiV2.Domains;
using RizenSoftApiV2.Models;

namespace RizenSoftApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : BaseApiController
	{
        AuthenticationDomain _authenticationDomain;

        TokenDomain _tokenDomain;

        public AuthenticationController(AuthenticationDomain authenticationDomain, TokenDomain tokenDomain)
        {
            _authenticationDomain = authenticationDomain;
            _tokenDomain = tokenDomain;
        }

        [HttpPost("/auth/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IActionResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser(RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage)).ToList();

                if (errors.Any())
                    return BadRequest(new TokenResponse
                    {
                        Error = $"{string.Join(",", errors)}",

                        ErrorCode = "S01"
                    });
            }

            var signupResponse = await _authenticationDomain.Register(request);

            if (!signupResponse.Success)
                return UnprocessableEntity(signupResponse);

            return Ok(signupResponse);
        }

        [HttpPost("/auth/login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IActionResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.EmailAddress) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new LoginResponse
                {
                    Error = "Missing login details",

                    ErrorCode = "L01"
                });
            
            var loginResponse = _authenticationDomain.LoginAsync(request);

            if (!loginResponse.Success)
                return Unauthorized(new
                {
                    loginResponse.ErrorCode,

                    loginResponse.Error
                });

            return Ok(loginResponse);
        }

        [HttpPost("/auth/refresh_token")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IActionResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest == null || string.IsNullOrEmpty(refreshTokenRequest.RefreshToken) || refreshTokenRequest.UserId == 0)
                return BadRequest(new TokenResponse
                {
                    Error = "Missing refresh token details",

                    ErrorCode = "R01"
                });

            var validateRefreshTokenResponse = await _tokenDomain.ValidateRefreshTokenAsync(refreshTokenRequest);

            if (!validateRefreshTokenResponse.Success)
                return UnprocessableEntity(validateRefreshTokenResponse);

            var tokenResponse = await _tokenDomain.GenerateTokensAsync(validateRefreshTokenResponse.UserId);

            if (tokenResponse != null)
                return Ok(new { AccessToken = tokenResponse.Item1, Refreshtoken = tokenResponse.Item2 });
            else
                return new StatusCodeResult(404);
        }

        [Authorize]
        [HttpPost("/auth/logout")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IActionResult))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            var logout = await _authenticationDomain.LogoutAsync(UserID);

            if (!logout.Success)
                return UnprocessableEntity(logout);

            return Ok();
        }
    }
}

