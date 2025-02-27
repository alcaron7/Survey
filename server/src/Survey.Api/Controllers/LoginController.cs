using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey.Application.DTOs.Login;
using Survey.Application.DTOs.User;
using Survey.Application.Interfaces;
using Survey.Core.Entities;

namespace Survey.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<LoginReplyDTO>> LoginUser(LoginRequestDTO loginRequestDTO)
        {
            var response = await _authService.LoginAsync(loginRequestDTO);
            return Ok(response);
        }

        [HttpPost("createHash")]
        public async Task<ActionResult<string>> CreateHash(string request)
        {
            var response = await _authService.CreateHashSync(request);
            return Ok(response);
        }
    }
}
