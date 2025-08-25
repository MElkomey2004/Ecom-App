namespace Ecom.Core.Sharing
{
	public static class EmailStringBody
	{
		public static string send(string email, string token, string component, string message)
		{
			// مهم: نعمل UrlEncode عشان التوكن فيه رموز خاصة
			string encodedToken = Uri.EscapeDataString(token);
			string link = $"https://localhost:7293/api/Account/active-account?email={email}&token={encodedToken}";

			return $@"
                <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>{message}</h2>
                        <p>Click the button below to proceed:</p>
                        <a href='{link}' 
                           style='display:inline-block;
                                  padding:10px 20px;
                                  margin:10px 0;
                                  font-size:16px;
                                  color:white;
                                  background-color:#28a745;
                                  text-decoration:none;
                                  border-radius:5px;'>
                            Activate Account
                        </a>
                        <p>If the button doesn't work, copy and paste this link in your browser:</p>
                        <p><a href='{link}'>{link}</a></p>
                    </body>
                </html>
            ";
		}
	}
}
