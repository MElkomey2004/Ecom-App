using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.Entites;
using Ecom.Core.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.API.Controllers
{
	public class AccountController : BaseController
	{
		public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
		{
		}

		[HttpGet("get-address-for-user")]
		public async Task<IActionResult> getAddress()
		{
			var address = await work.Auth.getUserAddress(User.FindFirst(ClaimTypes.Email).Value);
			var result = mapper.Map<ShipAddressDTO>(address);
			return Ok(result);
		}


		[HttpPut("update-address")]
		public async Task<IActionResult> updateAddress(ShipAddressDTO addressDTO)
		{
			var email = User.FindFirst(ClaimTypes.Email)?.Value;
			var address = mapper.Map<Adress>(addressDTO);
			var result = await work.Auth.UpdateAddress(email, address);
			return result ? Ok() : BadRequest();
		}

		[HttpPost("Register")]
		public async Task<IActionResult> Register(RegisterDTO registerDTO)
		{
			string result = await work.Auth.RegisterAsync(registerDTO);
			if (result != "done")
			{
				return BadRequest(new ResponseAPI(400, result));
			}
			return Ok(new ResponseAPI(200, "Registration successful, please check your email to confirm your account."));
		}

		[HttpPost("Login")]
		public async Task<IActionResult> Login(LoginDTO loginDTO)
		{
			string result = await work.Auth.LoginAsync(loginDTO);
			if (result.ToLower().StartsWith("please"))
			{
				return BadRequest(new ResponseAPI(400, result));
			}

			Response.Cookies.Append("token", result, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None,
				IsEssential = true,
				Domain = "localhost",
				Expires = DateTime.Now.AddDays(1)
			});

			return Ok(new ResponseAPI(200, "Login successful"));
		}

		// ✅ POST version (for Swagger / API calls)
		[HttpPost("active-account")]
		public async Task<IActionResult> ActivePost([FromBody] ActiveAccountDTO accountDTO)
		{
			var result = await work.Auth.ActiveAccount(accountDTO);
			return result
				? Ok(new ResponseAPI(200, "Account activated successfully"))
				: BadRequest(new ResponseAPI(400, "Invalid activation link"));
		}

		// ✅ GET version (for Email link clicks)
		[HttpGet("active-account")]
		public async Task<IActionResult> ActiveGet(string email, string token)
		{
			var dto = new ActiveAccountDTO
			{
				Email = email,
				Token = token
			};

			var result = await work.Auth.ActiveAccount(dto);
			return result
				? Ok(new ResponseAPI(200, "Account activated successfully"))
				: BadRequest(new ResponseAPI(400, "Invalid activation link"));
		}

		[HttpGet("send-email-forget-password")]
		public async Task<IActionResult> ForgetPassword(string email)
		{
			var result = await work.Auth.SendEmailForForgetPassword(email);
			return result
				? Ok(new ResponseAPI(200, "Password reset email sent"))
				: BadRequest(new ResponseAPI(400, "User not found"));
		}
	}
}
