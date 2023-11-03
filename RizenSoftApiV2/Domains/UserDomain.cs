using Microsoft.EntityFrameworkCore;
using RizenSoftApiV2.Helper;
using RizenSoftApiV2.Models;

namespace RizenSoftApiV2.Domains
{
    public class UserDomain
    {
        RizenSoftDBContext _context;

        TokenDomain _tokenDomain;

        public UserDomain(RizenSoftDBContext context, TokenDomain tokenDomain)
        {
            _context = context;
            _tokenDomain = tokenDomain;
        }

        public async Task<UserResponse> DeleteUser(int id)
        {
            var result = await _context.User.FindAsync(id);
            if (result == null)
                return new UserResponse
                {
                    Success = false,
                    Error = "User does not exist",
                    ErrorCode = "P0002"
                };
            else
            {
                _context.User.Remove(result);
                await _context.SaveChangesAsync();

                return new UserResponse
                {
                    Success = true
                };
            }
        }

        public async Task<UserResponse> GetAll()
        {
            var records = await _context.User.ToListAsync();
            if (records == null || !records.Any())
            {
                return new UserResponse
                {
                    Success = false,
                    Error = "No records found",
                    ErrorCode = "P0002"
                };
            }
            else
            {
                return new UserResponse
                {
                    Success = true,
                    User = records
                };
            }
        }

        public async Task<UserResponse> Search(string userName, string password)
        {
            var existing = await _context.User.Where(x => x.EmailAddress == userName && x.ConfirmPassword == password).ToListAsync();
            if (existing != null)
                return new UserResponse
                {
                    Success = true,
                    User = existing
                };
            else
                return new UserResponse
                {
                    Success = false,
                    Error = "User does not exist",
                    ErrorCode = "P0002"
                };
        }

        public async Task<UserResponse> GetById(int id)
        {
            var existing = await _context.User.Where(x => x.Id == id).ToListAsync();
            if (existing != null)
                return new UserResponse
                {
                    Success = true,
                    User = existing
                };
            else
                return new UserResponse
                {
                    Success = false,
                    Error = "User does not exist",
                    ErrorCode = "P0002"
                };
        }

        public async Task<UserResponse> Insert(UserRequest request)
        {
            var existing = await _context.User.Where(x => x.EmailAddress == request.User.EmailAddress).FirstOrDefaultAsync();
            if (existing == null)
            {
                var salt = PasswordEncoder.GetSecureSalt();

                var passwordHash = PasswordEncoder.HashUsingPbkdf2(request.User.Password, salt);
                request.User.Password = passwordHash;
                request.User.PasswordSalt = Convert.ToBase64String(salt);

                _context.User.Add(request.User);
                var result = await _context.SaveChangesAsync();
                var all = await GetAll();
                if (all.User != null)
                    all.User.Add(request.User);
                else
                    all.User = new List<User>();
                if (result >= 0)
                    return new UserResponse
                    {
                        Success = true,
                        User = all.User
                    };
            }

            return new UserResponse
            {
                Success = false,
                Error = "A user with this email address exists",
                ErrorCode = "23505"
            };
        }

        public async Task<UserResponse> Update(User user)
        {
            var existing = await _context.User.FindAsync(user.Id);
            if (existing != null)
            {
                var salt = PasswordEncoder.GetSecureSalt();

                var passwordHash = PasswordEncoder.HashUsingPbkdf2(user.Password, salt);
                existing.Password = passwordHash;
                existing.PasswordSalt = Convert.ToBase64String(salt);
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                existing.IdNumber = user.IdNumber;
                existing.Active = user.Active;
                existing.AddressId = user.AddressId;
                existing.ConfirmPassword = user.ConfirmPassword;
                existing.UserName = user.UserName;
                existing.RefreshTokens = user.RefreshTokens;
                existing.Gender = user.Gender;
                existing.EmailAddress = user.EmailAddress;
                existing.DateOfBirth = user.DateOfBirth;

                var result = await _context.SaveChangesAsync();
                var all = await GetAll();

                if (result >= 0)
                    return new UserResponse
                    {
                        Success = true,
                        User = all.User
                    };
                else
                    return new UserResponse
                    {
                        Success = false,
                        Error = "Error while updating user. Please try again later",
                        ErrorCode = "0"
                    };
            }
            else
                return new UserResponse
                {
                    Success = false,
                    Error = "Record does not exist",
                    ErrorCode = "P0002"
                };
        }
    }
}

