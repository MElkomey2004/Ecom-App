using AutoMapper;
using Ecom.API.Helper;
using Ecom.Core.DTO;
using Ecom.Core.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.API.Controllers
{

	public class AccountController : BaseController
	{
		public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
		{
		}

		[HttpPost("Register")]
		public async Task<IActionResult> register(RegisterDTO registerDTO)
		{
			string result = await work.Auth.RegisterAsync(registerDTO);
			if (result != "done")
			{
				return BadRequest(new ResponseAPI(400, result));
			}
			return Ok(new ResponseAPI(200, result));
		}



		[HttpPost("Login")]
		public async Task<IActionResult> login(LoginDTO loginDTO)
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
			return Ok(new ResponseAPI(200));
		}

		[HttpPost("active-account")]
		public async Task<ActionResult<ActiveAccountDTO>> active(ActiveAccountDTO accountDTO)
		{
			var result = await work.Auth.ActiveAccount(accountDTO);
			return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(200));
		}

		[HttpGet("send-email-forget-password")]
		public async Task<IActionResult> forget(string email)
		{
			var result = await work.Auth.SendEmailForForgetPassword(email);
			return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(200));
		}






	}
}
