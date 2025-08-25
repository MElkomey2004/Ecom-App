using Ecom.Core.DTO;
using Ecom.Core.Entites;
using Ecom.Core.interfaces;
using Ecom.Core.services;
using Ecom.Core.Sharing;
using Ecom.infrastructure.Data;
using Ecom.infrastructure.Repositories.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Ecom.infrastructure.Repositories
{
	public class AuthRepository : IAuth
	{
		private readonly UserManager<AppUser> userManager;
		private readonly IEmailService _emailService;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IGenerateToken _generateToken;
		private readonly AppDbContext context;
		public AuthRepository(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IGenerateToken generateToken, AppDbContext context)
		{
			this.userManager = userManager;
			_emailService = emailService;
			_signInManager = signInManager;
			_generateToken = generateToken;
			this.context = context;
		}

		public async Task<string> RegisterAsync(RegisterDTO registerDTO)
		{
			if (registerDTO == null)
			{
				return null;
			}
			if (await userManager.FindByNameAsync(registerDTO.UserName) is not null)
			{
				return "This UserName is already registered";
			}
			if (await userManager.FindByEmailAsync(registerDTO.Email) is not null)
			{
				return "This Email is already registered";
			}

			AppUser user = new AppUser
			{
				Email = registerDTO.Email,
				UserName = registerDTO.UserName,
				DisplayName = registerDTO.DisplayName,
			};

			var result = await userManager.CreateAsync(user, registerDTO.Password);
			if (!result.Succeeded)
			{
				return result.Errors.ToList()[0].Description;
			}
			string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
			await SendEmail(user.Email, token, "active", "Active Email", "Please activate your account by clicking the button");

			return "done";
		}

		public async Task SendEmail(string email, string code, string component, string subject, string message)
		{
			var result = new EmailDTO(email,
				"elkomey2004@gmail.com",
				subject,
				EmailStringBody.send(email, code, component, message));
			await _emailService.SendEmail(result);
		}

		public async Task<string> LoginAsync(LoginDTO login)
		{
			if (login == null)
			{
				return null;
			}

			var finduser = await userManager.FindByEmailAsync(login.Email);

			if (!finduser.EmailConfirmed)
			{
				string token = await userManager.GenerateEmailConfirmationTokenAsync(finduser);
				await SendEmail(finduser.Email, token, "active", "Active Email", "Please activate your account by clicking the button");
				return "Please confirm your email first, we have sent an activation link to your email";
			}

			var result = await _signInManager.CheckPasswordSignInAsync(finduser, login.Password, lockoutOnFailure: true);
			if (result.Succeeded)
			{
				return _generateToken.GetAndCreateToken(finduser);
			}
			return "Please check your email or password, they are not correct";
		}

		public async Task<bool> SendEmailForForgetPassword(string email)
		{
			var finduser = await userManager.FindByEmailAsync(email);
			if (finduser == null)
			{
				return false;
			}
			var token = await userManager.GeneratePasswordResetTokenAsync(finduser);
			await SendEmail(finduser.Email, token, "Reset-Password", "Reset Password", "Click the button to reset your password");

			return true;
		}

		public async Task<string> ResetPassword(ResetPasswordDTO resetPassword)
		{
			var finduser = await userManager.FindByEmailAsync(resetPassword.Email);
			if (finduser == null)
			{
				return null;
			}
			var result = await userManager.ResetPasswordAsync(finduser, resetPassword.Token, resetPassword.Password);
			if (result.Succeeded)
			{
				return "Password reset successfully";
			}
			return result.Errors.ToList()[0].Description;
		}

		public async Task<bool> ActiveAccount(ActiveAccountDTO activeAccount)
		{
			var finduser = await userManager.FindByEmailAsync(activeAccount.Email);
			if (finduser == null)
			{
				return false;
			}

			var decodedToken = Uri.UnescapeDataString(activeAccount.Token);
			var result = await userManager.ConfirmEmailAsync(finduser, decodedToken);
			if (result.Succeeded)
			{
				return true;
			}

			return false;
		}

		public async Task<bool> UpdateAddress(string email, Adress adress)
		{
			var findUser = await userManager.FindByEmailAsync(email);
			if (findUser is null)
			{
				return false;
			}
			var Myaddress = await context.Addresses.AsNoTracking()
				.FirstOrDefaultAsync(m => m.AppUserId == findUser.Id);

			if (Myaddress is null)
			{
				adress.AppUserId = findUser.Id;
				await context.Addresses.AddAsync(adress);
			}
			else
			{
				context.Entry(Myaddress).State = EntityState.Detached;
				adress.Id = Myaddress.Id;
				adress.AppUserId = Myaddress.AppUserId;
				context.Addresses.Update(adress);

			}
			await context.SaveChangesAsync();
			return true;

		}

		public async Task<Adress> getUserAddress(string email)
		{
			var user = await userManager.FindByEmailAsync(email);
			var address = await context.Addresses.FirstOrDefaultAsync(m => m.AppUserId == user.Id);
			return address;

		}
	}
}
