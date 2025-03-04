using System.IO;
using XWM.Application;

namespace XWM.Xampp;

internal class Apache
{
	public static int DefaultPort => 80;

	public static string DefaultHtdocsDirectory => XWM.Application.Config.Instance.XamppDirectory + "\\htdocs";

	public static string GetConfigurationPath()
	{
		return XWM.Application.Config.Instance.XamppDirectory + "\\apache\\conf";
	}

	public static string GetConfigurationFile()
	{
		return GetConfigurationPath() + "\\httpd.conf";
	}

	public static void UpdatePort(int port)
	{
		string[] array = System.IO.File.ReadAllLines(GetConfigurationFile());
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.Contains("#"))
			{
				if (text.ToLower().Contains("servername "))
				{
					array[num] = $"ServerName localhost:{port}";
				}
				if (text.ToLower().Contains("listen "))
				{
					array[num] = $"Listen {port}";
				}
			}
			num++;
		}
		System.IO.File.WriteAllLines(GetConfigurationFile(), array);
	}

	public static void UpdateHtdocsDirectory(string path)
	{
		string[] array = System.IO.File.ReadAllLines(GetConfigurationFile());
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.Contains("#"))
			{
				if (text.ToLower().Contains("documentroot "))
				{
					array[num] = $"DocumentRoot \"{path}\"";
				}
				if (text.ToLower().Contains("<directory \""))
				{
					array[num] = $"<Directory \"{path}\">";
				}
			}
			num++;
		}
		System.IO.File.WriteAllLines(GetConfigurationFile(), array);
	}

	public static int GetPort()
	{
		string[] array = System.IO.File.ReadAllLines(GetConfigurationFile());
		int result = DefaultPort;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.Contains("#") && text.ToLower().Contains("listen "))
			{
				int.TryParse(text.Replace("Listen ", string.Empty).Trim(), out result);
				break;
			}
		}
		return result;
	}

	public static string GetHtdocsDirectory()
	{
		string[] array = System.IO.File.ReadAllLines(GetConfigurationFile());
		string result = DefaultHtdocsDirectory;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (!text.Contains("#") && text.ToLower().Contains("documentroot "))
			{
				result = text.Replace("DocumentRoot ", string.Empty).Replace("\"", string.Empty).Trim();
				break;
			}
		}
		return result;
	}
}
