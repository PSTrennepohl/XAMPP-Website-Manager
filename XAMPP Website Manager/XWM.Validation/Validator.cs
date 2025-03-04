using System.Net.Mail;

namespace XWM.Validation;

public class Validator
{
	public static bool IsValidEmail(string email)
	{
		try
		{
			return new MailAddress(email).Address == email;
		}
		catch
		{
			return false;
		}
	}
}
