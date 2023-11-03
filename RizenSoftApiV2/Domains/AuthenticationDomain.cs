using Microsoft.EntityFrameworkCore;
using RizenSoftApiV2.Interfaces;
using RizenSoftApiV2.Models;
using RizenSoftApiV2.Helper;

namespace RizenSoftApiV2.Domains
{
    public class AuthenticationDomain
    {
        RizenSoftDBContext _context;

        ITokenService _tokenService;

        public AuthenticationDomain(RizenSoftDBContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<RegisterResponse> Register(RegistrationRequest request)
        {
            try
            {
                var existingUser = await _context.User.SingleOrDefaultAsync(user => user.EmailAddress == request.User.EmailAddress);
                if (existingUser != null)
                    return new RegisterResponse
                    {
                        Success = false,

                        Error = "User already exists with the same email",

                        ErrorCode = "S02"
                    };
                if (request.User.Password != request.User.ConfirmPassword)
                    return new RegisterResponse
                    {
                        Success = false,

                        Error = "Password and confirm password do not match",

                        ErrorCode = "S03"
                    };
                if (request.User.Password.Length <= 7) 
                    return new RegisterResponse
                    {
                        Success = false,

                        Error = "Password is weak",

                        ErrorCode = "S04"
                    };

                var sharedAddressGuid = Guid.NewGuid().ToString();

                request.User.AddressId = sharedAddressGuid;

                var salt = PasswordEncoder.GetSecureSalt();

                var passwordHash = PasswordEncoder.HashUsingPbkdf2(request.User.Password, salt);

                var user = new User
                {
                    FirstName = request.User.FirstName,
                    LastName = request.User.LastName,
                    UserName = request.User.UserName,
                    DateOfBirth = request.User.DateOfBirth,
                    EmailAddress = request.User.EmailAddress,
                    Gender = request.User.Gender,
                    Password = passwordHash,
                    ConfirmPassword = request.User.ConfirmPassword,
                    PasswordSalt = Convert.ToBase64String(salt),
                    IdNumber = request.User.IdNumber,
                    AddressId = request.User.AddressId,
                    Active = true
                };

                var address = new Address
                {
                    AddressId = sharedAddressGuid,
                    AddressLine1 = request.Address.AddressLine1,
                    AddressLine2 = request.Address.AddressLine2,
                    Suburb = request.Address.Suburb,
                    City = request.Address.City,
                    Province = request.Address.Province,
                    Country = request.Address.Country,
                    PostalCode = request.Address.PostalCode
                };

                await _context.Address.AddAsync(address);
                await _context.User.AddAsync(user);
                var result = await _context.SaveChangesAsync();
                if (result >= 0)
                    return new RegisterResponse
                    {
                        Success = true,
                        User = user,
                        AccessToken = _tokenService.CreateToken(user)
                    };
                else
                    return new RegisterResponse
                    {
                        Success = false,
                        Error = "Problem registering. Please try again later",
                        ErrorCode = "500"
                    };
            }
            catch (Exception e)
            {
                return new RegisterResponse
                {
                    Success = false,
                    Error = e.Message,
                    ErrorCode = e.HResult.ToString()
                };
            }
        }

        public LoginResponse LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = _context.User.SingleOrDefault(user => user.Active && user.EmailAddress == loginRequest.EmailAddress);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        Success = false,
                        Error = "Email not found",
                        ErrorCode = "L02"
                    };
                }

                var passwordHash = PasswordEncoder.HashUsingPbkdf2(loginRequest.Password, Convert.FromBase64String(user.PasswordSalt));


                if (user.Password != passwordHash)
                {
                    return new LoginResponse
                    {
                        Success = false,

                        Error = "Invalid Password",

                        ErrorCode = "L03"
                    };
                }
                else
                {
                    var token = _tokenService.CreateToken(user);
                    if (token != null)
                    {
                        return new LoginResponse
                        {
                            Success = true,

                            AccessToken = token,

                            User = user
                        };
                    }
                    else
                        return new LoginResponse
                        {
                            Success = false,

                            Error = "Token not generated",

                            ErrorCode = "401"
                        };
                }
            }
            catch(Exception e)
            {
                return new LoginResponse
                {
                    Success = false,

                    Error = e.Message,

                    ErrorCode = e.HResult.ToString()
                };
            }
        }

        public async Task<LogoutResponse> LogoutAsync(int userId)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(o => o.UserId == userId);

            if (refreshToken == null)
                return new LogoutResponse { Success = true };

            _context.RefreshTokens.Remove(refreshToken);

            var saveResponse = await _context.SaveChangesAsync();

            if (saveResponse >= 0)
                return new LogoutResponse { Success = true };

            return new LogoutResponse { Success = false, Error = "Unable to logout user", ErrorCode = "L04" };
        }
    }
}

