using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorECommerce.Shared;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<ActionResult<ServiceResponse<string>>> Login(UserLoginDTO userlogin)
        {
            var response = await authenticationServices.Login(userlogin);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("change-password"), Authorize]
        public async Task<ActionResult<ServiceResponse<bool>>> ChangePassword([FromBody] string newpassword)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await authenticationServices.ChangePassword(int.Parse(userid), newpassword);
            if (!response.Success)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
