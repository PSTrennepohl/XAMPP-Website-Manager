using System;
using System.IO;
using XWM.Application;

namespace XWM.Xampp;

internal class Phpmyadmin
{
	public static string GetConfigurationFile()
	{
		return XWM.Application.Config.Instance.XamppDirectory + "\\phpMyAdmin\\config.inc.php";
	}

	protected static void WriteConfig(string key, object value)
	{
		string configurationFile = GetConfigurationFile();
		if (!System.IO.File.Exists(configurationFile))
		{
			throw new Exception("Configuration file not found");
		}
		string[] array = System.IO.File.ReadAllLines(configurationFile);
		bool flag = false;
		int num = 0;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i].Contains("$cfg['Servers'][$i]['" + key + "']"))
			{
				array[num] = string.Format("$cfg['Servers'][$i]['" + key + "'] = '{0}';", value);
				flag = true;
			}
			num++;
		}
		if (!flag)
		{
			array[num + 1] = string.Format("$cfg['Servers'][$i]['" + key + "'] = '{0}';", value);
		}
		System.IO.File.WriteAllLines(configurationFile, array);
	}

	public static void ChangeMysqlPassword(string password)
	{
		WriteConfig("password", password);
	}

	public static void ChangeMysqlPort(int port)
	{
		WriteConfig("port", port);
	}
}
