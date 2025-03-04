using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using XWM.Application;

namespace XWM.File;

public class PhpConfiguration
{
	public enum PhpVersion
	{
		[Description("PHP 5.4.4")]
		Php544,
		[Description("PHP 5.5.2")]
		Php552,
		[Description("PHP 5.6.1")]
		Php561,
		[Description("PHP 7.0.0 RC4")]
		Php700Rc3
	}

	public static string GetDirectory(PhpVersion? phpVersion)
	{
		return phpVersion switch
		{
			PhpVersion.Php544 => AppDomain.CurrentDomain.BaseDirectory + "php\\php-5.4.4", 
			PhpVersion.Php552 => AppDomain.CurrentDomain.BaseDirectory + "php\\php-5.5.2", 
			PhpVersion.Php561 => AppDomain.CurrentDomain.BaseDirectory + "php\\php-5.6.1", 
			PhpVersion.Php700Rc3 => AppDomain.CurrentDomain.BaseDirectory + "php\\php-7.0.0RC4", 
			_ => Config.Instance.XamppDirectory + "\\php", 
		};
	}

	public static string GetIni(PhpVersion? phpVersion)
	{
		return GetDirectory(phpVersion) + "\\php.ini";
	}

	public static void Write(string phpIni)
	{
		List<string> list = System.IO.File.ReadAllLines(phpIni).ToList();
		string xamppDirectory = Config.Instance.XamppDirectory;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			if (text.Contains("error_log") && text.IndexOf(";") == -1)
			{
				list[i] = string.Format("error_log = \"{0}\"", xamppDirectory + "\\php\\logs\\php_error_log");
				flag = true;
			}
			if (text.Contains("sendmail_path") && text.IndexOf(";") == -1)
			{
				list[i] = string.Format("sendmail_path = \"{0}\"", xamppDirectory + "\\mailtodisk\\mailtodisk.exe");
				flag2 = true;
			}
			if (text.Contains("browscap") && text.IndexOf(";") == -1)
			{
				list[i] = string.Format("browscap = \"{0}\"", xamppDirectory + "\\php\\extras\\browscap.ini");
				flag3 = true;
			}
			if (text.Contains("extension_dir") && text.IndexOf(";") == -1)
			{
				list[i] = "extension_dir = \"ext\"";
				flag4 = true;
			}
		}
		if (!flag || !flag2 || !flag3 || !flag4)
		{
			for (int j = 0; j < list.Count; j++)
			{
				string text2 = list[j];
				if (!flag && text2.Contains("[PHP]"))
				{
					list[j] = text2 + "\n" + string.Format("error_log = \"{0}\"", xamppDirectory + "\\php\\logs\\php_error_log");
				}
				if (!flag2 && text2.Contains("[mail function]"))
				{
					list[j] = text2 + "\n" + string.Format("sendmail_path = \"{0}\"", xamppDirectory + "\\mailtodisk\\mailtodisk.exe");
				}
				if (!flag3 && text2.Contains("[bcmath]"))
				{
					list[j] = text2 + "\n" + string.Format("browscap = \"{0}\"", xamppDirectory + "\\php\\extras\\browscap.ini");
				}
				if (!flag4 && text2.Contains("[PHP]"))
				{
					list[j] = text2 + "\nextension_dir = \"ext\"";
				}
			}
		}
		System.IO.File.WriteAllLines(phpIni, list);
	}
}
