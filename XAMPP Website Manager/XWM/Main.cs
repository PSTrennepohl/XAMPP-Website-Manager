using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using log4net.Config;
using XWM.Application;
using XWM.File;
using XWM.Ftp;
using XWM.Xampp;

namespace XWM;

public class Main : Form
{
    private ContextMenuStrip TrayContextMenu;

	protected Form Editor;

	protected bool CreatingForm;

	private static Main _instance;

	protected FtpServer FtpServer;

	protected bool StartMinimized;

	private IContainer components;

	private NotifyIcon notifyIcon1;

	public static Main Instance()
	{
		return _instance ?? (_instance = new Main());
	}

	public Main()
	{
		InitializeComponent();
		_instance = this;
		base.Visible = false;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i].ToLower() == "/background")
			{
				StartMinimized = true;
			}
		}
		XmlConfigurator.Configure();
		notifyIcon1.Icon = (Icon)XWM.Editor.Resources.GetObject("ico");
		notifyIcon1.Text = System.Windows.Forms.Application.ProductName;
		TrayContextMenu = new ContextMenuStrip();
		XWM.Application.Config.Instance.Error += ConfigOnError;
		SetLanguage();
		base.Closing += delegate
		{
			StopFtpServer();
		};
		notifyIcon1.ContextMenuStrip = TrayContextMenu;
		notifyIcon1.MouseClick += NotifyIcon1OnMouseClick;
		if (XWM.Application.Config.Instance.AutoStart)
		{
			StartFtpServer();
			if (!XWM.Xampp.Config.ApacheRunning())
			{
				XWM.Xampp.Config.StartServers();
			}
		}
		if (!StartMinimized)
		{
			ShowEditor();
		}
	}

	public void StartFtpServer()
	{
		if (FtpRunning())
		{
			FtpServer.Dispose();
			FtpServer = null;
		}
		try
		{
			if (XWM.Application.Config.Instance.FtpEnabled)
			{
				IPAddress address = IPAddress.Parse("127.0.0.1");
				IPAddress.TryParse(XWM.Application.Config.Instance.FtpIpaddress, out address);
				FtpServer = new FtpServer(address, XWM.Application.Config.Instance.FtpPort);
				FtpServer.Start();
			}
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorFailedToStartFtp"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public void StopFtpServer()
	{
		try
		{
			if (FtpRunning())
			{
				FtpServer.Stop();
				FtpServer.Dispose();
				FtpServer = null;
			}
		}
		catch (Exception)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorFailedToStopFtp"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public bool FtpRunning()
	{
		return FtpServer != null;
	}

    public void SetLanguage()
    {
        XWM.Application.Config.Instance.SetLocale();
        CreateCertificate.Reset();
        TrayContextMenu.Items.Clear();

        // Criando o item do menu PHP
        ToolStripMenuItem menuItem = new ToolStripMenuItem("PHP");
        menuItem.DropDownItems.Add(new ToolStripMenuItem(
            XWM.Application.Config.Instance.GetTranslation("PHPVersionDefault"),
            null,
            (sender, e) => ShowConfigurationEditor(XWM.Application.Config.Instance.XamppDirectory + "\\php\\php.ini")
        ));

        // Criando o item do menu Apache
        ToolStripMenuItem menuItem2 = new ToolStripMenuItem("Apache");
        menuItem2.DropDownItems.Add(new ToolStripMenuItem(
            "Apache",
            null,
            (sender, e) => ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\apache\\logs\\error.log")
        ));
        menuItem2.DropDownItems.Add(new ToolStripMenuItem(
            "Access log",
            null,
            (sender, e) => ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\apache\\logs\\access.log")
        ));

        // Iteração para adicionar versões do PHP ao menu PHP
        foreach (object value in Enum.GetValues(typeof(PhpConfiguration.PhpVersion)))
        {
            PhpConfiguration.PhpVersion v = (PhpConfiguration.PhpVersion)value;
            DescriptionAttribute[] array = (DescriptionAttribute[])value.GetType().GetField(v.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
            string caption = ((array.Length != 0) ? array[0].Description : value.ToString());

            // Adicionando ao menu de PHP corretamente
            menuItem.DropDownItems.Add(new ToolStripMenuItem(
                caption,
                null,
                (sender, e) => ShowConfigurationEditor(PhpConfiguration.GetIni(v))
            ));
        }

        // Adicionando os menus ao TrayContextMenu
        TrayContextMenu.Items.Add(new ToolStripMenuItem(
            XWM.Application.Config.Instance.GetTranslation("Show"),
            null,
            (sender, e) => ShowEditor()
        ));

        TrayContextMenu.Items.Add("-");

        // Criando e adicionando um submenu de arquivos de configuração
        ToolStripMenuItem configMenu = new ToolStripMenuItem(XWM.Application.Config.Instance.GetTranslation("ConfigurationFiles"))
        {
            Name = XWM.Application.Config.Instance.GetTranslation("Configuration")
        };

        configMenu.DropDownItems.Add(menuItem);
        configMenu.DropDownItems.Add(new ToolStripMenuItem("Apache", null,
            (sender, e) => ShowFile(XWM.Application.Config.Instance.GetTranslation("TypeConfiguration", "Apache"), "\\apache\\conf\\httpd.conf"))
        {
            Name = "ApacheLogFile"
        });
        configMenu.DropDownItems.Add(new ToolStripMenuItem("MySQL", null,
            (sender, e) => ShowConfigurationEditor(Mysql.GetConfigurationFile()))
        {
            Name = "MySQLLogFile"
        });

        TrayContextMenu.Items.Add(configMenu);

        // Criando e adicionando um submenu de logs
        ToolStripMenuItem logsMenu = new ToolStripMenuItem(XWM.Application.Config.Instance.GetTranslation("Logs"))
        {
            Name = XWM.Application.Config.Instance.GetTranslation("Logs")
        };

        logsMenu.DropDownItems.Add(new ToolStripMenuItem("PHP", null,
            (sender, e) => ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\php\\logs\\php_error_log"))
        {
            Name = "PHPLogFile"
        });
        logsMenu.DropDownItems.Add(menuItem2);
        logsMenu.DropDownItems.Add(new ToolStripMenuItem("MySQL", null,
            (sender, e) => ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\mysql\\data\\mysql_error.log"))
        {
            Name = "MySQLLogFile"
        });
        logsMenu.DropDownItems.Add(new ToolStripMenuItem("FTP", null,
            (sender, e) => ShowLogViewer("logs\\ftp.log"))
        {
            Name = "FTP"
        });

        TrayContextMenu.Items.Add(logsMenu);

        // Criando botão para iniciar/parar servidores
        TrayContextMenu.Items.Add(new ToolStripMenuItem(XWM.Application.Config.Instance.GetTranslation("StartServers"),
            null,
            (sender, e) =>
            {
                if (XWM.Xampp.Config.ApacheRunning() || XWM.Xampp.Config.MysqlRunning())
                {
                    XWM.Xampp.Config.StopServers();
                }
                else
                {
                    XWM.Xampp.Config.StartServers();
                }
            })
        {
            Name = "startStopServer"
        });

        TrayContextMenu.Items.Add("-");

        // Criando botão de saída
        TrayContextMenu.Items.Add(new ToolStripMenuItem(XWM.Application.Config.Instance.GetTranslation("Exit"),
            null,
            (sender, e) => System.Windows.Forms.Application.Exit())
        {
            Name = XWM.Application.Config.Instance.GetTranslation("Exit")
        });

        // Atualiza o nome do botão "Start/Stop Servers" dinamicamente
        TrayContextMenu.Opening += (sender, e) =>
        {
            // Encontrando o item pelo nome
            ToolStripItem item = TrayContextMenu.Items.Find("startStopServer", searchAllChildren: true).FirstOrDefault();

            // Verifica se o item encontrado é do tipo ToolStripMenuItem
            if (item is ToolStripMenuItem menuItem3)
            {
                // Atualiza o texto do menuItem3 de acordo com o estado dos servidores
                menuItem3.Text = (XWM.Xampp.Config.ApacheRunning() || XWM.Xampp.Config.MysqlRunning()) ?
                    XWM.Application.Config.Instance.GetTranslation("StopServers") :
                    XWM.Application.Config.Instance.GetTranslation("StartServers");
            }
        };
    }


    private void ConfigOnError(object sender, ErrorEventArgs errorEventArgs)
	{
		MessageBox.Show(errorEventArgs.GetException().Message, XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
	}

	private void NotifyIcon1OnMouseClick(object sender, MouseEventArgs mouseEventArgs)
	{
		MouseButtons button = mouseEventArgs.Button;
		if (button == MouseButtons.Left && !CreatingForm)
		{
			if (Editor != null)
			{
				HideEditor();
			}
			else
			{
				ShowEditor();
			}
		}
	}

	public void ShowEditor()
	{
		if (Editor != null)
		{
			Editor.Visible = true;
			Editor.Focus();
			return;
		}
		CreatingForm = true;
		Editor = new Editor();
		Editor.Resize += EditorOnResize;
		Editor.Closing += EditorOnClosing;
		Editor.Show();
		CreatingForm = false;
	}

	private void EditorOnClosing(object sender, CancelEventArgs cancelEventArgs)
	{
		XWM.Application.Config.Instance.ApplicationWidth = ((Form)sender).Width;
		XWM.Application.Config.Instance.ApplicationHeight = ((Form)sender).Height;
		XWM.Application.Config.Instance.Save();
		if (XWM.Xampp.Config.ApacheRunning() || XWM.Xampp.Config.MysqlRunning())
		{
			XWM.Xampp.Config.StopServers();
		}
		System.Windows.Forms.Application.Exit();
	}

	private void EditorOnResize(object sender, System.EventArgs eventArgs)
	{
		if (XWM.Application.Config.Instance.MinimizeTray && ((Form)sender).WindowState == FormWindowState.Minimized && Editor != null)
		{
			((Form)sender).WindowState = FormWindowState.Normal;
			HideEditor();
		}
	}

	public void HideEditor()
	{
		Editor.Resize -= EditorOnResize;
		Editor.Dispose();
		Editor.Closing -= EditorOnClosing;
		Editor = null;
		MemoryManagement.FlushMemory();
	}

	public static void ShowFile(string name, string fileToOpen, bool xamppDirectory = true)
	{
		string text = (xamppDirectory ? (XWM.Application.Config.Instance.XamppDirectory + fileToOpen) : fileToOpen);
		if (!System.IO.File.Exists(text))
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorNotFound", name), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			Process.Start(XWM.Application.Config.Instance.GetDefaultEditor(), text);
		}
	}

	protected override void OnVisibleChanged(System.EventArgs e)
	{
		base.OnVisibleChanged(e);
		base.Visible = false;
	}

	public static void ShowConfigurationEditor(string file)
	{
		new ConfigurationEditor(file).Show();
	}

	public static void ShowLogViewer(string file)
	{
		new LogViewer(file).Show();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
		base.SuspendLayout();
		this.notifyIcon1.Text = "notifyIcon1";
		this.notifyIcon1.Visible = true;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.ClientSize = new System.Drawing.Size(46, 31);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "Main";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Main";
		base.WindowState = System.Windows.Forms.FormWindowState.Minimized;
		base.ResumeLayout(false);
	}
}
