using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorECommerce.Shared;

namespace BlazorECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServices authenticationServices;

        public AuthenticationController(IAuthenticationServices authenticationServices)
        {
            this.authenticationServices = authenticationServices;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<User>>> Register(UserViewModel userRegister)
        {
            var response = await authenticationServices.Register(userRegister);
            if(response.Data == null)
                return BadRequest(response.Message);
            return Ok(response.Message);
        }

        [HttpPost("login")]
        public async Task<string> Login(UserViewModel userlogin)
        {
            var str = await authenticationServices.Login(userlogin);
            if (str == null)
                return str;
            return str;
        }
    }
}
