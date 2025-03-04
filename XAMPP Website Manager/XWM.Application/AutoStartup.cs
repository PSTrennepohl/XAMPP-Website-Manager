using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace XWM.Application;

public class AutoStartup
{
	public static void Startup(bool isChecked)
	{
		RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", writable: true);
		if (registryKey != null)
		{
			if (isChecked)
			{
				registryKey.SetValue(System.Windows.Forms.Application.ProductName, AppDomain.CurrentDomain.BaseDirectory + "XWMLauncher.exe");
			}
			else if (registryKey.GetValue(System.Windows.Forms.Application.ProductName) != null)
			{
				registryKey.DeleteValue(System.Windows.Forms.Application.ProductName);
			}
		}
	}
}
