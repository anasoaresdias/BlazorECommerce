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
        public async Task<ActionResult<ServiceResponse<int>>> Register(UserViewModel userRegister)
        {
            var response = await authenticationServices.Register(userRegister);
            if(!response.Success)
                return BadRequest(response);
            return Ok(response);
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
