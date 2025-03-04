using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using XWM.File;
using XWM.Ftp;
using XWM.Language;

namespace XWM.Application;

public class Config
{
	protected IniEntryCollection Settings;

	public List<Vhost> Vhosts = new List<Vhost>();

	private static Config _instance;

	protected string RegistryKey = "HKEY_CURRENT_USER\\SOFTWARE\\" + System.Windows.Forms.Application.ProductName;

	private CultureInfo _culture;

	private string _settingsFile;

	public string XamppDirectory { get; set; }

	public int ApplicationWidth { get; set; }

	public int ApplicationHeight { get; set; }

	public bool MinimizeTray { get; set; }

	public bool WindowsStartup { get; set; }

	public bool AutoStart { get; set; }

	public string DefaultEditor { get; set; }

	public bool Php7WarningShowed { get; set; }

	public int LogViewerWidth { get; set; }

	public int LogViewerHeight { get; set; }

	public int ConfigurationEditorWidth { get; set; }

	public int ConfigurationEditorHeight { get; set; }

	public bool FtpEnabled { get; set; }

	public string FtpIpaddress { get; set; }

	public int FtpPort { get; set; }

	public string FtpStoragePath => AppDomain.CurrentDomain.BaseDirectory + "ftp";

	public List<User> FtpUsers { get; set; }

	public bool AutoRestart { get; set; }

	public static Config Instance => _instance ?? (_instance = new Config());

	public CultureInfo Culture
	{
		get
		{
			return _culture;
		}
		set
		{
			Thread.CurrentThread.CurrentCulture = value;
			Thread.CurrentThread.CurrentUICulture = value;
			System.Windows.Forms.Application.CurrentCulture = value;
			_culture = value;
		}
	}

	public XWM.Language.Language Language { get; set; }

	public event ErrorEventHandler Error;

	public Config()
	{
		_settingsFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + System.Windows.Forms.Application.ProductName + "\\settings.ini";
		Settings = new IniEntryCollection();
		Culture = CultureInfo.CurrentCulture;
		Load();
		Language = new XWM.Language.Language();
	}

	public string GetDefaultEditor()
	{
		if (!string.IsNullOrEmpty(DefaultEditor) && System.IO.File.Exists(DefaultEditor))
		{
			return DefaultEditor;
		}
		return "notepad.exe";
	}

	protected virtual void OnError(ErrorEventArgs args)
	{
		this.Error?.Invoke(this, args);
	}

	public void Load()
	{
		try
		{
			Settings = IniParser.Parse(_settingsFile);
		}
		catch (Exception)
		{
			OnError(new ErrorEventArgs(new Exception("Failed to read settings.ini")));
		}
		XamppDirectory = Settings.FindString("XAMPP", "Directory", null);
		if (XamppDirectory == null)
		{
			object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\xampp", "Location", null);
			if (value != null)
			{
				XamppDirectory = value.ToString();
			}
		}
		ApplicationWidth = Settings.FindInt("Main", "ApplicationWidth", 0);
		ApplicationHeight = Settings.FindInt("Main", "ApplicationHeight", 0);
		LogViewerWidth = Settings.FindInt("Main", "LogViewerWidth", 0);
		LogViewerHeight = Settings.FindInt("Main", "LogViewerHeight", 0);
		ConfigurationEditorWidth = Settings.FindInt("Main", "ConfigurationEditorWidth", 0);
		ConfigurationEditorHeight = Settings.FindInt("Main", "ConfigurationEditorHeight", 0);
		DefaultEditor = Settings.FindString("Main", "DefaultEditor", null);
		if (Settings.FindString("Main", "Language", null) != null)
		{
			Culture = new CultureInfo(Settings.FindString("Main", "Language", null));
		}
		AutoRestart = Settings.FindBool("Main", "AutoRestart", defaultValue: false);
		MinimizeTray = Settings.FindBool("Main", "MinimizeTray", defaultValue: true);
		AutoStart = Settings.FindBool("Main", "AutoStart", defaultValue: false);
		WindowsStartup = Settings.FindBool("Main", "WindowsStartup", defaultValue: false);
		FtpEnabled = Settings.FindBool("FTP", "Enabled", defaultValue: false);
		FtpPort = Settings.FindInt("FTP", "Port", 21);
		FtpIpaddress = Settings.FindString("FTP", "IP", "127.0.0.1");
		FtpUsers = UserStore.Users;
		ReadVhosts();
	}

