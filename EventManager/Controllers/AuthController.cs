using EventManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;

		public AuthController(
			SignInManager<ApplicationUser> signInManager,
			UserManager<ApplicationUser> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginInputModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(new AuthResultDto
				{
					Success = false,
					Message = "Invalid form data."
				});
			}

			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user == null)
			{
				return Unauthorized(new AuthResultDto
				{
					Success = false,
					Message = "Invalid email or password."
				});
			}

			var result = await _signInManager.PasswordSignInAsync(
				user.UserName!,
				model.Password,
				model.RememberMe,
				lockoutOnFailure: false);

			if (!result.Succeeded)
			{
				return Unauthorized(new AuthResultDto
				{
					Success = false,
					Message = "Invalid email or password."
				});
			}

			return Ok(new AuthResultDto
			{
				Success = true,
				Message = "Login successful."
			});
		}
	}
}