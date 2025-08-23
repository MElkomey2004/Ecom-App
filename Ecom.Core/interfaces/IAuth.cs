using Ecom.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Core.interfaces
{
	public interface IAuth
	{
		Task<string> RegisterAsync(RegisterDTO registerDTO);
		Task SendEmail(string email, string code, string component, string subject, string message);
		Task<string> LoginAsync(LoginDTO login);
		Task<bool> SendEmailForForgetPassword(string email);
		Task<string> ResetPassword(ResetPasswordDTO resetPassword);
		Task<bool> ActiveAccount(ActiveAccountDTO activeAccount);
	}
}