	public void Save()
	{
		Settings.AddOrReplace("XAMPP", "Directory", XamppDirectory);
		Settings.AddOrReplace("Main", "ApplicationWidth", ApplicationWidth);
		Settings.AddOrReplace("Main", "ApplicationHeight", ApplicationHeight);
		Settings.AddOrReplace("Main", "LogViewerWidth", LogViewerWidth);
		Settings.AddOrReplace("Main", "LogViewerHeight", LogViewerHeight);
		Settings.AddOrReplace("Main", "ConfigurationEditorWidth", ConfigurationEditorWidth);
		Settings.AddOrReplace("Main", "ConfigurationEditorHeight", ConfigurationEditorHeight);
		Settings.AddOrReplace("Main", "DefaultEditor", DefaultEditor);
		Settings.AddOrReplace("Main", "AutoRestart", AutoRestart);
		Settings.AddOrReplace("Main", "MinimizeTray", MinimizeTray);
		Settings.AddOrReplace("Main", "Language", Culture.Name);
		Settings.AddOrReplace("Main", "AutoStart", AutoStart);
		Settings.AddOrReplace("Main", "WindowsStartup", WindowsStartup);
		Settings.AddOrReplace("FTP", "Enabled", FtpEnabled);
		Settings.AddOrReplace("FTP", "IP", FtpIpaddress);
		Settings.AddOrReplace("FTP", "port", FtpPort);
		IniParser.Write(_settingsFile, Settings);
		AutoStartup.Startup(WindowsStartup);
	}

	public void SaveVhosts()
	{
		WriteVhosts();
	}

	public string GetHostFile()
	{
		return Environment.SystemDirectory + "\\drivers\\etc\\hosts";
	}

	public string GetVhostFile()
	{
		return XamppDirectory + "\\apache\\conf\\extra\\httpd-vhosts.conf";
	}

	protected void WriteVhosts()
	{
		try
		{
			foreach (PhpConfiguration.PhpVersion value in Enum.GetValues(typeof(PhpConfiguration.PhpVersion)))
			{
				PhpConfiguration.Write(PhpConfiguration.GetIni(value));
			}
			XWM.File.Vhosts.WriteVhosts(GetVhostFile(), Vhosts);
			new HostFile().Write(Vhosts);
			if (!Directory.Exists(FtpStoragePath))
			{
				Directory.CreateDirectory(FtpStoragePath);
			}
			string[] directories = Directory.GetDirectories(FtpStoragePath);
			for (int i = 0; i < directories.Length; i++)
			{
				Directory.Delete(directories[i]);
			}
			if (Vhosts.Count <= 0)
			{
				return;
			}
			foreach (Vhost item in Vhosts.Where((Vhost h) => h.EnableFtp))
			{
				string text = FtpStoragePath + "\\" + item.ServerName;
				if (!SymLink.Exists(text))
				{
					SymLink.Create(item.DocumentRoot, text, SymLink.SymbolicLink.Directory);
				}
			}
		}
		catch (Exception exception)
		{
			OnError(new ErrorEventArgs(exception));
		}
	}

	protected void ReadVhosts()
	{
		Vhosts.Clear();
		try
		{
			Vhosts = XWM.File.Vhosts.ReadVhosts(GetVhostFile());
			if (!Directory.Exists(FtpStoragePath))
			{
				return;
			}
			string[] directories = Directory.GetDirectories(FtpStoragePath);
			foreach (string path in directories)
			{
				DirectoryInfo dirInfo = new DirectoryInfo(path);
				Vhost vhost = Vhosts.FirstOrDefault((Vhost v) => v.ServerName.ToLower() == dirInfo.Name.ToLower());
				if (vhost != null)
				{
					vhost.EnableFtp = true;
				}
			}
		}
		catch (Exception exception)
		{
			OnError(new ErrorEventArgs(exception));
		}
	}

	public void SetLocale()
	{
		try
		{
			Language.SetLocale(Culture.Name);
		}
		catch (Exception exception)
		{
			OnError(new ErrorEventArgs(exception));
		}
	}

	public string GetTranslation(string key)
	{
		try
		{
			return Language.GetByKey(key);
		}
		catch (Exception exception)
		{
			OnError(new ErrorEventArgs(exception));
		}
		return string.Empty;
	}

	public string GetTranslation(string key, params object[] arguments)
	{
		try
		{
			return Language.GetByKey(key, arguments);
		}
		catch (Exception exception)
		{
			OnError(new ErrorEventArgs(exception));
		}
		return string.Empty;
	}
}
