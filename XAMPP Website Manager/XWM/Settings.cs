using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using XWM.Application;
using XWM.File;
using XWM.Ftp;
using XWM.Xampp;

namespace XWM;

public class Settings : Form
{
	protected Dictionary<string, string> LanguageMap = new Dictionary<string, string>();

	protected int MysqlPort;

	protected string ApacheHtdocsDirectory;

	protected int ApachePort;

	private IContainer components;

	private TextBox xamppDirectory;

	private Label xamppDirectoryLbl;

	private Button browseBtnXamppDirectory;

	private Button saveBtn;

	private Button cancelBtn;

	private FolderBrowserDialog folderBrowserDialog1;

	private CheckBox autoRestart;

	private Button browseDefaultEditor;

	private Label defaultEditorLbl;

	private TextBox defaultEditor;

	private OpenFileDialog openFileDialog1;

	private CheckBox minimizeTray;

	private TabControl tabControl1;

	private TabPage generalTab;

	private TabPage startupTab;

	private TabPage languageTab;

	private ComboBox language;

	private Label languageLbl;

	private CheckBox autostart;

	private CheckBox windowsStartup;

	private TabPage tabPage1;

	private TabPage tabPage2;

	private TabPage tabPage3;

	private TabPage tabPage4;

	private TabPage ftpTab;

	private TextBox ftpPortTxt;

	private Label usersLbl;

	private Label ftpPortLbl;

	private CheckBox ftpEnabled;

	private Label ftpIpLabl;

	private TextBox ftpIpTxt;

	private Button addFtpUserBtn;

	private ListBox ftpUsers;

	private ContextMenuStrip ftpUsersContextMenu;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem editToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private TabPage mysqlTab;

	private Button changeMysqlPasswordBtn;

	private Label changeMysqlPasswordLbl;

	private Label mysqlPortLbl;

	private TextBox mysqlPortTxt;

	private Button mysqlDefaultPortBtn;

	private TabPage apacheTab;

	private Label apacheDefaultHtdocsLbl;

	private Button apacheChangeHtdocsBtn;

	private Label apacheDefaultHtdocsDescriptionLbl;

	private Button apacheDefaultHtdocsBtn;

	private Button apacheDefaultPortBtn;

	private TextBox apacheDefaultPortTxt;

	private Label apacheDefaultPortLbl;

	public Settings()
	{
		InitializeComponent();
		LanguageMap.Add("da-DK", "Dansk");
		LanguageMap.Add("en-GB", "English");
		LanguageMap.Add("nb-NO", "Norsk");
		LanguageMap.Add("sv-SE", "Svenska");
		base.Icon = (Icon)Editor.Resources.GetObject("ico");
		xamppDirectory.Text = XWM.Application.Config.Instance.XamppDirectory;
		autoRestart.Checked = XWM.Application.Config.Instance.AutoRestart;
		defaultEditor.Text = XWM.Application.Config.Instance.DefaultEditor;
		minimizeTray.Checked = XWM.Application.Config.Instance.MinimizeTray;
		autostart.Checked = XWM.Application.Config.Instance.AutoStart;
		windowsStartup.Checked = XWM.Application.Config.Instance.WindowsStartup;
		ftpEnabled.Checked = XWM.Application.Config.Instance.FtpEnabled;
		ftpIpTxt.Text = XWM.Application.Config.Instance.FtpIpaddress.ToString();
		ftpPortTxt.Text = XWM.Application.Config.Instance.FtpPort.ToString();
		IniEntry port = Mysql.GetPort();
		mysqlPortTxt.Text = ((port != null) ? port.Value : Mysql.DefaultPort.ToString());
		int.TryParse(mysqlPortTxt.Text, out MysqlPort);
		Text = XWM.Application.Config.Instance.GetTranslation("Settings");
		browseDefaultEditor.Text = XWM.Application.Config.Instance.GetTranslation("Browse");
		browseBtnXamppDirectory.Text = XWM.Application.Config.Instance.GetTranslation("Browse");
		xamppDirectoryLbl.Text = XWM.Application.Config.Instance.GetTranslation("XAMPPInstallationDirectory");
		defaultEditorLbl.Text = XWM.Application.Config.Instance.GetTranslation("DefaultEditor");
		languageLbl.Text = XWM.Application.Config.Instance.GetTranslation("Language");
		autoRestart.Text = XWM.Application.Config.Instance.GetTranslation("AutoRestartServers");
		minimizeTray.Text = XWM.Application.Config.Instance.GetTranslation("MinimizeToTray");
		cancelBtn.Text = XWM.Application.Config.Instance.GetTranslation("Cancel");
		saveBtn.Text = XWM.Application.Config.Instance.GetTranslation("Save");
		windowsStartup.Text = XWM.Application.Config.Instance.GetTranslation("StartWithWindows");
		autostart.Text = XWM.Application.Config.Instance.GetTranslation("StartServersOnProgramStart");
		ftpEnabled.Text = XWM.Application.Config.Instance.GetTranslation("EnableFTP");
		ftpIpLabl.Text = XWM.Application.Config.Instance.GetTranslation("IpAddress");
		ftpPortLbl.Text = XWM.Application.Config.Instance.GetTranslation("Port");
		addFtpUserBtn.Text = XWM.Application.Config.Instance.GetTranslation("Add");
		usersLbl.Text = XWM.Application.Config.Instance.GetTranslation("Users");
		addToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("Add");
		editToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("Edit");
		deleteToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("Delete");
		mysqlPortLbl.Text = XWM.Application.Config.Instance.GetTranslation("Port");
		changeMysqlPasswordBtn.Text = XWM.Application.Config.Instance.GetTranslation("ChangeMySQLPassword");
		changeMysqlPasswordLbl.Text = XWM.Application.Config.Instance.GetTranslation("ChangeMySQLPasswordDescription");
		mysqlDefaultPortBtn.Text = XWM.Application.Config.Instance.GetTranslation("Default");
		apacheDefaultHtdocsDescriptionLbl.Text = XWM.Application.Config.Instance.GetTranslation("DefaultHtdocsDirectoryApache");
		apacheChangeHtdocsBtn.Text = XWM.Application.Config.Instance.GetTranslation("Change");
		apacheDefaultHtdocsBtn.Text = XWM.Application.Config.Instance.GetTranslation("Default");
		apacheDefaultPortLbl.Text = XWM.Application.Config.Instance.GetTranslation("DefaultPortApache");
		apacheDefaultPortBtn.Text = XWM.Application.Config.Instance.GetTranslation("Default");
		generalTab.Text = XWM.Application.Config.Instance.GetTranslation("Generel");
		startupTab.Text = XWM.Application.Config.Instance.GetTranslation("Startup");
		languageTab.Text = XWM.Application.Config.Instance.GetTranslation("Language");
		language.Items.Clear();
		foreach (KeyValuePair<string, string> item in LanguageMap)
		{
			language.Items.Add(item.Value);
		}
		string[] array = XWM.Application.Config.Instance.Culture.NativeName.Split(' ');
		int num = 0;
		foreach (object item2 in language.Items)
		{
			if (item2.ToString().ToLower() == array[0].ToLower())
			{
				language.SelectedItem = item2;
				break;
			}
			num++;
		}
		ApachePort = Apache.GetPort();
		ApacheHtdocsDirectory = Apache.GetHtdocsDirectory();
		apacheDefaultHtdocsLbl.Text = ApacheHtdocsDirectory;
		apacheDefaultPortTxt.Text = ApachePort.ToString();
		SetFtpUsers();
	}

