using Microsoft.EntityFrameworkCore;
using RizenSoftApiV2.Models;

namespace RizenSoftApiV2.Domains
{
    public class AddressDomain
    {
        RizenSoftDBContext _context;

        public AddressDomain(RizenSoftDBContext context)
        {
            _context = context;
        }

        public async Task<AddressResponse> DeleteAddress(int id)
        {
            var result = await _context.Address.FindAsync(id);
            if (result == null)
                return new AddressResponse
                {
                    Success = false,
                    Error = "Record not found",
                    ErrorCode = "P0002"
                };
            else
            {
                _context.Address.Remove(result);
                await _context.SaveChangesAsync();
                return new AddressResponse
                {
                    Success = true
                };
            }

        }

        public async Task<AddressResponse> DeleteAll()
        {
            if (_context.Address.Any())
            {
                var response = await GetAll();
                if (response.Addresses != null)
                    _context.Address.RemoveRange(response.Addresses);
                await _context.SaveChangesAsync();
                return new AddressResponse
                {
                    Success = true
                };
            }
            else
                return new AddressResponse
                {
                    Success = false,
                    Error = "No records to delete",
                    ErrorCode = "P0002"
                };
        }

        public async Task<AddressResponse> GetAll()
        {
            var records = await _context.Address.ToListAsync();
            if (records == null || !records.Any())
            {
                return new AddressResponse
                {
                    Success = false,
                    Error = "No records found",
                    ErrorCode = "P0002"
                };
            }
            else
            {
                return new AddressResponse
                {
                    Success = true,
                    Addresses = records
                };
            }
        }

        public async Task<AddressResponse> GetById(int id)
        {
            var records = await _context.Address.Where(x => x.Id == id).ToListAsync();
            if (records == null)
            {
                return new AddressResponse
                {
                    Success = false,
                    Error = "No record found",
                    ErrorCode = "P0002"
                };
            }
            else
                return new AddressResponse
                {
                    Success = true,
                    Addresses = records
                };
        }

        public async Task<AddressResponse> Insert(Address address)
        {
            var existing = await _context.Address.Where(x => x.AddressId == address.AddressId).FirstOrDefaultAsync();
            if (existing != null)
                return new AddressResponse
                {
                    Success = false,
                    Error = "This address already exists",
                    ErrorCode = "23505"
                };
            else
                _context.Address.Add(address);

            var result = await _context.SaveChangesAsync();
            if (result >= 0)
                return new AddressResponse
                {
                    Success = true
                };
            else
                return new AddressResponse
                {
                    Success = false,
                    Error = "No records were inserted",
                    ErrorCode = "23505"
                };
        }

        public async Task<AddressResponse> Update(Address address)
        {
            var existing = await _context.Address.FindAsync(address.Id);

            if (existing != null)
            {
                existing.AddressId = address.AddressId;
                existing.AddressLine1 = address.AddressLine1;
                existing.AddressLine2 = address.AddressLine2;
                existing.City = address.City;
                existing.Country = address.Country;
                existing.Province = address.Province;
                existing.Suburb = address.Suburb;
                existing.PostalCode = address.PostalCode;

                var result = await _context.SaveChangesAsync();
                if (result >= 0)
                    return new AddressResponse
                    {
                        Success = true
                    };
                else
                    return new AddressResponse
                    {
                        Success = false,
                        Error = "Record was not updated",
                        ErrorCode = "404"
                    };
            }
            else
                return new AddressResponse
                {
                    Success = false,
                    Error = "No record to update",
                    ErrorCode = "P0001"
                };
        }
    }
}

