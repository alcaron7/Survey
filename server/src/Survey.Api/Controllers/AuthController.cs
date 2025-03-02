using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Survey.Application.DTOs.Login;
using Survey.Application.DTOs.User;
using Survey.Application.Interfaces;
using Survey.Core.Entities;
using System.Security.Claims;

namespace Survey.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticate a user with email and password
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginReplyDTO>> Login(LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var response = await _authService.LoginAsync(loginRequestDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Refresh the access token using a refresh token
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO refreshTokenRequest)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(refreshTokenRequest);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Revoke a refresh token (logout)
        /// </summary>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout(RevokeTokenRequestDTO revokeTokenRequest)
        {
            var result = await _authService.RevokeTokenAsync(revokeTokenRequest);
            if (!result)
                return BadRequest(new { message = "Invalid token" });

            return Ok(new { message = "Logged out successfully" });
        }

        /// <summary>
        /// Revoke all refresh tokens for the current user (logout from all devices)
        /// </summary>
        [Authorize]
        [HttpPost("logout-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LogoutAll()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _authService.RevokeAllUserTokensAsync(userId);
            return Ok(new { message = "Logged out from all devices" });
        }

        /// <summary>
        /// Validate a token
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TokenValidationResponseDTO>> ValidateToken([FromBody] string token)
        {
            var result = await _authService.ValidateTokenAsync(token);
            return Ok(result);
        }
    }
}
