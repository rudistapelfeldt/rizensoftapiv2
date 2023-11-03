using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RizenSoftApiV2.Domains;
using RizenSoftApiV2.Models;

namespace RizenSoftApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController : ControllerBase
	{
        AddressDomain _addressDomain;

        private readonly ILogger<AddressController> _logger;

        public AddressController(AddressDomain addressDomain, ILogger<AddressController> logger)
        {
            _addressDomain = addressDomain;
            _logger = logger;
        }

        [Authorize]
        [HttpDelete("/address/delete/{id}")]
        public async Task<AddressResponse> DeleteAddress(int id)
        {
            return await _addressDomain.DeleteAddress(id);
        }

        [Authorize]
        [HttpGet("/address/get")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<AddressResponse> GetAll()
        {
            return await _addressDomain.GetAll();
        }

        [Authorize]
        [HttpGet("/address/get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<AddressResponse> GetById(int id)
        {
            return await _addressDomain.GetById(id);
        }

        [Authorize]
        [HttpGet("/address/insert")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<AddressResponse> Insert([FromBody]Address address)
        {
            return await _addressDomain.Insert(address);
        }

        [Authorize]
        [HttpGet("/address/update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddressResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<AddressResponse> Update([FromBody] Address address)
        {
            return await _addressDomain.Update(address);
        }
    }
}

