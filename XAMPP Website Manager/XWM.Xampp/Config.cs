using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using XWM.Application;

namespace XWM.Xampp;

internal static class Config
{
	public static void StartApache()
	{
		try
		{
			Process.Start(new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				FileName = XWM.Application.Config.Instance.XamppDirectory + "\\apache_start.bat"
			});
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorApacheFailedStart"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public static void StartMySql()
	{
		try
		{
			Process.Start(new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				FileName = XWM.Application.Config.Instance.XamppDirectory + "\\mysql_start.bat"
			});
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorMySQLFailedStart"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public static void StartServers()
	{
		StartApache();
		StartMySql();
		Main.Instance().StartFtpServer();
	}

	public static void StopServers()
	{
		try
		{
			Process.Start(new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				FileName = XWM.Application.Config.Instance.XamppDirectory + "\\apache_stop.bat"
			});
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorApacheFailedStop"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		try
		{
			Process.Start(new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				FileName = XWM.Application.Config.Instance.XamppDirectory + "\\mysql_stop.bat"
			});
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorMySQLFailedStop"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		Main.Instance().StopFtpServer();
	}

	public static void RestartRequired()
	{
		if (!ApacheRunning() || (!XWM.Application.Config.Instance.AutoRestart && MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ConfirmRestartChanges"), XWM.Application.Config.Instance.GetTranslation("RestartRequired"), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes))
		{
			return;
		}
		try
		{
			Process.Start(new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				FileName = XWM.Application.Config.Instance.XamppDirectory + "\\apache_stop.bat"
			});
			new Thread((ThreadStart)delegate
			{
				Thread.Sleep(1000);
				Process.Start(new ProcessStartInfo
				{
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = XWM.Application.Config.Instance.XamppDirectory + "\\apache_start.bat"
				});
			}).Start();
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorApacheFailedStart"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public static bool ApacheRunning()
	{
		return Process.GetProcessesByName("httpd").Length != 0;
	}

	public static bool MysqlRunning()
	{
		return Process.GetProcessesByName("mysqld").Length != 0;
	}
}
