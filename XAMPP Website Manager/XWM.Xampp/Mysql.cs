using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using XWM.Application;
using XWM.File;

namespace XWM.Xampp;

internal static class Mysql
{
	public static int DefaultPort => 3306;

	public static string GetConfigurationPath()
	{
		string text = XWM.Application.Config.Instance.XamppDirectory + "\\properties.ini";
		IniEntryCollection iniEntryCollection = null;
		IniEntry iniEntry = null;
		if (System.IO.File.Exists(text))
		{
			iniEntryCollection = IniParser.Parse(text);
			iniEntry = iniEntryCollection.FirstOrDefault((IniEntry p) => p.Key.ToLower() == "mysql_configuration_directory");
		}
		if (iniEntryCollection == null || iniEntry == null)
		{
			return XWM.Application.Config.Instance.XamppDirectory + "\\mysql\\bin";
		}
		return iniEntry.Value;
	}

	public static string GetConfigurationFile()
	{
		return GetConfigurationPath() + "\\my.ini";
	}

	public static IniEntryCollection GetConfiguration()
	{
		return IniParser.Parse(GetConfigurationFile());
	}

	public static void ChangePassword(string oldPassword, string password, Action<Process> callback)
	{
		string configurationPath = GetConfigurationPath();
		string arg = ((!string.IsNullOrEmpty(oldPassword)) ? ("-p" + oldPassword) : string.Empty);
		Process process = new Process
		{
			StartInfo = 
			{
				FileName = configurationPath + "\\mysql.exe",
				Arguments = string.Format("-u root {0} -e \"SET PASSWORD FOR 'root'@'localhost' = PASSWORD('{1}');\"", arg, password.Replace("'", "'")),
				WindowStyle = ProcessWindowStyle.Hidden
			},
			EnableRaisingEvents = true
		};
		process.Exited += delegate
		{
			callback(process);
		};
		process.Start();
	}

	public static void ChangePort(int port)
	{
		IniEntryCollection configuration = GetConfiguration();
		configuration.AddOrReplace("mysqld", "port", port);
		IniParser.Update(GetConfigurationFile(), configuration);
	}

	public static IniEntry GetPort()
	{
		return GetConfiguration().FirstOrDefault((IniEntry c) => c.Key.ToLower() == "port" && c.Group.ToLower() == "mysqld");
	}
}
