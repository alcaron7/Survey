using Survey.Application.DTOs.User;
using Survey.Core.Entities;
using Survey.Core.Interfaces;
using Survey.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Survey.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(CreateUserDTO createUserDto)
        {
            User user = await _userService.CreateUser(createUserDto);
            return StatusCode(201);
        }
    }
}