	protected void SetFtpUsers()
	{
		ftpUsers.Items.Clear();
		if (XWM.Application.Config.Instance.FtpUsers.Count <= 0)
		{
			return;
		}
		foreach (User ftpUser in XWM.Application.Config.Instance.FtpUsers)
		{
			ftpUsers.Items.Add(ftpUser.Username);
		}
	}

	private void button1_Click(object sender, System.EventArgs e)
	{
		if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
		{
			if (!System.IO.File.Exists(folderBrowserDialog1.SelectedPath + "\\properties.ini"))
			{
				MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("ErrorXAMPPDirectoryNotFound"), XWM.Application.Config.Instance.GetTranslation("InvalidXAMPPDirectory"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else
			{
				xamppDirectory.Text = folderBrowserDialog1.SelectedPath;
			}
		}
	}

	private void cancelBtn_Click(object sender, System.EventArgs e)
	{
		Close();
	}

	private void saveBtn_Click(object sender, System.EventArgs e)
	{
		if (xamppDirectory.Text.Trim() == string.Empty)
		{
			MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("ErrorChooseXAMPPDirectory"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (!System.IO.File.Exists(xamppDirectory.Text.Trim() + "\\properties.ini"))
		{
			MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("ErrorXAMPPDirectoryNotFound"), XWM.Application.Config.Instance.GetTranslation("InvalidXAMPPDirectory"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (defaultEditor.Text.Trim() != string.Empty && !System.IO.File.Exists(defaultEditor.Text.Trim()))
		{
			MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("The selected default editor is not valid"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		XWM.Application.Config.Instance.XamppDirectory = xamppDirectory.Text;
		XWM.Application.Config.Instance.AutoRestart = autoRestart.Checked;
		XWM.Application.Config.Instance.DefaultEditor = defaultEditor.Text;
		XWM.Application.Config.Instance.MinimizeTray = minimizeTray.Checked;
		XWM.Application.Config.Instance.AutoStart = autostart.Checked;
		XWM.Application.Config.Instance.WindowsStartup = windowsStartup.Checked;
		bool flag = ftpEnabled.Checked != XWM.Application.Config.Instance.FtpEnabled;
		bool flag2 = false;
		XWM.Application.Config.Instance.FtpEnabled = ftpEnabled.Checked;
		if (int.TryParse(ftpPortTxt.Text, out var result))
		{
			if (XWM.Application.Config.Instance.FtpPort != result)
			{
				flag = true;
			}
			XWM.Application.Config.Instance.FtpPort = result;
		}
		if (IPAddress.TryParse(ftpIpTxt.Text, out var address))
		{
			if (XWM.Application.Config.Instance.FtpIpaddress != address.ToString())
			{
				flag = true;
			}
			XWM.Application.Config.Instance.FtpIpaddress = address.ToString();
		}
		if (int.TryParse(mysqlPortTxt.Text, out MysqlPort) && MysqlPort > 0 && MysqlPort != int.Parse(Mysql.GetPort().Value))
		{
			try
			{
				Mysql.ChangePort(MysqlPort);
				Phpmyadmin.ChangeMysqlPort(MysqlPort);
				flag2 = true;
			}
			catch (Exception)
			{
			}
		}
		if (int.TryParse(apacheDefaultPortTxt.Text, out ApachePort) && ApachePort > 0 && ApachePort != Apache.GetPort())
		{
			try
			{
				Apache.UpdatePort(ApachePort);
				flag2 = true;
			}
			catch (Exception)
			{
			}
		}
		ApacheHtdocsDirectory = apacheDefaultHtdocsLbl.Text;
		if (!string.IsNullOrEmpty(ApacheHtdocsDirectory) && ApacheHtdocsDirectory != Apache.GetHtdocsDirectory())
		{
			try
			{
				Apache.UpdateHtdocsDirectory(ApacheHtdocsDirectory);
				flag2 = true;
			}
			catch (Exception)
			{
			}
		}
		KeyValuePair<string, string> keyValuePair = LanguageMap.FirstOrDefault((KeyValuePair<string, string> l) => l.Value.ToLower() == language.SelectedItem.ToString().ToLower());
		if (keyValuePair.Key != XWM.Application.Config.Instance.Culture.Name)
		{
			XWM.Application.Config.Instance.Culture = new CultureInfo(keyValuePair.Key);
			XWM.Application.Config.Instance.Save();
			Main main = Main.Instance();
			main.HideEditor();
			main.SetLanguage();
			main.ShowEditor();
		}
		XWM.Application.Config.Instance.Save();
		if (flag && Main.Instance().FtpRunning() && MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("NewChangesRestartFtp"), XWM.Application.Config.Instance.GetTranslation("RestartFtp"), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
		{
			Main.Instance().StartFtpServer();
		}
		if (flag2 && XWM.Xampp.Config.MysqlRunning())
		{
			XWM.Xampp.Config.RestartRequired();
		}
		Close();
	}

	private void browseDefaultEditor_Click(object sender, System.EventArgs e)
	{
		if (openFileDialog1.ShowDialog() == DialogResult.OK)
		{
			defaultEditor.Text = openFileDialog1.FileName;
		}
	}

	private void addFtpUserBtn_Click(object sender, System.EventArgs e)
	{
		AddFtpUser addFtpUser = new AddFtpUser();
		addFtpUser.Closing += delegate
		{
			SetFtpUsers();
		};
		addFtpUser.ShowDialog(this);
	}

	private void ftpUsersContextMenu_Opening(object sender, CancelEventArgs e)
	{
		editToolStripMenuItem.Visible = ftpUsers.SelectedItems.Count > 0;
		deleteToolStripMenuItem.Visible = ftpUsers.SelectedItems.Count > 0;
	}

	private void deleteToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		if (ftpUsers.SelectedItems.Count > 0)
		{
			object username = ftpUsers.Items[ftpUsers.SelectedIndex];
			User user = UserStore.Users.FirstOrDefault((User u) => u.Username.ToLower() == username.ToString().ToLower());
			if (user != null)
			{
				UserStore.Users.Remove(user);
				SetFtpUsers();
			}
		}
	}

	private void editToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		if (ftpUsers.SelectedItems.Count <= 0)
		{
			return;
		}
		object username = ftpUsers.Items[ftpUsers.SelectedIndex];
		User user = UserStore.Users.FirstOrDefault((User u) => u.Username.ToLower() == username.ToString().ToLower());
		if (user != null)
		{
			AddFtpUser addFtpUser = new AddFtpUser(user);
			addFtpUser.Closing += delegate
			{
				SetFtpUsers();
			};
			addFtpUser.ShowDialog(this);
		}
	}

	private void addToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		AddFtpUser addFtpUser = new AddFtpUser();
		addFtpUser.Closing += delegate
		{
			SetFtpUsers();
		};
		addFtpUser.ShowDialog(this);
	}

	private void ftpUsers_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (ftpUsers.SelectedItems.Count <= 0)
		{
			return;
		}
		object username = ftpUsers.Items[ftpUsers.SelectedIndex];
		User user = UserStore.Users.FirstOrDefault((User u) => u.Username.ToLower() == username.ToString().ToLower());
		if (user != null)
		{
			AddFtpUser addFtpUser = new AddFtpUser(user);
			addFtpUser.Closing += delegate
			{
				SetFtpUsers();
			};
			addFtpUser.ShowDialog(this);
		}
	}

	private void changeMysqlPasswordBtn_Click(object sender, System.EventArgs e)
	{
		new ChangeMySqlPassword().ShowDialog(this);
	}

	private void mysqlDefaultPortBtn_Click(object sender, System.EventArgs e)
	{
		mysqlPortTxt.Text = Mysql.DefaultPort.ToString();
	}

	private void apacheChangeHtdocsBtn_Click(object sender, System.EventArgs e)
	{
		FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		if (folderBrowserDialog.ShowDialog() == DialogResult.OK && Directory.Exists(folderBrowserDialog.SelectedPath))
		{
			ApacheHtdocsDirectory = folderBrowserDialog.SelectedPath;
			apacheDefaultHtdocsLbl.Text = folderBrowserDialog.SelectedPath;
		}
	}

	private void apacheDefaultHtdocsBtn_Click(object sender, System.EventArgs e)
	{
		ApacheHtdocsDirectory = Apache.DefaultHtdocsDirectory;
		apacheDefaultHtdocsLbl.Text = Apache.DefaultHtdocsDirectory;
	}

	private void apacheDefaultPortBtn_Click(object sender, System.EventArgs e)
	{
		ApachePort = Apache.DefaultPort;
		apacheDefaultPortTxt.Text = Apache.DefaultPort.ToString();
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
		this.xamppDirectory = new System.Windows.Forms.TextBox();
		this.xamppDirectoryLbl = new System.Windows.Forms.Label();
		this.browseBtnXamppDirectory = new System.Windows.Forms.Button();
		this.saveBtn = new System.Windows.Forms.Button();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
		this.autoRestart = new System.Windows.Forms.CheckBox();
		this.browseDefaultEditor = new System.Windows.Forms.Button();
		this.defaultEditorLbl = new System.Windows.Forms.Label();
		this.defaultEditor = new System.Windows.Forms.TextBox();
		this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
		this.minimizeTray = new System.Windows.Forms.CheckBox();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.generalTab = new System.Windows.Forms.TabPage();
		this.apacheTab = new System.Windows.Forms.TabPage();
		this.apacheDefaultPortBtn = new System.Windows.Forms.Button();
		this.apacheDefaultPortTxt = new System.Windows.Forms.TextBox();
		this.apacheDefaultPortLbl = new System.Windows.Forms.Label();
		this.apacheDefaultHtdocsBtn = new System.Windows.Forms.Button();
		this.apacheDefaultHtdocsLbl = new System.Windows.Forms.Label();
		this.apacheChangeHtdocsBtn = new System.Windows.Forms.Button();
		this.apacheDefaultHtdocsDescriptionLbl = new System.Windows.Forms.Label();
		this.mysqlTab = new System.Windows.Forms.TabPage();
		this.mysqlDefaultPortBtn = new System.Windows.Forms.Button();
		this.changeMysqlPasswordLbl = new System.Windows.Forms.Label();
		this.mysqlPortLbl = new System.Windows.Forms.Label();
		this.mysqlPortTxt = new System.Windows.Forms.TextBox();
		this.changeMysqlPasswordBtn = new System.Windows.Forms.Button();
		this.ftpTab = new System.Windows.Forms.TabPage();
		this.ftpUsers = new System.Windows.Forms.ListBox();
		this.ftpUsersContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addFtpUserBtn = new System.Windows.Forms.Button();
		this.ftpEnabled = new System.Windows.Forms.CheckBox();
		this.ftpIpLabl = new System.Windows.Forms.Label();
		this.ftpIpTxt = new System.Windows.Forms.TextBox();
		this.usersLbl = new System.Windows.Forms.Label();
		this.ftpPortLbl = new System.Windows.Forms.Label();
		this.ftpPortTxt = new System.Windows.Forms.TextBox();
		this.startupTab = new System.Windows.Forms.TabPage();
		this.autostart = new System.Windows.Forms.CheckBox();
		this.windowsStartup = new System.Windows.Forms.CheckBox();
		this.languageTab = new System.Windows.Forms.TabPage();
		this.language = new System.Windows.Forms.ComboBox();
		this.languageLbl = new System.Windows.Forms.Label();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.tabPage4 = new System.Windows.Forms.TabPage();
		this.tabControl1.SuspendLayout();
		this.generalTab.SuspendLayout();
		this.apacheTab.SuspendLayout();
		this.mysqlTab.SuspendLayout();
		this.ftpTab.SuspendLayout();
		this.ftpUsersContextMenu.SuspendLayout();
		this.startupTab.SuspendLayout();
		this.languageTab.SuspendLayout();
		base.SuspendLayout();
		this.xamppDirectory.Location = new System.Drawing.Point(20, 40);
		this.xamppDirectory.Name = "xamppDirectory";
		this.xamppDirectory.Size = new System.Drawing.Size(293, 22);
		this.xamppDirectory.TabIndex = 2;
		this.xamppDirectoryLbl.Location = new System.Drawing.Point(17, 19);
		this.xamppDirectoryLbl.Name = "xamppDirectoryLbl";
		this.xamppDirectoryLbl.Size = new System.Drawing.Size(195, 21);
		this.xamppDirectoryLbl.TabIndex = 1;
		this.xamppDirectoryLbl.Text = "XAMPP directory";
		this.browseBtnXamppDirectory.Location = new System.Drawing.Point(319, 39);
		this.browseBtnXamppDirectory.Name = "browseBtnXamppDirectory";
		this.browseBtnXamppDirectory.Size = new System.Drawing.Size(75, 24);
		this.browseBtnXamppDirectory.TabIndex = 3;
		this.browseBtnXamppDirectory.Text = "Browse";
		this.browseBtnXamppDirectory.UseVisualStyleBackColor = true;
		this.browseBtnXamppDirectory.Click += new System.EventHandler(button1_Click);
		this.saveBtn.Location = new System.Drawing.Point(402, 282);
		this.saveBtn.Name = "saveBtn";
		this.saveBtn.Size = new System.Drawing.Size(73, 23);
		this.saveBtn.TabIndex = 100;
		this.saveBtn.Text = "Save";
		this.saveBtn.UseVisualStyleBackColor = true;
		this.saveBtn.Click += new System.EventHandler(saveBtn_Click);
		this.cancelBtn.Location = new System.Drawing.Point(325, 282);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(73, 23);
		this.cancelBtn.TabIndex = 99;
		this.cancelBtn.Text = "Cancel";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.autoRestart.AutoSize = true;
		this.autoRestart.Location = new System.Drawing.Point(17, 143);
		this.autoRestart.Name = "autoRestart";
		this.autoRestart.Size = new System.Drawing.Size(324, 17);
		this.autoRestart.TabIndex = 6;
		this.autoRestart.Text = "Restart servers automatically when new changes are made";
		this.autoRestart.UseVisualStyleBackColor = true;
		this.browseDefaultEditor.Location = new System.Drawing.Point(319, 95);
		this.browseDefaultEditor.Name = "browseDefaultEditor";
		this.browseDefaultEditor.Size = new System.Drawing.Size(75, 24);
		this.browseDefaultEditor.TabIndex = 5;
		this.browseDefaultEditor.Text = "Browse";
		this.browseDefaultEditor.UseVisualStyleBackColor = true;
		this.browseDefaultEditor.Click += new System.EventHandler(browseDefaultEditor_Click);
		this.defaultEditorLbl.Location = new System.Drawing.Point(17, 75);
		this.defaultEditorLbl.Name = "defaultEditorLbl";
		this.defaultEditorLbl.Size = new System.Drawing.Size(195, 21);
		this.defaultEditorLbl.TabIndex = 7;
		this.defaultEditorLbl.Text = "Default editor";
		this.defaultEditor.Location = new System.Drawing.Point(20, 96);
		this.defaultEditor.Name = "defaultEditor";
		this.defaultEditor.Size = new System.Drawing.Size(293, 22);
		this.defaultEditor.TabIndex = 4;
		this.openFileDialog1.FileName = "openFileDialog1";
		this.minimizeTray.AutoSize = true;
		this.minimizeTray.Location = new System.Drawing.Point(17, 169);
		this.minimizeTray.Name = "minimizeTray";
		this.minimizeTray.Size = new System.Drawing.Size(108, 17);
		this.minimizeTray.TabIndex = 7;
		this.minimizeTray.Text = "Minimize to tray";
		this.minimizeTray.UseVisualStyleBackColor = true;
		this.tabControl1.Controls.Add(this.generalTab);
		this.tabControl1.Controls.Add(this.apacheTab);
		this.tabControl1.Controls.Add(this.mysqlTab);
		this.tabControl1.Controls.Add(this.ftpTab);
		this.tabControl1.Controls.Add(this.startupTab);
		this.tabControl1.Controls.Add(this.languageTab);
		this.tabControl1.Location = new System.Drawing.Point(0, 4);
		this.tabControl1.Margin = new System.Windows.Forms.Padding(10);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(483, 272);
		this.tabControl1.TabIndex = 12;
		this.generalTab.Controls.Add(this.autoRestart);
		this.generalTab.Controls.Add(this.xamppDirectory);
		this.generalTab.Controls.Add(this.minimizeTray);
		this.generalTab.Controls.Add(this.xamppDirectoryLbl);
		this.generalTab.Controls.Add(this.browseDefaultEditor);
		this.generalTab.Controls.Add(this.browseBtnXamppDirectory);
		this.generalTab.Controls.Add(this.defaultEditorLbl);
		this.generalTab.Controls.Add(this.defaultEditor);
		this.generalTab.Location = new System.Drawing.Point(4, 22);
		this.generalTab.Name = "generalTab";
		this.generalTab.Size = new System.Drawing.Size(475, 246);
		this.generalTab.TabIndex = 0;
		this.generalTab.Text = "Generel";
		this.generalTab.UseVisualStyleBackColor = true;
		this.apacheTab.Controls.Add(this.apacheDefaultPortBtn);
		this.apacheTab.Controls.Add(this.apacheDefaultPortTxt);
		this.apacheTab.Controls.Add(this.apacheDefaultPortLbl);
		this.apacheTab.Controls.Add(this.apacheDefaultHtdocsBtn);
		this.apacheTab.Controls.Add(this.apacheDefaultHtdocsLbl);
		this.apacheTab.Controls.Add(this.apacheChangeHtdocsBtn);
		this.apacheTab.Controls.Add(this.apacheDefaultHtdocsDescriptionLbl);
		this.apacheTab.Location = new System.Drawing.Point(4, 22);
		this.apacheTab.Name = "apacheTab";
		this.apacheTab.Size = new System.Drawing.Size(475, 246);
		this.apacheTab.TabIndex = 5;
		this.apacheTab.Text = "Apache";
		this.apacheTab.UseVisualStyleBackColor = true;
		this.apacheDefaultPortBtn.Location = new System.Drawing.Point(73, 110);
		this.apacheDefaultPortBtn.Name = "apacheDefaultPortBtn";
		this.apacheDefaultPortBtn.Size = new System.Drawing.Size(75, 24);
		this.apacheDefaultPortBtn.TabIndex = 7;
		this.apacheDefaultPortBtn.Text = "Default";
		this.apacheDefaultPortBtn.UseVisualStyleBackColor = true;
		this.apacheDefaultPortBtn.Click += new System.EventHandler(apacheDefaultPortBtn_Click);
		this.apacheDefaultPortTxt.Location = new System.Drawing.Point(20, 111);
		this.apacheDefaultPortTxt.MaxLength = 4;
		this.apacheDefaultPortTxt.Name = "apacheDefaultPortTxt";
		this.apacheDefaultPortTxt.Size = new System.Drawing.Size(46, 22);
		this.apacheDefaultPortTxt.TabIndex = 6;
		this.apacheDefaultPortLbl.AutoSize = true;
		this.apacheDefaultPortLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.apacheDefaultPortLbl.Location = new System.Drawing.Point(17, 83);
		this.apacheDefaultPortLbl.Name = "apacheDefaultPortLbl";
		this.apacheDefaultPortLbl.Size = new System.Drawing.Size(70, 13);
		this.apacheDefaultPortLbl.TabIndex = 5;
		this.apacheDefaultPortLbl.Text = "Default port";
		this.apacheDefaultHtdocsBtn.Location = new System.Drawing.Point(379, 39);
		this.apacheDefaultHtdocsBtn.Name = "apacheDefaultHtdocsBtn";
		this.apacheDefaultHtdocsBtn.Size = new System.Drawing.Size(75, 24);
		this.apacheDefaultHtdocsBtn.TabIndex = 4;
		this.apacheDefaultHtdocsBtn.Text = "Default";
		this.apacheDefaultHtdocsBtn.UseVisualStyleBackColor = true;
		this.apacheDefaultHtdocsBtn.Click += new System.EventHandler(apacheDefaultHtdocsBtn_Click);
		this.apacheDefaultHtdocsLbl.Location = new System.Drawing.Point(17, 44);
		this.apacheDefaultHtdocsLbl.Name = "apacheDefaultHtdocsLbl";
		this.apacheDefaultHtdocsLbl.Size = new System.Drawing.Size(275, 26);
		this.apacheDefaultHtdocsLbl.TabIndex = 3;
		this.apacheDefaultHtdocsLbl.Text = "c:\\xampp\\htdocs";
		this.apacheChangeHtdocsBtn.Location = new System.Drawing.Point(298, 39);
		this.apacheChangeHtdocsBtn.Name = "apacheChangeHtdocsBtn";
		this.apacheChangeHtdocsBtn.Size = new System.Drawing.Size(75, 24);
		this.apacheChangeHtdocsBtn.TabIndex = 3;
		this.apacheChangeHtdocsBtn.Text = "Change";
		this.apacheChangeHtdocsBtn.UseVisualStyleBackColor = true;
		this.apacheChangeHtdocsBtn.Click += new System.EventHandler(apacheChangeHtdocsBtn_Click);
		this.apacheDefaultHtdocsDescriptionLbl.AutoSize = true;
		this.apacheDefaultHtdocsDescriptionLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.apacheDefaultHtdocsDescriptionLbl.Location = new System.Drawing.Point(17, 20);
		this.apacheDefaultHtdocsDescriptionLbl.Name = "apacheDefaultHtdocsDescriptionLbl";
		this.apacheDefaultHtdocsDescriptionLbl.Size = new System.Drawing.Size(192, 13);
		this.apacheDefaultHtdocsDescriptionLbl.TabIndex = 1;
		this.apacheDefaultHtdocsDescriptionLbl.Text = "Default htdocs directory for Apache";
		this.mysqlTab.Controls.Add(this.mysqlDefaultPortBtn);
		this.mysqlTab.Controls.Add(this.changeMysqlPasswordLbl);
		this.mysqlTab.Controls.Add(this.mysqlPortLbl);
		this.mysqlTab.Controls.Add(this.mysqlPortTxt);
		this.mysqlTab.Controls.Add(this.changeMysqlPasswordBtn);
		this.mysqlTab.Location = new System.Drawing.Point(4, 22);
		this.mysqlTab.Name = "mysqlTab";
		this.mysqlTab.Size = new System.Drawing.Size(475, 246);
		this.mysqlTab.TabIndex = 4;
		this.mysqlTab.Text = "MySQL";
		this.mysqlTab.UseVisualStyleBackColor = true;
		this.mysqlDefaultPortBtn.Location = new System.Drawing.Point(75, 39);
		this.mysqlDefaultPortBtn.Name = "mysqlDefaultPortBtn";
		this.mysqlDefaultPortBtn.Size = new System.Drawing.Size(105, 24);
		this.mysqlDefaultPortBtn.TabIndex = 7;
		this.mysqlDefaultPortBtn.Text = "Default";
		this.mysqlDefaultPortBtn.UseVisualStyleBackColor = true;
		this.mysqlDefaultPortBtn.Click += new System.EventHandler(mysqlDefaultPortBtn_Click);
		this.changeMysqlPasswordLbl.AutoSize = true;
		this.changeMysqlPasswordLbl.Location = new System.Drawing.Point(20, 132);
		this.changeMysqlPasswordLbl.Name = "changeMysqlPasswordLbl";
		this.changeMysqlPasswordLbl.Size = new System.Drawing.Size(311, 13);
		this.changeMysqlPasswordLbl.TabIndex = 6;
		this.changeMysqlPasswordLbl.Text = "Click here to change the default password used for MySQL.";
		this.mysqlPortLbl.AutoSize = true;
		this.mysqlPortLbl.Location = new System.Drawing.Point(19, 19);
		this.mysqlPortLbl.Name = "mysqlPortLbl";
		this.mysqlPortLbl.Size = new System.Drawing.Size(28, 13);
		this.mysqlPortLbl.TabIndex = 4;
		this.mysqlPortLbl.Text = "Port";
		this.mysqlPortTxt.Location = new System.Drawing.Point(22, 40);
		this.mysqlPortTxt.MaxLength = 4;
		this.mysqlPortTxt.Name = "mysqlPortTxt";
		this.mysqlPortTxt.Size = new System.Drawing.Size(46, 22);
		this.mysqlPortTxt.TabIndex = 0;
		this.changeMysqlPasswordBtn.Location = new System.Drawing.Point(22, 92);
		this.changeMysqlPasswordBtn.Name = "changeMysqlPasswordBtn";
		this.changeMysqlPasswordBtn.Size = new System.Drawing.Size(158, 23);
		this.changeMysqlPasswordBtn.TabIndex = 1;
		this.changeMysqlPasswordBtn.Text = "Change MySQL password";
		this.changeMysqlPasswordBtn.UseVisualStyleBackColor = true;
		this.changeMysqlPasswordBtn.Click += new System.EventHandler(changeMysqlPasswordBtn_Click);
		this.ftpTab.Controls.Add(this.ftpUsers);
		this.ftpTab.Controls.Add(this.addFtpUserBtn);
		this.ftpTab.Controls.Add(this.ftpEnabled);
		this.ftpTab.Controls.Add(this.ftpIpLabl);
		this.ftpTab.Controls.Add(this.ftpIpTxt);
		this.ftpTab.Controls.Add(this.usersLbl);
		this.ftpTab.Controls.Add(this.ftpPortLbl);
		this.ftpTab.Controls.Add(this.ftpPortTxt);
		this.ftpTab.Location = new System.Drawing.Point(4, 22);
		this.ftpTab.Name = "ftpTab";
		this.ftpTab.Size = new System.Drawing.Size(475, 246);
		this.ftpTab.TabIndex = 3;
		this.ftpTab.Text = "FTP";
		this.ftpTab.UseVisualStyleBackColor = true;
		this.ftpUsers.ContextMenuStrip = this.ftpUsersContextMenu;
		this.ftpUsers.FormattingEnabled = true;
		this.ftpUsers.Items.AddRange(new object[1] { "sessingø" });
		this.ftpUsers.Location = new System.Drawing.Point(18, 130);
		this.ftpUsers.Name = "ftpUsers";
		this.ftpUsers.Size = new System.Drawing.Size(437, 95);
		this.ftpUsers.TabIndex = 5;
		this.ftpUsers.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(ftpUsers_MouseDoubleClick);
		this.ftpUsersContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addToolStripMenuItem, this.editToolStripMenuItem, this.deleteToolStripMenuItem });
		this.ftpUsersContextMenu.Name = "ftpUsersContextMenu";
		this.ftpUsersContextMenu.Size = new System.Drawing.Size(108, 70);
		this.ftpUsersContextMenu.Opening += new System.ComponentModel.CancelEventHandler(ftpUsersContextMenu_Opening);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(addToolStripMenuItem_Click);
		this.editToolStripMenuItem.Name = "editToolStripMenuItem";
		this.editToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.editToolStripMenuItem.Text = "Edit";
		this.editToolStripMenuItem.Click += new System.EventHandler(editToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		this.addFtpUserBtn.Location = new System.Drawing.Point(395, 99);
		this.addFtpUserBtn.Name = "addFtpUserBtn";
		this.addFtpUserBtn.Size = new System.Drawing.Size(61, 23);
		this.addFtpUserBtn.TabIndex = 4;
		this.addFtpUserBtn.Text = "Add";
		this.addFtpUserBtn.UseVisualStyleBackColor = true;
		this.addFtpUserBtn.Click += new System.EventHandler(addFtpUserBtn_Click);
		this.ftpEnabled.AutoSize = true;
		this.ftpEnabled.Location = new System.Drawing.Point(18, 19);
		this.ftpEnabled.Name = "ftpEnabled";
		this.ftpEnabled.Size = new System.Drawing.Size(68, 17);
		this.ftpEnabled.TabIndex = 1;
		this.ftpEnabled.Text = "Enabled";
		this.ftpEnabled.UseVisualStyleBackColor = true;
		this.ftpIpLabl.AutoSize = true;
		this.ftpIpLabl.Location = new System.Drawing.Point(15, 47);
		this.ftpIpLabl.Name = "ftpIpLabl";
		this.ftpIpLabl.Size = new System.Drawing.Size(60, 13);
		this.ftpIpLabl.TabIndex = 4;
		this.ftpIpLabl.Text = "IP Address";
		this.ftpIpTxt.Location = new System.Drawing.Point(18, 68);
		this.ftpIpTxt.Name = "ftpIpTxt";
		this.ftpIpTxt.Size = new System.Drawing.Size(100, 22);
		this.ftpIpTxt.TabIndex = 2;
		this.usersLbl.AutoSize = true;
		this.usersLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.usersLbl.Location = new System.Drawing.Point(14, 105);
		this.usersLbl.Name = "usersLbl";
		this.usersLbl.Size = new System.Drawing.Size(35, 13);
		this.usersLbl.TabIndex = 2;
		this.usersLbl.Text = "Users";
		this.ftpPortLbl.AutoSize = true;
		this.ftpPortLbl.Location = new System.Drawing.Point(127, 47);
		this.ftpPortLbl.Name = "ftpPortLbl";
		this.ftpPortLbl.Size = new System.Drawing.Size(28, 13);
		this.ftpPortLbl.TabIndex = 1;
		this.ftpPortLbl.Text = "Port";
		this.ftpPortTxt.Location = new System.Drawing.Point(130, 68);
		this.ftpPortTxt.MaxLength = 4;
		this.ftpPortTxt.Name = "ftpPortTxt";
		this.ftpPortTxt.Size = new System.Drawing.Size(46, 22);
		this.ftpPortTxt.TabIndex = 3;
		this.startupTab.Controls.Add(this.autostart);
		this.startupTab.Controls.Add(this.windowsStartup);
		this.startupTab.Location = new System.Drawing.Point(4, 22);
		this.startupTab.Name = "startupTab";
		this.startupTab.Size = new System.Drawing.Size(475, 246);
		this.startupTab.TabIndex = 1;
		this.startupTab.Text = "Startup";
		this.startupTab.UseVisualStyleBackColor = true;
		this.autostart.AutoSize = true;
		this.autostart.Location = new System.Drawing.Point(18, 46);
		this.autostart.Name = "autostart";
		this.autostart.Size = new System.Drawing.Size(221, 17);
		this.autostart.TabIndex = 1;
		this.autostart.Text = "Start servers when program is opened";
		this.autostart.UseVisualStyleBackColor = true;
		this.windowsStartup.AutoSize = true;
		this.windowsStartup.Location = new System.Drawing.Point(18, 19);
		this.windowsStartup.Name = "windowsStartup";
		this.windowsStartup.Size = new System.Drawing.Size(297, 17);
		this.windowsStartup.TabIndex = 0;
		this.windowsStartup.Text = "Start XAMPP Website Manager when Windows starts";
		this.windowsStartup.UseVisualStyleBackColor = true;
		this.languageTab.Controls.Add(this.language);
		this.languageTab.Controls.Add(this.languageLbl);
		this.languageTab.Location = new System.Drawing.Point(4, 22);
		this.languageTab.Name = "languageTab";
		this.languageTab.Size = new System.Drawing.Size(475, 246);
		this.languageTab.TabIndex = 2;
		this.languageTab.Text = "Language";
		this.languageTab.UseVisualStyleBackColor = true;
		this.language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.language.FormattingEnabled = true;
		this.language.Items.AddRange(new object[2] { "Dansk", "English" });
		this.language.Location = new System.Drawing.Point(20, 41);
		this.language.Name = "language";
		this.language.Size = new System.Drawing.Size(121, 21);
		this.language.Sorted = true;
		this.language.TabIndex = 1;
		this.languageLbl.Location = new System.Drawing.Point(17, 19);
		this.languageLbl.Name = "languageLbl";
		this.languageLbl.Size = new System.Drawing.Size(195, 21);
		this.languageLbl.TabIndex = 12;
		this.languageLbl.Text = "Language";
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(192, 74);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "tabPage1";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(192, 74);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "tabPage2";
		this.tabPage2.UseVisualStyleBackColor = true;
		this.tabPage3.Location = new System.Drawing.Point(4, 22);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(192, 74);
		this.tabPage3.TabIndex = 0;
		this.tabPage3.Text = "tabPage3";
		this.tabPage3.UseVisualStyleBackColor = true;
		this.tabPage4.Location = new System.Drawing.Point(4, 22);
		this.tabPage4.Name = "tabPage4";
		this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage4.Size = new System.Drawing.Size(192, 74);
		this.tabPage4.TabIndex = 1;
		this.tabPage4.Text = "tabPage4";
		this.tabPage4.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(481, 313);
		base.Controls.Add(this.cancelBtn);
		base.Controls.Add(this.saveBtn);
		base.Controls.Add(this.tabControl1);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.Name = "Settings";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Settings";
		this.tabControl1.ResumeLayout(false);
		this.generalTab.ResumeLayout(false);
		this.generalTab.PerformLayout();
		this.apacheTab.ResumeLayout(false);
		this.apacheTab.PerformLayout();
		this.mysqlTab.ResumeLayout(false);
		this.mysqlTab.PerformLayout();
		this.ftpTab.ResumeLayout(false);
		this.ftpTab.PerformLayout();
		this.ftpUsersContextMenu.ResumeLayout(false);
		this.startupTab.ResumeLayout(false);
		this.startupTab.PerformLayout();
		this.languageTab.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
