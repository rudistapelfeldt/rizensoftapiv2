using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Webapi.Domains;
using Webapi.Models;

namespace RizenSoftApiV2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseApiController
	{
		UserDomain _userDomain;

		public UserController(UserDomain userDomain)
		{
			_userDomain = userDomain;
		}

        [Authorize]
        [HttpDelete("/user/delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UserResponse> DeleteUser(int id)
        {
            return await _userDomain.DeleteUser(id);
        }

        [Authorize]
        [HttpGet("/user/get")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UserResponse> GetAll()
		{
			return await _userDomain.GetAll();
		}

        [Authorize]
        [HttpGet("/user/get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UserResponse> GetById(int id)
        {
            return await _userDomain.GetById(id);
        }

        [Authorize]
        [HttpGet("/user/insert")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UserResponse> Insert([FromBody]UserRequest request)
        {
            return await _userDomain.Insert(request);
        }

        [Authorize]
        [HttpGet("/user/update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<UserResponse> Update([FromBody]User user)
        {
            return await _userDomain.Update(user);
        }
    }
}

