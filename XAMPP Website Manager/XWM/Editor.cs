using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using XWM.Application;
using XWM.CustomControls;
using XWM.Data;
using XWM.EventArgs;
using XWM.File;
using XWM.Xampp;

namespace XWM;

public class Editor : Form
{
	private delegate void CertificateCallback(CreateCertificateEventArgs args);

	public static ResourceManager Resources = XWM.Data.Resources.Get();

	private IContainer components;

	private ToolStripMenuItem filesToolStripMenuItem;

	private ToolStripMenuItem exitToolStripMenuItem;

	private ToolStripMenuItem editToolStripMenuItem;

	private ToolStripMenuItem settingsToolStripMenuItem;

	private ToolStripMenuItem helpToolStripMenuItem;

	private ToolStripMenuItem aboutToolStripMenuItem;

	private Label documentRootLbl;

	private TextBox documentRoot;

	private Label serverNameLbl;

	private TextBox serverName;

	private Button saveBtn;

	private System.Windows.Forms.Timer serverStatus;

	private Panel headerPanel;

	private CheckBox mysqlStatus;

	private CheckBox apacheStatus;

	private Button browseBtn;

	private Button addVhostBtn;

	private Button deleteVhostBtn;

	private FolderBrowserDialog folderBrowserDialog1;

	private PictureBox logo;

	private ToolStripMenuItem checkForUpdateToolStripMenuItem;

	private ToolStripMenuItem configurationToolStripMenuItem;

	private ToolStripMenuItem phpiniToolStripMenuItem;

	private ToolStripMenuItem mycnfToolStripMenuItem;

	private PictureBox powerPicture;

	private Label serverStatusTxt;

	private System.Windows.Forms.Timer appStatus;

	private ToolStripMenuItem apacheToolStripMenuItem1;

	private ToolStripMenuItem logsToolStripMenuItem;

	private ToolStripMenuItem apacheToolStripMenuItem;

	private ToolStripMenuItem pHPToolStripMenuItem;

	private ToolStripMenuItem mySQLToolStripMenuItem;

	private Label label3;

	private PictureBox phpmyadminPicture;

	private Panel menuPanel;

	private Button filesBtn;

	private Button settingsBtn;

	private Button helpBtn;

	private ContextMenuStrip filesContextMenu;

	private ToolStripMenuItem exitToolStripMenuItem1;

	private ContextMenuStrip settingsContextMenu;

	private ToolStripMenuItem settingsToolStripMenuItem1;

	private ToolStripMenuItem pHPToolStripMenuItem3;

	private ToolStripMenuItem apacheToolStripMenuItem4;

	private ToolStripMenuItem mySQLToolStripMenuItem3;

	private ToolStripMenuItem logsToolStripMenuItem2;

	private ToolStripMenuItem settingsToolStripMenuItem2;

	private ContextMenuStrip helpContextMenu;

	private ToolStripMenuItem aboutToolStripMenuItem1;

	private ToolStripMenuItem pHPToolStripMenuItem4;

	private ToolStripMenuItem apacheToolStripMenuItem5;

	private ToolStripMenuItem mySQLToolStripMenuItem4;

	private ToolStripMenuItem checkForUpdateToolStripMenuItem1;

	private TextBox filterTextbox;

	private TabControl editPanel;

	private TabPage generelTab;

	private TabPage SSLTab;

	private Button createSelfSignedCertificateBtn;

	private Label certificateFilePath;

	private Button chooseCertificateBtn;

	private Label certificateFileLbl;

	private CheckBox enableSSL;

	private OpenFileDialog chooseCertificateDialog;

	private Panel panel1;

	private Label certificateKeyPath;

	private Button chooseCertificateKeyBtn;

	private Label certificateKeyLbl;

	private OpenFileDialog chooseCertificateKeyDialog;

	private Label phpVersionLbl;

	private ComboBox phpVersion;

	private ToolStripMenuItem defaultXAMPPToolStripMenuItem1;

	private TabPage aliasesTab;

	private Button createAliasBtn;

	private ListView aliases;

	private ColumnHeader columnAliasName;

	private ColumnHeader columnAliasPath;

	private Button deleteAliasBtn;

	private Button editAliasBtn;

	private ToolStripMenuItem reportBugToolStripMenuItem;

	private TextBox port;

	private Label portLbl;

	private Panel panel2;

	private Label customPortDescriptionLbl;

	private ToolStripMenuItem apacheLogToolStripMenuItem;

	private ToolStripMenuItem accessLogToolStripMenuItem;

	private TabPage advancedTab;

	private Panel panel3;

	private Button optionsResetBtn;

	private CheckBox optionsExecuteCGI;

	private CheckBox optionsSymlinksIfOwnerMatch;

	private CheckBox optionsFollowSymlinks;

	private CheckBox optionsIncludes;

	private CheckBox optionsIndexes;

	private Label optionsForDirectoryLbl;

	private Label directoryOptionsDescriptionLbl;

	private ListView apacheSettings;

	private Label customApacheSettingsLbl;

	private ContextMenuStrip apacheSettingsContextMenu;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem editToolStripMenuItem1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ColumnHeader columnHeader3;

	private CheckBox ftpStatus;

	private CheckBox ftpEnabled;

	private Panel panel4;

	private ToolStripMenuItem fTPToolStripMenuItem;

	private ListViewCustomControl vhosts;

	private ColumnHeader columnHeader1;

	private ColumnHeader columnHeader2;

	private ToolStripMenuItem donateToolStripMenuItem;

	private Button showWebsiteBtn;

	protected string CertificateFilePath { get; set; }

	protected string CertificateKeyPath { get; set; }

	protected bool NewChanges { get; set; }

	protected Vhost CurrentVhost { get; set; }

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public sealed override Color BackColor
	{
		get
		{
			return base.BackColor;
		}
		set
		{
			base.BackColor = value;
		}
	}

	public Editor()
	{
		InitializeComponent();
		if (XWM.Application.Config.Instance.XamppDirectory == null || XWM.Application.Config.Instance.XamppDirectory.Trim() == string.Empty || !Directory.Exists(XWM.Application.Config.Instance.XamppDirectory))
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorXAMPPNotFoundChooseManually"), XWM.Application.Config.Instance.GetTranslation("Information"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			new Settings().Show(this);
		}
		object @object = Resources.GetObject("logo");
		if (@object != null)
		{
			logo.Image = (Bitmap)@object;
		}
		object object2 = Resources.GetObject("power");
		if (object2 != null)
		{
			powerPicture.BackgroundImage = (Bitmap)object2;
		}
		object object3 = Resources.GetObject("phpmyadmin");
		if (object3 != null)
		{
			phpmyadminPicture.Image = (Bitmap)object3;
		}
		object object4 = Resources.GetObject("header_top");
		if (object4 != null)
		{
			headerPanel.BackgroundImage = (Bitmap)object4;
			menuPanel.BackgroundImage = (Bitmap)object4;
		}
		base.Icon = (Icon)Resources.GetObject("ico");
		filesBtn.Text = XWM.Application.Config.Instance.GetTranslation("Files");
		settingsBtn.Text = XWM.Application.Config.Instance.GetTranslation("Settings");
		settingsToolStripMenuItem2.Text = XWM.Application.Config.Instance.GetTranslation("Settings");
		helpBtn.Text = XWM.Application.Config.Instance.GetTranslation("Help");
		exitToolStripMenuItem1.Text = XWM.Application.Config.Instance.GetTranslation("Exit");
		settingsToolStripMenuItem1.Text = XWM.Application.Config.Instance.GetTranslation("ConfigurationFiles");
		logsToolStripMenuItem2.Text = XWM.Application.Config.Instance.GetTranslation("Logs");
		checkForUpdateToolStripMenuItem1.Text = XWM.Application.Config.Instance.GetTranslation("CheckForUpdate");
		aboutToolStripMenuItem1.Text = XWM.Application.Config.Instance.GetTranslation("About");
		serverNameLbl.Text = XWM.Application.Config.Instance.GetTranslation("ServerName");
		documentRootLbl.Text = XWM.Application.Config.Instance.GetTranslation("DocumentRoot");
		browseBtn.Text = XWM.Application.Config.Instance.GetTranslation("Browse");
		saveBtn.Text = XWM.Application.Config.Instance.GetTranslation("Save");
		serverStatusTxt.Text = XWM.Application.Config.Instance.GetTranslation("StartServers");
		filterTextbox.Text = XWM.Application.Config.Instance.GetTranslation("Filter");
		chooseCertificateDialog.Title = XWM.Application.Config.Instance.GetTranslation("ChooseCertificate");
		chooseCertificateKeyDialog.Title = XWM.Application.Config.Instance.GetTranslation("ChooseCertificate");
		generelTab.Text = XWM.Application.Config.Instance.GetTranslation("Generel");
		SSLTab.Text = XWM.Application.Config.Instance.GetTranslation("SSL");
		aliasesTab.Text = XWM.Application.Config.Instance.GetTranslation("Aliases");
		advancedTab.Text = XWM.Application.Config.Instance.GetTranslation("Advanced");
		enableSSL.Text = XWM.Application.Config.Instance.GetTranslation("EnableSSL");
		certificateFileLbl.Text = XWM.Application.Config.Instance.GetTranslation("Certificate");
		certificateKeyLbl.Text = XWM.Application.Config.Instance.GetTranslation("CertificateKey");
		chooseCertificateBtn.Text = XWM.Application.Config.Instance.GetTranslation("Browse");
		chooseCertificateKeyBtn.Text = XWM.Application.Config.Instance.GetTranslation("Browse");
		createSelfSignedCertificateBtn.Text = XWM.Application.Config.Instance.GetTranslation("CreateSelfsignedCertificate");
		aliases.Columns[0].Text = XWM.Application.Config.Instance.GetTranslation("Name");
		aliases.Columns[1].Text = XWM.Application.Config.Instance.GetTranslation("Directory");
		editAliasBtn.Text = XWM.Application.Config.Instance.GetTranslation("Edit");
		reportBugToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("ReportBug");
		portLbl.Text = XWM.Application.Config.Instance.GetTranslation("CustomPort");
		customPortDescriptionLbl.Text = XWM.Application.Config.Instance.GetTranslation("CustomPortDescription");
		optionsForDirectoryLbl.Text = XWM.Application.Config.Instance.GetTranslation("OptionsForDirectory");
		directoryOptionsDescriptionLbl.Text = XWM.Application.Config.Instance.GetTranslation("OptionsForDirectoryDescription");
		optionsResetBtn.Text = XWM.Application.Config.Instance.GetTranslation("UseDefaults");
		customApacheSettingsLbl.Text = XWM.Application.Config.Instance.GetTranslation("CustomApacheSettings");
		addToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("Add");
		editToolStripMenuItem1.Text = XWM.Application.Config.Instance.GetTranslation("Edit");
		deleteToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("Delete");
		ftpEnabled.Text = XWM.Application.Config.Instance.GetTranslation("EnableFTP");
		showWebsiteBtn.Text = XWM.Application.Config.Instance.GetTranslation("ShowWebsite");
		donateToolStripMenuItem.Text = XWM.Application.Config.Instance.GetTranslation("Donate");
		vhosts.Columns[0].Text = XWM.Application.Config.Instance.GetTranslation("ServerName");
		vhosts.Columns[1].Text = XWM.Application.Config.Instance.GetTranslation("DocumentRoot");
		filesBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
		settingsBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
		helpBtn.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
		Text = System.Windows.Forms.Application.ProductName + " " + System.Windows.Forms.Application.ProductVersion;
		BackColor = Color.FromArgb(242, 242, 242);
		editPanel.BackColor = Color.FromArgb(233, 233, 233);
		foreach (Vhost vhost in XWM.Application.Config.Instance.Vhosts)
		{
			ListViewItem listViewItem = new ListViewItem
			{
				Text = vhost.ServerName
			};
			listViewItem.SubItems.Add(vhost.DocumentRoot);
			vhosts.Items.Add(listViewItem);
		}
		base.KeyUp += OnKeyUp;
		appStatus.Start();
		menuPanel.Visible = false;
		base.Width = XWM.Application.Config.Instance.ApplicationWidth;
		base.Height = XWM.Application.Config.Instance.ApplicationHeight;
		serverStatus.Start();
		filterTextbox.GotFocus += delegate(object sender, System.EventArgs args)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox.Text == XWM.Application.Config.Instance.GetTranslation("Filter") || textBox.Text.Trim() == string.Empty)
			{
				textBox.ForeColor = Color.Black;
				textBox.Text = "";
			}
		};
		filterTextbox.LostFocus += delegate(object sender, System.EventArgs args)
		{
			TextBox textBox2 = (TextBox)sender;
			if (textBox2.Text == XWM.Application.Config.Instance.GetTranslation("Filter") || textBox2.Text.Trim() == string.Empty)
			{
				textBox2.ForeColor = Color.Gray;
				textBox2.Text = XWM.Application.Config.Instance.GetTranslation("Filter");
			}
		};
		filterTextbox.KeyUp += delegate(object sender, KeyEventArgs args)
		{
			TextBox txt = (TextBox)sender;
			IEnumerable<Vhost> enumerable = XWM.Application.Config.Instance.Vhosts.Where((Vhost v) => v.ServerName.Contains(txt.Text.Trim().ToLower()) || v.DocumentRoot.ToLower().Contains(txt.Text.Trim().ToLower()));
			vhosts.Items.Clear();
			foreach (Vhost item in enumerable)
			{
				vhosts.Items.Add(new ListViewItem
				{
					Text = item.ServerName,
					SubItems = { item.DocumentRoot }
				});
			}
		};
		apacheSettings.DoubleClick += delegate
		{
			if (apacheSettings.SelectedItems.Count > 0)
			{
				apacheSettings.SelectedItems[0].BeginEdit();
			}
		};
		apacheSettings.AfterLabelEdit += delegate(object sender, LabelEditEventArgs args)
		{
			if (string.IsNullOrEmpty(args.Label))
			{
				args.CancelEdit = true;
				apacheSettings.SelectedItems[0].Remove();
			}
		};
		base.Click += delegate
		{
			serverStatusTxt.Focus();
		};
		chooseCertificateDialog.Filter = XWM.Application.Config.Instance.GetTranslation("Certificates") + " | *.cert";
		chooseCertificateDialog.DefaultExt = "cert";
		chooseCertificateKeyDialog.Filter = XWM.Application.Config.Instance.GetTranslation("Certificates") + " | *.key";
		chooseCertificateKeyDialog.DefaultExt = "key";
		foreach (object value in Enum.GetValues(typeof(PhpConfiguration.PhpVersion)))
		{
			PhpConfiguration.PhpVersion v2 = (PhpConfiguration.PhpVersion)value;
			DescriptionAttribute[] array = (DescriptionAttribute[])value.GetType().GetField(v2.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
			string text = ((array.Length != 0) ? array[0].Description : value.ToString());
			pHPToolStripMenuItem3.DropDownItems.Add(text, null, delegate
			{
				Main.ShowConfigurationEditor(PhpConfiguration.GetIni(v2));
			});
		}
		ResetEditorForm();
		CheckUpdate(silent: true);
	}

	private void OnKeyUp(object sender, KeyEventArgs keyEventArgs)
	{
		if (keyEventArgs.KeyValue == 18)
		{
			menuPanel.Visible = !menuPanel.Visible;
		}
	}

	private void serverStatus_Tick(object sender, System.EventArgs e)
	{
		apacheStatus.Checked = XWM.Xampp.Config.ApacheRunning();
		mysqlStatus.Checked = XWM.Xampp.Config.MysqlRunning();
		ftpStatus.Checked = Main.Instance().FtpRunning();
		serverStatusTxt.Text = ((Main.Instance().FtpRunning() || XWM.Xampp.Config.ApacheRunning() || XWM.Xampp.Config.MysqlRunning()) ? XWM.Application.Config.Instance.GetTranslation("StopServers") : XWM.Application.Config.Instance.GetTranslation("StartServers"));
	}

	private void deleteVhostBtn_Click(object sender, System.EventArgs e)
	{
		if (MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ConfirmDeleteVirtualHost", vhosts.SelectedItems[0].Text), XWM.Application.Config.Instance.GetTranslation("ConfirmDeletion"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		ListViewItem selected = vhosts.SelectedItems[0];
		if (selected.Selected)
		{
			Vhost vhost = XWM.Application.Config.Instance.Vhosts.FirstOrDefault((Vhost v) => v.ServerName.ToLower() == selected.Text.ToLower());
			if (vhost != null)
			{
				XWM.Application.Config.Instance.Vhosts.Remove(vhost);
				vhosts.Items.Remove(selected);
				XWM.Application.Config.Instance.SaveVhosts();
				XWM.Xampp.Config.RestartRequired();
			}
		}
	}

	protected void DefaultOptions()
	{
		optionsIndexes.Checked = true;
		optionsIncludes.Checked = true;
		optionsFollowSymlinks.Checked = true;
		optionsExecuteCGI.Checked = true;
		optionsSymlinksIfOwnerMatch.Checked = false;
	}

	protected void ResetEditorForm()
	{
		showWebsiteBtn.Visible = false;
		NewChanges = true;
		enableSSL.Checked = false;
		CertificateKeyPath = string.Empty;
		CertificateFilePath = string.Empty;
		CurrentVhost = null;
		certificateFilePath.Text = XWM.Application.Config.Instance.GetTranslation("NoCertificateChosen");
		certificateKeyPath.Text = XWM.Application.Config.Instance.GetTranslation("NoCertificateChosen");
		documentRoot.Text = string.Empty;
		serverName.Text = string.Empty;
		vhosts.SelectedItems.Clear();
		DefaultOptions();
		apacheSettings.Items.Clear();
		phpVersion.Items.Clear();
		phpVersion.Items.Add(XWM.Application.Config.Instance.GetTranslation("PHPVersionDefault"));
		foreach (object value in Enum.GetValues(typeof(PhpConfiguration.PhpVersion)))
		{
			DescriptionAttribute[] array = (DescriptionAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
			phpVersion.Items.Add((array.Length != 0) ? array[0].Description : value.ToString());
		}
		phpVersion.SelectedIndex = 0;
		aliases.Items.Clear();
	}

	private void addVhostBtn_Click(object sender, System.EventArgs e)
	{
		ResetEditorForm();
		serverName.Focus();
	}

	private void browseBtn_Click(object sender, System.EventArgs e)
	{
		if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
		{
			documentRoot.Text = folderBrowserDialog1.SelectedPath;
		}
	}

	protected void SetVhostData(ref Vhost vhost)
	{
		vhost.ServerName = serverName.Text.Trim();
		vhost.DocumentRoot = documentRoot.Text.Trim();
		vhost.SslEnabled = enableSSL.Checked;
		vhost.CertificateFilePath = CertificateFilePath;
		vhost.CertificateKeyPath = CertificateKeyPath;
		vhost.PhpVersion = null;
		vhost.Port = null;
		if (port.TextLength > 0 && int.TryParse(port.Text, out var result) && !new int[2] { 80, 443 }.Contains(result))
		{
			vhost.Port = result;
		}
		if (phpVersion.SelectedIndex > 0)
		{
			Enum.TryParse<PhpConfiguration.PhpVersion>(Enum.GetNames(typeof(PhpConfiguration.PhpVersion))[phpVersion.SelectedIndex - 1], ignoreCase: true, out var result2);
			vhost.PhpVersion = result2;
		}
		vhost.Aliases.Clear();
		if (aliases.Items.Count > 0)
		{
			foreach (ListViewItem item2 in aliases.Items)
			{
				HostAlias item = new HostAlias
				{
					AliasName = item2.Text,
					Directory = item2.SubItems[1].Text
				};
				vhost.Aliases.Add(item);
			}
		}
		vhost.CustomSettings = new List<string>();
		if (apacheSettings.Items.Count > 0)
		{
			foreach (ListViewItem item3 in apacheSettings.Items)
			{
				vhost.CustomSettings.Add(item3.Text);
			}
		}
		vhost.DirectoryIndex = optionsIndexes.Checked;
		vhost.ExecuteCgi = optionsExecuteCGI.Checked;
		vhost.FollowSymLinks = optionsFollowSymlinks.Checked;
		vhost.SymLinksIfOwnerMatch = optionsSymlinksIfOwnerMatch.Checked;
		vhost.Includes = optionsIncludes.Checked;
		vhost.EnableFtp = ftpEnabled.Checked;
	}

	private void saveBtn_Click(object sender, System.EventArgs e)
	{
		if (string.IsNullOrEmpty(serverName.Text.Trim()))
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorServerNameNotSpecified"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (string.IsNullOrEmpty(documentRoot.Text.Trim()))
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorSelectDocumentRoot"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (!Directory.Exists(documentRoot.Text.Trim()))
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorInvalidDocumentRoot"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (vhosts.SelectedItems.Count == 0 && XWM.Application.Config.Instance.Vhosts.FirstOrDefault((Vhost v) => v.ServerName.Trim().ToLower() == serverName.Text.Trim().ToLower()) != null)
		{
			MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorServerNameExists"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (vhosts.SelectedItems.Count > 0)
		{
			Vhost vhost = XWM.Application.Config.Instance.Vhosts.FirstOrDefault((Vhost v) => v.ServerName.Trim().ToLower() == vhosts.SelectedItems[0].Text.ToLower());
			if (vhost != null)
			{
				SetVhostData(ref vhost);
				vhosts.SelectedItems[0].Text = serverName.Text;
				vhosts.SelectedItems[0].SubItems[1].Text = documentRoot.Text;
			}
		}
		else
		{
			Vhost vhost2 = new Vhost();
			SetVhostData(ref vhost2);
			XWM.Application.Config.Instance.Vhosts.Add(vhost2);
			ListViewItem listViewItem = new ListViewItem
			{
				Text = vhost2.ServerName
			};
			listViewItem.SubItems.Add(vhost2.DocumentRoot);
			vhosts.Items.Add(listViewItem);
		}
		XWM.Application.Config.Instance.SaveVhosts();
		XWM.Xampp.Config.RestartRequired();
	}

	private void vhosts_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		if (vhosts.SelectedItems.Count == 0)
		{
			ResetEditorForm();
			return;
		}
		CurrentVhost = XWM.Application.Config.Instance.Vhosts.FirstOrDefault((Vhost v) => v.ServerName.Trim().ToLower() == vhosts.SelectedItems[0].Text.Trim().ToLower());
		if (CurrentVhost == null)
		{
			return;
		}
		showWebsiteBtn.Visible = true;
		NewChanges = false;
		serverName.Text = CurrentVhost.ServerName.Trim();
		documentRoot.Text = CurrentVhost.DocumentRoot.Trim();
		enableSSL.Checked = CurrentVhost.SslEnabled;
		certificateFilePath.Text = CurrentVhost.CertificateFilePath ?? XWM.Application.Config.Instance.GetTranslation("NoCertificateChosen");
		certificateKeyPath.Text = CurrentVhost.CertificateKeyPath ?? XWM.Application.Config.Instance.GetTranslation("NoCertificateChosen");
		CertificateFilePath = CurrentVhost.CertificateFilePath;
		CertificateKeyPath = CurrentVhost.CertificateKeyPath;
		optionsIncludes.Checked = CurrentVhost.Includes;
		optionsFollowSymlinks.Checked = CurrentVhost.FollowSymLinks;
		optionsExecuteCGI.Checked = CurrentVhost.ExecuteCgi;
		optionsSymlinksIfOwnerMatch.Checked = CurrentVhost.SymLinksIfOwnerMatch;
		optionsIndexes.Checked = CurrentVhost.DirectoryIndex;
		ftpEnabled.Checked = CurrentVhost.EnableFtp;
		apacheSettings.Items.Clear();
		if (CurrentVhost.CustomSettings.Count > 0)
		{
			foreach (string customSetting in CurrentVhost.CustomSettings)
			{
				apacheSettings.Items.Add(customSetting);
			}
		}
		port.Text = (CurrentVhost.Port.HasValue ? CurrentVhost.Port.ToString() : string.Empty);
		phpVersion.SelectedIndex = 0;
		if (CurrentVhost.PhpVersion.HasValue)
		{
			for (int i = 0; i < Enum.GetValues(typeof(PhpConfiguration.PhpVersion)).Length; i++)
			{
				if ((PhpConfiguration.PhpVersion)Enum.Parse(typeof(PhpConfiguration.PhpVersion), Enum.GetNames(typeof(PhpConfiguration.PhpVersion))[i]) == CurrentVhost.PhpVersion)
				{
					phpVersion.SelectedIndex = i + 1;
					break;
				}
			}
		}
		if (CurrentVhost.Aliases.Count <= 0)
		{
			return;
		}
		foreach (HostAlias alias in CurrentVhost.Aliases)
		{
			ListViewItem value = new ListViewItem
			{
				Text = alias.AliasName,
				SubItems = { alias.Directory }
			};
			aliases.Items.Add(value);
		}
	}

	private void pictureBox2_Click(object sender, System.EventArgs e)
	{
		if (XWM.Xampp.Config.MysqlRunning() || XWM.Xampp.Config.ApacheRunning())
		{
			XWM.Xampp.Config.StopServers();
		}
		else
		{
			XWM.Xampp.Config.StartServers();
		}
	}

	private void app_Tick(object sender, System.EventArgs e)
	{
		deleteVhostBtn.Enabled = vhosts.SelectedItems.Count > 0;
		bool enabled = documentRoot.Text.Trim().Length > 0 && serverName.Text.Trim().Length > 0;
		if ((enableSSL.Checked && (CertificateFilePath == null || CertificateKeyPath == null)) || !Directory.Exists(documentRoot.Text))
		{
			enabled = false;
		}
		deleteAliasBtn.Enabled = aliases.SelectedItems.Count > 0;
		editAliasBtn.Enabled = aliases.SelectedItems.Count > 0;
		chooseCertificateBtn.Enabled = enableSSL.Checked;
		chooseCertificateKeyBtn.Enabled = enableSSL.Checked;
		createSelfSignedCertificateBtn.Enabled = enableSSL.Checked;
		saveBtn.Enabled = enabled;
	}

	private void pictureBox3_Click(object sender, System.EventArgs e)
	{
		if (!XWM.Xampp.Config.ApacheRunning() || !XWM.Xampp.Config.MysqlRunning())
		{
			if (MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ConfirmStartServers"), XWM.Application.Config.Instance.GetTranslation("StartServers"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			XWM.Xampp.Config.StartServers();
			Thread.Sleep(1000);
		}
		new Thread((ThreadStart)delegate
		{
			Process.Start("http://localhost/phpmyadmin");
		}).Start();
	}

	private void button2_Click(object sender, System.EventArgs e)
	{
		Button button = (Button)sender;
		Point screenLocation = new Point(base.Left + button.Left + (button.Width - helpContextMenu.Width / 2), base.Top + headerPanel.Height + 17);
		settingsContextMenu.Show(screenLocation);
	}

	private void button1_Click(object sender, System.EventArgs e)
	{
		Button button = (Button)sender;
		Point screenLocation = new Point(base.Left + button.Left + (button.Width - helpContextMenu.Width / 2), base.Top + headerPanel.Height + 17);
		filesContextMenu.Show(screenLocation);
	}

	private void button3_Click(object sender, System.EventArgs e)
	{
		Button button = (Button)sender;
		Point screenLocation = new Point(base.Left + button.Left + (button.Width - helpContextMenu.Width / 2), base.Top + headerPanel.Height + 17);
		helpContextMenu.Show(screenLocation);
	}

	private void exitToolStripMenuItem1_Click(object sender, System.EventArgs e)
	{
		System.Windows.Forms.Application.Exit();
	}

	private void apacheToolStripMenuItem4_Click(object sender, System.EventArgs e)
	{
		Main.ShowFile(XWM.Application.Config.Instance.GetTranslation("TypeConfiguration", "Apache"), "\\apache\\conf\\httpd.conf");
	}

	private void mySQLToolStripMenuItem3_Click(object sender, System.EventArgs e)
	{
		Main.ShowConfigurationEditor(Mysql.GetConfigurationFile());
	}

	private void pHPToolStripMenuItem4_Click_1(object sender, System.EventArgs e)
	{
		Main.ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\php\\logs\\php_error_log");
	}

	private void mySQLToolStripMenuItem4_Click(object sender, System.EventArgs e)
	{
		Main.ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\mysql\\data\\mysql_error.log");
	}

	private void settingsToolStripMenuItem2_Click(object sender, System.EventArgs e)
	{
		new Settings().ShowDialog();
	}

	private void aboutToolStripMenuItem1_Click(object sender, System.EventArgs e)
	{
		new About().ShowDialog();
	}

	protected void CheckUpdate(bool silent)
	{
		/*try
		{
			Uri address = new Uri("http://service.pecee.dk/wm/check");
			using WebClient webClient = new WebClient();
			webClient.DownloadStringCompleted += delegate(object o, DownloadStringCompletedEventArgs args)
			{
				if (args.Result != null)
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.LoadXml(args.Result);
					if (xmlDocument.DocumentElement != null)
					{
						XmlNode xmlNode = xmlDocument.DocumentElement.SelectSingleNode("/info/version");
						XmlNode xmlNode2 = xmlDocument.DocumentElement.SelectSingleNode("/info/url");
						if (xmlNode != null && xmlNode2 != null)
						{
							int num = int.Parse(System.Windows.Forms.Application.ProductVersion.Replace(".", ""));
							if (int.Parse(xmlNode.InnerText.Replace(".", "")) > num)
							{
								new Update().ShowDialog(this);
							}
							else if (!silent)
							{
								MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("NoAvailableDialog"), XWM.Application.Config.Instance.GetTranslation("Information"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
							}
						}
					}
				}
			};
			webClient.DownloadStringAsync(address);
		}
		catch (Exception)
		{
			if (!silent)
			{
				MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("ErrorFailedCheckForUpdates"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}*/
	}

	private void checkForUpdateToolStripMenuItem1_Click(object sender, System.EventArgs e)
	{
		CheckUpdate(silent: false);
	}

	protected void SetCertificates(CreateCertificateEventArgs args)
	{
		NewChanges = true;
		certificateFilePath.Text = args.CertificateFilePath;
		certificateKeyPath.Text = args.CertificateKeyPath;
		CertificateFilePath = args.CertificateFilePath;
		CertificateKeyPath = args.CertificateKeyPath;
		CreateCertificate.Instance().Close();
	}

	private void createSelfSignedCertificateBtn_Click(object sender, System.EventArgs e)
	{
		CreateCertificate createCertificate = CreateCertificate.Instance();
		createCertificate.CertificateCreated += delegate(object o, CreateCertificateEventArgs args)
		{
			if (base.InvokeRequired)
			{
				CertificateCallback method = SetCertificates;
				Invoke(method, args);
			}
			else
			{
				SetCertificates(args);
			}
		};
		createCertificate.ShowDialog(this);
	}

	private void chooseCertificateBtn_Click(object sender, System.EventArgs e)
	{
		if (chooseCertificateDialog.ShowDialog(this) == DialogResult.OK)
		{
			NewChanges = true;
			certificateFilePath.Text = chooseCertificateDialog.FileName;
			CertificateFilePath = chooseCertificateDialog.FileName;
		}
	}

	private void chooseCertificateKeyBtn_Click(object sender, System.EventArgs e)
	{
		if (chooseCertificateKeyDialog.ShowDialog(this) == DialogResult.OK)
		{
			NewChanges = true;
			certificateKeyPath.Text = chooseCertificateKeyDialog.FileName;
			CertificateKeyPath = chooseCertificateKeyDialog.FileName;
		}
	}

	private void defaultXAMPPToolStripMenuItem1_Click(object sender, System.EventArgs e)
	{
		Main.ShowConfigurationEditor(XWM.Application.Config.Instance.XamppDirectory + "\\php\\php.ini");
	}

	private void phpVersion_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		if (phpVersion.SelectedIndex <= 0 || CurrentVhost != null || XWM.Application.Config.Instance.Php7WarningShowed)
		{
			return;
		}
		string text = Enum.GetNames(typeof(PhpConfiguration.PhpVersion))[phpVersion.SelectedIndex - 1];
		if (text != null && text == PhpConfiguration.PhpVersion.Php700Rc3.ToString())
		{
			if (MessageBox.Show(XWM.Application.Config.Instance.GetTranslation("PHP7WarningMessage"), XWM.Application.Config.Instance.GetTranslation("PHP7WarningMessageTitle"), MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
			{
				Process.Start("http://www.microsoft.com/en-us/download/details.aspx?id=46881");
			}
			XWM.Application.Config.Instance.Php7WarningShowed = true;
		}
	}

	private void deleteAliasBtn_Click(object sender, System.EventArgs e)
	{
		if (aliases.SelectedItems.Count <= 0)
		{
			return;
		}
		if (vhosts.SelectedItems.Count > 0)
		{
			Vhost vhost = XWM.Application.Config.Instance.Vhosts.FirstOrDefault((Vhost v) => v.ServerName.ToLower() == vhosts.SelectedItems[0].Text.ToLower());
			if (vhost != null)
			{
				HostAlias hostAlias = vhost.Aliases.FirstOrDefault((HostAlias a) => a.AliasName.ToLower() == aliases.SelectedItems[0].Text.ToLower() && a.Directory.ToLower() == aliases.SelectedItems[0].SubItems[1].Text.ToLower());
				if (hostAlias != null)
				{
					vhost.Aliases.Remove(hostAlias);
				}
			}
		}
		aliases.SelectedItems[0].Remove();
	}

	private void createAliasBtn_Click(object sender, System.EventArgs e)
	{
		CreateAlias createAlias = new CreateAlias();
		createAlias.AliasCreated += delegate(object o, CreateAliasEventArgs args)
		{
			ListViewItem listViewItem = new ListViewItem
			{
				Text = args.AliasName
			};
			listViewItem.SubItems.Add(args.DirectoryPath);
			aliases.Items.Add(listViewItem);
		};
		createAlias.ShowDialog(this);
	}

	private void editAliasBtn_Click(object sender, System.EventArgs e)
	{
		if (aliases.SelectedItems.Count > 0)
		{
			CreateAlias createAlias = new CreateAlias(new HostAlias
			{
				AliasName = aliases.SelectedItems[0].Text,
				Directory = aliases.SelectedItems[0].SubItems[1].Text
			});
			createAlias.AliasCreated += delegate(object o, CreateAliasEventArgs args)
			{
				NewChanges = true;
				aliases.SelectedItems[0].Text = args.AliasName;
				aliases.SelectedItems[0].SubItems[1].Text = args.DirectoryPath;
			};
			createAlias.ShowDialog(this);
		}
	}

	private void reportBugToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		Process.Start("https://github.com/skipperbent/XAMPP-Website-Manager/issues");
	}

	private void apacheLogToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		Main.ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\apache\\logs\\error.log");
	}

	private void accessLogToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		Main.ShowLogViewer(XWM.Application.Config.Instance.XamppDirectory + "\\apache\\logs\\access.log");
	}

	private void optionsResetBtn_Click(object sender, System.EventArgs e)
	{
		DefaultOptions();
	}

	private void apacheSettingsContextMenu_Opening(object sender, CancelEventArgs e)
	{
		addToolStripMenuItem.Visible = apacheSettings.SelectedItems.Count == 0;
		deleteToolStripMenuItem.Visible = apacheSettings.SelectedItems.Count > 0;
		editToolStripMenuItem1.Visible = apacheSettings.SelectedItems.Count == 1;
	}

	private void editToolStripMenuItem1_Click(object sender, System.EventArgs e)
	{
		apacheSettings.SelectedItems[0].BeginEdit();
	}

	private void addToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		apacheSettings.Items.Add(string.Empty);
		apacheSettings.Items[apacheSettings.Items.Count - 1].BeginEdit();
	}

	private void deleteToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		foreach (ListViewItem selectedItem in apacheSettings.SelectedItems)
		{
			selectedItem.Remove();
		}
	}

	private void fTPToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		Main.ShowLogViewer("logs\\ftp.log");
	}

	private void Editor_Load(object sender, System.EventArgs e)
	{
		vhosts.AfterItemEdit += VhostsOnAfterItemEdit;
	}

	private void VhostsOnAfterItemEdit(object sender, CustomLabelEditEventArgs args)
	{
		ListViewItem listViewItem = vhosts.Items[args.Item];
		CurrentVhost.ServerName = listViewItem.Text.Trim();
		if (args.Index == 1)
		{
			if (Directory.Exists(args.Label))
			{
				CurrentVhost.DocumentRoot = listViewItem.SubItems[0].Text.Trim();
			}
			else
			{
				args.CancelEdit = true;
			}
		}
	}

	private void donateToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=NNX4D2RUSALCN");
	}

	private void showWebsiteBtn_Click(object sender, System.EventArgs e)
	{
		if (CurrentVhost != null)
		{
			string arg = ((CurrentVhost.Port == 443) ? "https" : "http");
			Process.Start($"{arg}://{CurrentVhost.ServerName}/");
		}
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
		System.Windows.Forms.ListViewItem listViewItem = new System.Windows.Forms.ListViewItem("SetEnv HTTP_HOST test.dk");
		this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.phpiniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.apacheToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.mycnfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pHPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.apacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.mySQLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.browseBtn = new System.Windows.Forms.Button();
		this.documentRootLbl = new System.Windows.Forms.Label();
		this.documentRoot = new System.Windows.Forms.TextBox();
		this.serverNameLbl = new System.Windows.Forms.Label();
		this.serverName = new System.Windows.Forms.TextBox();
		this.saveBtn = new System.Windows.Forms.Button();
		this.serverStatus = new System.Windows.Forms.Timer(this.components);
		this.headerPanel = new System.Windows.Forms.Panel();
		this.ftpStatus = new System.Windows.Forms.CheckBox();
		this.menuPanel = new System.Windows.Forms.Panel();
		this.helpBtn = new System.Windows.Forms.Button();
		this.settingsBtn = new System.Windows.Forms.Button();
		this.filesBtn = new System.Windows.Forms.Button();
		this.label3 = new System.Windows.Forms.Label();
		this.serverStatusTxt = new System.Windows.Forms.Label();
		this.apacheStatus = new System.Windows.Forms.CheckBox();
		this.mysqlStatus = new System.Windows.Forms.CheckBox();
		this.logo = new System.Windows.Forms.PictureBox();
		this.powerPicture = new System.Windows.Forms.PictureBox();
		this.phpmyadminPicture = new System.Windows.Forms.PictureBox();
		this.addVhostBtn = new System.Windows.Forms.Button();
		this.deleteVhostBtn = new System.Windows.Forms.Button();
		this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
		this.appStatus = new System.Windows.Forms.Timer(this.components);
		this.filesContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.exitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.settingsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.pHPToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
		this.defaultXAMPPToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.apacheToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
		this.mySQLToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
		this.logsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.pHPToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
		this.apacheToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
		this.apacheLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.accessLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.mySQLToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
		this.fTPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.settingsToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.helpContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.checkForUpdateToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.reportBugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.filterTextbox = new System.Windows.Forms.TextBox();
		this.editPanel = new System.Windows.Forms.TabControl();
		this.generelTab = new System.Windows.Forms.TabPage();
		this.ftpEnabled = new System.Windows.Forms.CheckBox();
		this.panel4 = new System.Windows.Forms.Panel();
		this.customPortDescriptionLbl = new System.Windows.Forms.Label();
		this.port = new System.Windows.Forms.TextBox();
		this.portLbl = new System.Windows.Forms.Label();
		this.panel2 = new System.Windows.Forms.Panel();
		this.phpVersionLbl = new System.Windows.Forms.Label();
		this.phpVersion = new System.Windows.Forms.ComboBox();
		this.SSLTab = new System.Windows.Forms.TabPage();
		this.certificateKeyPath = new System.Windows.Forms.Label();
		this.chooseCertificateKeyBtn = new System.Windows.Forms.Button();
		this.certificateKeyLbl = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.createSelfSignedCertificateBtn = new System.Windows.Forms.Button();
		this.certificateFilePath = new System.Windows.Forms.Label();
		this.chooseCertificateBtn = new System.Windows.Forms.Button();
		this.certificateFileLbl = new System.Windows.Forms.Label();
		this.enableSSL = new System.Windows.Forms.CheckBox();
		this.aliasesTab = new System.Windows.Forms.TabPage();
		this.editAliasBtn = new System.Windows.Forms.Button();
		this.deleteAliasBtn = new System.Windows.Forms.Button();
		this.createAliasBtn = new System.Windows.Forms.Button();
		this.aliases = new System.Windows.Forms.ListView();
		this.columnAliasName = new System.Windows.Forms.ColumnHeader();
		this.columnAliasPath = new System.Windows.Forms.ColumnHeader();
		this.advancedTab = new System.Windows.Forms.TabPage();
		this.apacheSettings = new System.Windows.Forms.ListView();
		this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
		this.apacheSettingsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.customApacheSettingsLbl = new System.Windows.Forms.Label();
		this.panel3 = new System.Windows.Forms.Panel();
		this.optionsResetBtn = new System.Windows.Forms.Button();
		this.optionsExecuteCGI = new System.Windows.Forms.CheckBox();
		this.optionsSymlinksIfOwnerMatch = new System.Windows.Forms.CheckBox();
		this.optionsFollowSymlinks = new System.Windows.Forms.CheckBox();
		this.optionsIncludes = new System.Windows.Forms.CheckBox();
		this.optionsIndexes = new System.Windows.Forms.CheckBox();
		this.optionsForDirectoryLbl = new System.Windows.Forms.Label();
		this.directoryOptionsDescriptionLbl = new System.Windows.Forms.Label();
		this.chooseCertificateDialog = new System.Windows.Forms.OpenFileDialog();
		this.chooseCertificateKeyDialog = new System.Windows.Forms.OpenFileDialog();
		this.showWebsiteBtn = new System.Windows.Forms.Button();
		this.vhosts = new XWM.CustomControls.ListViewCustomControl();
		this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
		this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
		this.headerPanel.SuspendLayout();
		this.menuPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.logo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.powerPicture).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.phpmyadminPicture).BeginInit();
		this.filesContextMenu.SuspendLayout();
		this.settingsContextMenu.SuspendLayout();
		this.helpContextMenu.SuspendLayout();
		this.editPanel.SuspendLayout();
		this.generelTab.SuspendLayout();
		this.SSLTab.SuspendLayout();
		this.aliasesTab.SuspendLayout();
		this.advancedTab.SuspendLayout();
		this.apacheSettingsContextMenu.SuspendLayout();
		base.SuspendLayout();
		this.filesToolStripMenuItem.AutoSize = false;
		this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.exitToolStripMenuItem });
		this.filesToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
		this.filesToolStripMenuItem.Size = new System.Drawing.Size(70, 58);
		this.filesToolStripMenuItem.Text = "Files";
		this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
		this.exitToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
		this.editToolStripMenuItem.AutoSize = false;
		this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.configurationToolStripMenuItem, this.logsToolStripMenuItem, this.settingsToolStripMenuItem });
		this.editToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.editToolStripMenuItem.Name = "editToolStripMenuItem";
		this.editToolStripMenuItem.Size = new System.Drawing.Size(70, 58);
		this.editToolStripMenuItem.Text = "Edit";
		this.configurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.phpiniToolStripMenuItem, this.apacheToolStripMenuItem1, this.mycnfToolStripMenuItem });
		this.configurationToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
		this.configurationToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
		this.configurationToolStripMenuItem.Text = "Configuration files";
		this.phpiniToolStripMenuItem.Name = "phpiniToolStripMenuItem";
		this.phpiniToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
		this.apacheToolStripMenuItem1.Name = "apacheToolStripMenuItem1";
		this.apacheToolStripMenuItem1.Size = new System.Drawing.Size(67, 22);
		this.mycnfToolStripMenuItem.Name = "mycnfToolStripMenuItem";
		this.mycnfToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
		this.logsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.pHPToolStripMenuItem, this.apacheToolStripMenuItem, this.mySQLToolStripMenuItem });
		this.logsToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
		this.logsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
		this.logsToolStripMenuItem.Text = "Logs";
		this.pHPToolStripMenuItem.Name = "pHPToolStripMenuItem";
		this.pHPToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
		this.apacheToolStripMenuItem.Name = "apacheToolStripMenuItem";
		this.apacheToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
		this.mySQLToolStripMenuItem.Name = "mySQLToolStripMenuItem";
		this.mySQLToolStripMenuItem.Size = new System.Drawing.Size(67, 22);
		this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
		this.settingsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
		this.helpToolStripMenuItem.AutoSize = false;
		this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.checkForUpdateToolStripMenuItem, this.aboutToolStripMenuItem });
		this.helpToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
		this.helpToolStripMenuItem.Size = new System.Drawing.Size(70, 58);
		this.helpToolStripMenuItem.Text = "Help";
		this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
		this.checkForUpdateToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
		this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
		this.aboutToolStripMenuItem.Size = new System.Drawing.Size(68, 22);
		this.browseBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.browseBtn.Location = new System.Drawing.Point(328, 72);
		this.browseBtn.Name = "browseBtn";
		this.browseBtn.Size = new System.Drawing.Size(75, 24);
		this.browseBtn.TabIndex = 4;
		this.browseBtn.Text = "Browse";
		this.browseBtn.UseVisualStyleBackColor = true;
		this.browseBtn.Click += new System.EventHandler(browseBtn_Click);
		this.documentRootLbl.BackColor = System.Drawing.Color.Transparent;
		this.documentRootLbl.Location = new System.Drawing.Point(21, 73);
		this.documentRootLbl.Name = "documentRootLbl";
		this.documentRootLbl.Size = new System.Drawing.Size(89, 21);
		this.documentRootLbl.TabIndex = 3;
		this.documentRootLbl.Text = "Document root";
		this.documentRootLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.documentRoot.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.documentRoot.Location = new System.Drawing.Point(119, 73);
		this.documentRoot.Name = "documentRoot";
		this.documentRoot.Size = new System.Drawing.Size(203, 22);
		this.documentRoot.TabIndex = 2;
		this.serverNameLbl.BackColor = System.Drawing.Color.Transparent;
		this.serverNameLbl.Location = new System.Drawing.Point(22, 31);
		this.serverNameLbl.Name = "serverNameLbl";
		this.serverNameLbl.Size = new System.Drawing.Size(89, 21);
		this.serverNameLbl.TabIndex = 1;
		this.serverNameLbl.Text = "Domain";
		this.serverNameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.serverName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.serverName.Location = new System.Drawing.Point(119, 31);
		this.serverName.Name = "serverName";
		this.serverName.Size = new System.Drawing.Size(283, 22);
		this.serverName.TabIndex = 0;
		this.saveBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.saveBtn.Enabled = false;
		this.saveBtn.Location = new System.Drawing.Point(762, 461);
		this.saveBtn.Name = "saveBtn";
		this.saveBtn.Size = new System.Drawing.Size(75, 23);
		this.saveBtn.TabIndex = 5;
		this.saveBtn.Text = "Save";
		this.saveBtn.UseVisualStyleBackColor = true;
		this.saveBtn.Click += new System.EventHandler(saveBtn_Click);
		this.serverStatus.Interval = 1500;
		this.serverStatus.Tick += new System.EventHandler(serverStatus_Tick);
		this.headerPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.headerPanel.BackColor = System.Drawing.Color.Gainsboro;
		this.headerPanel.Controls.Add(this.ftpStatus);
		this.headerPanel.Controls.Add(this.menuPanel);
		this.headerPanel.Controls.Add(this.label3);
		this.headerPanel.Controls.Add(this.serverStatusTxt);
		this.headerPanel.Controls.Add(this.apacheStatus);
		this.headerPanel.Controls.Add(this.mysqlStatus);
		this.headerPanel.Controls.Add(this.logo);
		this.headerPanel.Controls.Add(this.powerPicture);
		this.headerPanel.Controls.Add(this.phpmyadminPicture);
		this.headerPanel.Location = new System.Drawing.Point(-1, 0);
		this.headerPanel.Name = "headerPanel";
		this.headerPanel.Size = new System.Drawing.Size(853, 62);
		this.headerPanel.TabIndex = 9;
		this.ftpStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.ftpStatus.AutoSize = true;
		this.ftpStatus.BackColor = System.Drawing.Color.Transparent;
		this.ftpStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.ftpStatus.Enabled = false;
		this.ftpStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.ftpStatus.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.ftpStatus.Location = new System.Drawing.Point(801, 38);
		this.ftpStatus.Name = "ftpStatus";
		this.ftpStatus.Size = new System.Drawing.Size(37, 16);
		this.ftpStatus.TabIndex = 17;
		this.ftpStatus.Text = "FTP";
		this.ftpStatus.UseVisualStyleBackColor = false;
		this.menuPanel.Controls.Add(this.helpBtn);
		this.menuPanel.Controls.Add(this.settingsBtn);
		this.menuPanel.Controls.Add(this.filesBtn);
		this.menuPanel.Location = new System.Drawing.Point(0, 0);
		this.menuPanel.Name = "menuPanel";
		this.menuPanel.Size = new System.Drawing.Size(353, 62);
		this.menuPanel.TabIndex = 16;
		this.helpBtn.BackColor = System.Drawing.Color.Transparent;
		this.helpBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.helpBtn.FlatAppearance.BorderSize = 0;
		this.helpBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
		this.helpBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
		this.helpBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.helpBtn.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.helpBtn.Location = new System.Drawing.Point(231, 13);
		this.helpBtn.Name = "helpBtn";
		this.helpBtn.Size = new System.Drawing.Size(102, 37);
		this.helpBtn.TabIndex = 2;
		this.helpBtn.Text = "Help";
		this.helpBtn.UseVisualStyleBackColor = false;
		this.helpBtn.Click += new System.EventHandler(button3_Click);
		this.settingsBtn.BackColor = System.Drawing.Color.Transparent;
		this.settingsBtn.FlatAppearance.BorderSize = 0;
		this.settingsBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
		this.settingsBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
		this.settingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.settingsBtn.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.settingsBtn.ForeColor = System.Drawing.Color.Black;
		this.settingsBtn.Location = new System.Drawing.Point(123, 13);
		this.settingsBtn.Name = "settingsBtn";
		this.settingsBtn.Size = new System.Drawing.Size(102, 37);
		this.settingsBtn.TabIndex = 1;
		this.settingsBtn.Text = "Settings";
		this.settingsBtn.UseVisualStyleBackColor = false;
		this.settingsBtn.Click += new System.EventHandler(button2_Click);
		this.filesBtn.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.filesBtn.BackColor = System.Drawing.Color.Transparent;
		this.filesBtn.FlatAppearance.BorderColor = System.Drawing.Color.White;
		this.filesBtn.FlatAppearance.BorderSize = 0;
		this.filesBtn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
		this.filesBtn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
		this.filesBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.filesBtn.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.filesBtn.Location = new System.Drawing.Point(15, 13);
		this.filesBtn.Name = "filesBtn";
		this.filesBtn.Size = new System.Drawing.Size(102, 37);
		this.filesBtn.TabIndex = 0;
		this.filesBtn.Text = "Files";
		this.filesBtn.UseVisualStyleBackColor = false;
		this.filesBtn.Click += new System.EventHandler(button1_Click);
		this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.label3.BackColor = System.Drawing.Color.Transparent;
		this.label3.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.label3.Location = new System.Drawing.Point(623, 42);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(71, 23);
		this.label3.TabIndex = 14;
		this.label3.Text = "phpMyAdmin";
		this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		this.serverStatusTxt.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.serverStatusTxt.BackColor = System.Drawing.Color.Transparent;
		this.serverStatusTxt.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.serverStatusTxt.Location = new System.Drawing.Point(684, 42);
		this.serverStatusTxt.Name = "serverStatusTxt";
		this.serverStatusTxt.Size = new System.Drawing.Size(91, 23);
		this.serverStatusTxt.TabIndex = 12;
		this.serverStatusTxt.Text = "Start server";
		this.serverStatusTxt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		this.apacheStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.apacheStatus.AutoSize = true;
		this.apacheStatus.BackColor = System.Drawing.Color.Transparent;
		this.apacheStatus.CheckAlign = System.Drawing.ContentAlignment.TopRight;
		this.apacheStatus.Enabled = false;
		this.apacheStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.apacheStatus.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.apacheStatus.Location = new System.Drawing.Point(784, 8);
		this.apacheStatus.Name = "apacheStatus";
		this.apacheStatus.Size = new System.Drawing.Size(54, 16);
		this.apacheStatus.TabIndex = 4;
		this.apacheStatus.Text = "Apache";
		this.apacheStatus.UseVisualStyleBackColor = false;
		this.mysqlStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.mysqlStatus.AutoSize = true;
		this.mysqlStatus.BackColor = System.Drawing.Color.Transparent;
		this.mysqlStatus.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.mysqlStatus.Enabled = false;
		this.mysqlStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
		this.mysqlStatus.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.mysqlStatus.Location = new System.Drawing.Point(785, 23);
		this.mysqlStatus.Name = "mysqlStatus";
		this.mysqlStatus.Size = new System.Drawing.Size(53, 16);
		this.mysqlStatus.TabIndex = 5;
		this.mysqlStatus.Text = "MySQL";
		this.mysqlStatus.UseVisualStyleBackColor = false;
		this.logo.BackColor = System.Drawing.Color.Transparent;
		this.logo.Location = new System.Drawing.Point(15, 7);
		this.logo.Name = "logo";
		this.logo.Size = new System.Drawing.Size(198, 49);
		this.logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.logo.TabIndex = 10;
		this.logo.TabStop = false;
		this.powerPicture.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.powerPicture.BackColor = System.Drawing.Color.Transparent;
		this.powerPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
		this.powerPicture.Location = new System.Drawing.Point(697, 0);
		this.powerPicture.Name = "powerPicture";
		this.powerPicture.Size = new System.Drawing.Size(62, 50);
		this.powerPicture.TabIndex = 11;
		this.powerPicture.TabStop = false;
		this.powerPicture.Click += new System.EventHandler(pictureBox2_Click);
		this.phpmyadminPicture.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.phpmyadminPicture.BackColor = System.Drawing.Color.Transparent;
		this.phpmyadminPicture.Location = new System.Drawing.Point(624, 5);
		this.phpmyadminPicture.Name = "phpmyadminPicture";
		this.phpmyadminPicture.Size = new System.Drawing.Size(66, 37);
		this.phpmyadminPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.phpmyadminPicture.TabIndex = 15;
		this.phpmyadminPicture.TabStop = false;
		this.phpmyadminPicture.Click += new System.EventHandler(pictureBox3_Click);
		this.addVhostBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.addVhostBtn.Location = new System.Drawing.Point(14, 461);
		this.addVhostBtn.Name = "addVhostBtn";
		this.addVhostBtn.Size = new System.Drawing.Size(29, 23);
		this.addVhostBtn.TabIndex = 8;
		this.addVhostBtn.Text = "+";
		this.addVhostBtn.UseVisualStyleBackColor = true;
		this.addVhostBtn.Click += new System.EventHandler(addVhostBtn_Click);
		this.deleteVhostBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.deleteVhostBtn.Enabled = false;
		this.deleteVhostBtn.Location = new System.Drawing.Point(49, 461);
		this.deleteVhostBtn.Name = "deleteVhostBtn";
		this.deleteVhostBtn.Size = new System.Drawing.Size(29, 23);
		this.deleteVhostBtn.TabIndex = 9;
		this.deleteVhostBtn.Text = "-";
		this.deleteVhostBtn.UseVisualStyleBackColor = true;
		this.deleteVhostBtn.Click += new System.EventHandler(deleteVhostBtn_Click);
		this.appStatus.Tick += new System.EventHandler(app_Tick);
		this.filesContextMenu.DropShadowEnabled = false;
		this.filesContextMenu.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.filesContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.exitToolStripMenuItem1 });
		this.filesContextMenu.Name = "filesContextMenu";
		this.filesContextMenu.Size = new System.Drawing.Size(93, 26);
		this.exitToolStripMenuItem1.Name = "exitToolStripMenuItem1";
		this.exitToolStripMenuItem1.Size = new System.Drawing.Size(92, 22);
		this.exitToolStripMenuItem1.Text = "Exit";
		this.exitToolStripMenuItem1.Click += new System.EventHandler(exitToolStripMenuItem1_Click);
		this.settingsContextMenu.DropShadowEnabled = false;
		this.settingsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.settingsToolStripMenuItem1, this.logsToolStripMenuItem2, this.settingsToolStripMenuItem2 });
		this.settingsContextMenu.Name = "settingsContextMenu";
		this.settingsContextMenu.Size = new System.Drawing.Size(172, 70);
		this.settingsToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.pHPToolStripMenuItem3, this.apacheToolStripMenuItem4, this.mySQLToolStripMenuItem3 });
		this.settingsToolStripMenuItem1.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
		this.settingsToolStripMenuItem1.Size = new System.Drawing.Size(171, 22);
		this.settingsToolStripMenuItem1.Text = "Configuration files";
		this.pHPToolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.defaultXAMPPToolStripMenuItem1 });
		this.pHPToolStripMenuItem3.Name = "pHPToolStripMenuItem3";
		this.pHPToolStripMenuItem3.Size = new System.Drawing.Size(112, 22);
		this.pHPToolStripMenuItem3.Text = "PHP";
		this.defaultXAMPPToolStripMenuItem1.Name = "defaultXAMPPToolStripMenuItem1";
		this.defaultXAMPPToolStripMenuItem1.Size = new System.Drawing.Size(156, 22);
		this.defaultXAMPPToolStripMenuItem1.Text = "Default (XAMPP)";
		this.defaultXAMPPToolStripMenuItem1.Click += new System.EventHandler(defaultXAMPPToolStripMenuItem1_Click);
		this.apacheToolStripMenuItem4.Name = "apacheToolStripMenuItem4";
		this.apacheToolStripMenuItem4.Size = new System.Drawing.Size(112, 22);
		this.apacheToolStripMenuItem4.Text = "Apache";
		this.apacheToolStripMenuItem4.Click += new System.EventHandler(apacheToolStripMenuItem4_Click);
		this.mySQLToolStripMenuItem3.Name = "mySQLToolStripMenuItem3";
		this.mySQLToolStripMenuItem3.Size = new System.Drawing.Size(112, 22);
		this.mySQLToolStripMenuItem3.Text = "MySQL";
		this.mySQLToolStripMenuItem3.Click += new System.EventHandler(mySQLToolStripMenuItem3_Click);
		this.logsToolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.pHPToolStripMenuItem4, this.apacheToolStripMenuItem5, this.mySQLToolStripMenuItem4, this.fTPToolStripMenuItem });
		this.logsToolStripMenuItem2.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.logsToolStripMenuItem2.Name = "logsToolStripMenuItem2";
		this.logsToolStripMenuItem2.Size = new System.Drawing.Size(171, 22);
		this.logsToolStripMenuItem2.Text = "Logs";
		this.pHPToolStripMenuItem4.Name = "pHPToolStripMenuItem4";
		this.pHPToolStripMenuItem4.Size = new System.Drawing.Size(112, 22);
		this.pHPToolStripMenuItem4.Text = "PHP";
		this.pHPToolStripMenuItem4.Click += new System.EventHandler(pHPToolStripMenuItem4_Click_1);
		this.apacheToolStripMenuItem5.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.apacheLogToolStripMenuItem, this.accessLogToolStripMenuItem });
		this.apacheToolStripMenuItem5.Name = "apacheToolStripMenuItem5";
		this.apacheToolStripMenuItem5.Size = new System.Drawing.Size(112, 22);
		this.apacheToolStripMenuItem5.Text = "Apache";
		this.apacheLogToolStripMenuItem.Name = "apacheLogToolStripMenuItem";
		this.apacheLogToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
		this.apacheLogToolStripMenuItem.Text = "Apache";
		this.apacheLogToolStripMenuItem.Click += new System.EventHandler(apacheLogToolStripMenuItem_Click);
		this.accessLogToolStripMenuItem.Name = "accessLogToolStripMenuItem";
		this.accessLogToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
		this.accessLogToolStripMenuItem.Text = "Access log";
		this.accessLogToolStripMenuItem.Click += new System.EventHandler(accessLogToolStripMenuItem_Click);
		this.mySQLToolStripMenuItem4.Name = "mySQLToolStripMenuItem4";
		this.mySQLToolStripMenuItem4.Size = new System.Drawing.Size(112, 22);
		this.mySQLToolStripMenuItem4.Text = "MySQL";
		this.mySQLToolStripMenuItem4.Click += new System.EventHandler(mySQLToolStripMenuItem4_Click);
		this.fTPToolStripMenuItem.Name = "fTPToolStripMenuItem";
		this.fTPToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
		this.fTPToolStripMenuItem.Text = "FTP";
		this.fTPToolStripMenuItem.Click += new System.EventHandler(fTPToolStripMenuItem_Click);
		this.settingsToolStripMenuItem2.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.settingsToolStripMenuItem2.Name = "settingsToolStripMenuItem2";
		this.settingsToolStripMenuItem2.Size = new System.Drawing.Size(171, 22);
		this.settingsToolStripMenuItem2.Text = "Settings";
		this.settingsToolStripMenuItem2.Click += new System.EventHandler(settingsToolStripMenuItem2_Click);
		this.helpContextMenu.DropShadowEnabled = false;
		this.helpContextMenu.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.helpContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.donateToolStripMenuItem, this.checkForUpdateToolStripMenuItem1, this.reportBugToolStripMenuItem, this.aboutToolStripMenuItem1 });
		this.helpContextMenu.Name = "helpContextMenu";
		this.helpContextMenu.Size = new System.Drawing.Size(164, 92);
		this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
		this.donateToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
		this.donateToolStripMenuItem.Text = "Donate";
		this.donateToolStripMenuItem.Click += new System.EventHandler(donateToolStripMenuItem_Click);
		this.checkForUpdateToolStripMenuItem1.Name = "checkForUpdateToolStripMenuItem1";
		this.checkForUpdateToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
		this.checkForUpdateToolStripMenuItem1.Text = "Check for update";
		this.checkForUpdateToolStripMenuItem1.Click += new System.EventHandler(checkForUpdateToolStripMenuItem1_Click);
		this.reportBugToolStripMenuItem.Name = "reportBugToolStripMenuItem";
		this.reportBugToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
		this.reportBugToolStripMenuItem.Text = "Report bug";
		this.reportBugToolStripMenuItem.Click += new System.EventHandler(reportBugToolStripMenuItem_Click);
		this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
		this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
		this.aboutToolStripMenuItem1.Text = "About";
		this.aboutToolStripMenuItem1.Click += new System.EventHandler(aboutToolStripMenuItem1_Click);
		this.filterTextbox.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.filterTextbox.ForeColor = System.Drawing.Color.Gray;
		this.filterTextbox.Location = new System.Drawing.Point(230, 462);
		this.filterTextbox.Name = "filterTextbox";
		this.filterTextbox.Size = new System.Drawing.Size(150, 22);
		this.filterTextbox.TabIndex = 10;
		this.filterTextbox.Text = "Filter";
		this.editPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.editPanel.Controls.Add(this.generelTab);
		this.editPanel.Controls.Add(this.SSLTab);
		this.editPanel.Controls.Add(this.aliasesTab);
		this.editPanel.Controls.Add(this.advancedTab);
		this.editPanel.Location = new System.Drawing.Point(402, 81);
		this.editPanel.Name = "editPanel";
		this.editPanel.SelectedIndex = 0;
		this.editPanel.Size = new System.Drawing.Size(436, 374);
		this.editPanel.TabIndex = 5;
		this.generelTab.Controls.Add(this.ftpEnabled);
		this.generelTab.Controls.Add(this.panel4);
		this.generelTab.Controls.Add(this.customPortDescriptionLbl);
		this.generelTab.Controls.Add(this.port);
		this.generelTab.Controls.Add(this.portLbl);
		this.generelTab.Controls.Add(this.panel2);
		this.generelTab.Controls.Add(this.phpVersionLbl);
		this.generelTab.Controls.Add(this.phpVersion);
		this.generelTab.Controls.Add(this.documentRoot);
		this.generelTab.Controls.Add(this.documentRootLbl);
		this.generelTab.Controls.Add(this.serverName);
		this.generelTab.Controls.Add(this.serverNameLbl);
		this.generelTab.Controls.Add(this.browseBtn);
		this.generelTab.Location = new System.Drawing.Point(4, 22);
		this.generelTab.Name = "generelTab";
		this.generelTab.Padding = new System.Windows.Forms.Padding(3);
		this.generelTab.Size = new System.Drawing.Size(428, 348);
		this.generelTab.TabIndex = 0;
		this.generelTab.Text = "Generel";
		this.generelTab.UseVisualStyleBackColor = true;
		this.ftpEnabled.AutoSize = true;
		this.ftpEnabled.Location = new System.Drawing.Point(26, 247);
		this.ftpEnabled.Name = "ftpEnabled";
		this.ftpEnabled.Size = new System.Drawing.Size(81, 17);
		this.ftpEnabled.TabIndex = 12;
		this.ftpEnabled.Text = "Enable FTP";
		this.ftpEnabled.UseVisualStyleBackColor = true;
		this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel4.BackColor = System.Drawing.Color.Gainsboro;
		this.panel4.Location = new System.Drawing.Point(25, 227);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(377, 1);
		this.panel4.TabIndex = 11;
		this.customPortDescriptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.customPortDescriptionLbl.Font = new System.Drawing.Font("Segoe UI", 7.5f);
		this.customPortDescriptionLbl.ForeColor = System.Drawing.SystemColors.Desktop;
		this.customPortDescriptionLbl.Location = new System.Drawing.Point(177, 189);
		this.customPortDescriptionLbl.Name = "customPortDescriptionLbl";
		this.customPortDescriptionLbl.Size = new System.Drawing.Size(226, 28);
		this.customPortDescriptionLbl.TabIndex = 10;
		this.customPortDescriptionLbl.Text = "Only required for people who want their website to run on a custom port";
		this.port.Location = new System.Drawing.Point(119, 190);
		this.port.MaxLength = 5;
		this.port.Name = "port";
		this.port.Size = new System.Drawing.Size(42, 22);
		this.port.TabIndex = 8;
		this.portLbl.BackColor = System.Drawing.Color.Transparent;
		this.portLbl.Location = new System.Drawing.Point(22, 190);
		this.portLbl.Name = "portLbl";
		this.portLbl.Size = new System.Drawing.Size(89, 21);
		this.portLbl.TabIndex = 9;
		this.portLbl.Text = "Custom port";
		this.portLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.BackColor = System.Drawing.Color.Gainsboro;
		this.panel2.Location = new System.Drawing.Point(26, 174);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(377, 1);
		this.panel2.TabIndex = 7;
		this.phpVersionLbl.BackColor = System.Drawing.Color.Transparent;
		this.phpVersionLbl.Location = new System.Drawing.Point(21, 116);
		this.phpVersionLbl.Name = "phpVersionLbl";
		this.phpVersionLbl.Size = new System.Drawing.Size(89, 21);
		this.phpVersionLbl.TabIndex = 6;
		this.phpVersionLbl.Text = "PHP version";
		this.phpVersionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.phpVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.phpVersion.Items.AddRange(new object[4] { "Default", "PHP 5.4", "PHP 5.5", "PHP 5.6" });
		this.phpVersion.Location = new System.Drawing.Point(119, 116);
		this.phpVersion.Name = "phpVersion";
		this.phpVersion.Size = new System.Drawing.Size(121, 21);
		this.phpVersion.TabIndex = 5;
		this.phpVersion.SelectedIndexChanged += new System.EventHandler(phpVersion_SelectedIndexChanged);
		this.SSLTab.Controls.Add(this.certificateKeyPath);
		this.SSLTab.Controls.Add(this.chooseCertificateKeyBtn);
		this.SSLTab.Controls.Add(this.certificateKeyLbl);
		this.SSLTab.Controls.Add(this.panel1);
		this.SSLTab.Controls.Add(this.createSelfSignedCertificateBtn);
		this.SSLTab.Controls.Add(this.certificateFilePath);
		this.SSLTab.Controls.Add(this.chooseCertificateBtn);
		this.SSLTab.Controls.Add(this.certificateFileLbl);
		this.SSLTab.Controls.Add(this.enableSSL);
		this.SSLTab.Location = new System.Drawing.Point(4, 22);
		this.SSLTab.Name = "SSLTab";
		this.SSLTab.Padding = new System.Windows.Forms.Padding(3);
		this.SSLTab.Size = new System.Drawing.Size(428, 348);
		this.SSLTab.TabIndex = 1;
		this.SSLTab.Text = "SSL";
		this.SSLTab.UseVisualStyleBackColor = true;
		this.certificateKeyPath.AutoSize = true;
		this.certificateKeyPath.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		this.certificateKeyPath.Location = new System.Drawing.Point(21, 157);
		this.certificateKeyPath.Name = "certificateKeyPath";
		this.certificateKeyPath.Size = new System.Drawing.Size(129, 13);
		this.certificateKeyPath.TabIndex = 8;
		this.certificateKeyPath.Text = "d:\\xampp\\certificate.key";
		this.chooseCertificateKeyBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chooseCertificateKeyBtn.Location = new System.Drawing.Point(328, 131);
		this.chooseCertificateKeyBtn.Name = "chooseCertificateKeyBtn";
		this.chooseCertificateKeyBtn.Size = new System.Drawing.Size(75, 24);
		this.chooseCertificateKeyBtn.TabIndex = 2;
		this.chooseCertificateKeyBtn.Text = "Choose";
		this.chooseCertificateKeyBtn.UseVisualStyleBackColor = true;
		this.chooseCertificateKeyBtn.Click += new System.EventHandler(chooseCertificateKeyBtn_Click);
		this.certificateKeyLbl.AutoSize = true;
		this.certificateKeyLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.certificateKeyLbl.Location = new System.Drawing.Point(21, 133);
		this.certificateKeyLbl.Name = "certificateKeyLbl";
		this.certificateKeyLbl.Size = new System.Drawing.Size(80, 13);
		this.certificateKeyLbl.TabIndex = 6;
		this.certificateKeyLbl.Text = "Certificate key";
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BackColor = System.Drawing.Color.Gainsboro;
		this.panel1.Location = new System.Drawing.Point(24, 199);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(377, 1);
		this.panel1.TabIndex = 5;
		this.createSelfSignedCertificateBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.createSelfSignedCertificateBtn.Location = new System.Drawing.Point(189, 224);
		this.createSelfSignedCertificateBtn.Name = "createSelfSignedCertificateBtn";
		this.createSelfSignedCertificateBtn.Size = new System.Drawing.Size(212, 24);
		this.createSelfSignedCertificateBtn.TabIndex = 3;
		this.createSelfSignedCertificateBtn.Text = "Create self signed certificate";
		this.createSelfSignedCertificateBtn.UseVisualStyleBackColor = true;
		this.createSelfSignedCertificateBtn.Click += new System.EventHandler(createSelfSignedCertificateBtn_Click);
		this.certificateFilePath.AutoSize = true;
		this.certificateFilePath.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
		this.certificateFilePath.Location = new System.Drawing.Point(21, 98);
		this.certificateFilePath.Name = "certificateFilePath";
		this.certificateFilePath.Size = new System.Drawing.Size(134, 13);
		this.certificateFilePath.TabIndex = 3;
		this.certificateFilePath.Text = "d:\\xampp\\certificate.pem";
		this.chooseCertificateBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chooseCertificateBtn.Location = new System.Drawing.Point(328, 72);
		this.chooseCertificateBtn.Name = "chooseCertificateBtn";
		this.chooseCertificateBtn.Size = new System.Drawing.Size(75, 24);
		this.chooseCertificateBtn.TabIndex = 1;
		this.chooseCertificateBtn.Text = "Choose";
		this.chooseCertificateBtn.UseVisualStyleBackColor = true;
		this.chooseCertificateBtn.Click += new System.EventHandler(chooseCertificateBtn_Click);
		this.certificateFileLbl.AutoSize = true;
		this.certificateFileLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.certificateFileLbl.Location = new System.Drawing.Point(21, 74);
		this.certificateFileLbl.Name = "certificateFileLbl";
		this.certificateFileLbl.Size = new System.Drawing.Size(59, 13);
		this.certificateFileLbl.TabIndex = 1;
		this.certificateFileLbl.Text = "Certificate";
		this.enableSSL.AutoSize = true;
		this.enableSSL.Location = new System.Drawing.Point(23, 29);
		this.enableSSL.Name = "enableSSL";
		this.enableSSL.Size = new System.Drawing.Size(81, 17);
		this.enableSSL.TabIndex = 0;
		this.enableSSL.Text = "Enable SSL";
		this.enableSSL.UseVisualStyleBackColor = true;
		this.aliasesTab.Controls.Add(this.editAliasBtn);
		this.aliasesTab.Controls.Add(this.deleteAliasBtn);
		this.aliasesTab.Controls.Add(this.createAliasBtn);
		this.aliasesTab.Controls.Add(this.aliases);
		this.aliasesTab.Location = new System.Drawing.Point(4, 22);
		this.aliasesTab.Name = "aliasesTab";
		this.aliasesTab.Size = new System.Drawing.Size(428, 348);
		this.aliasesTab.TabIndex = 2;
		this.aliasesTab.Text = "Aliases";
		this.aliasesTab.UseVisualStyleBackColor = true;
		this.editAliasBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.editAliasBtn.Enabled = false;
		this.editAliasBtn.Location = new System.Drawing.Point(331, 310);
		this.editAliasBtn.Name = "editAliasBtn";
		this.editAliasBtn.Size = new System.Drawing.Size(75, 23);
		this.editAliasBtn.TabIndex = 4;
		this.editAliasBtn.Text = "Edit";
		this.editAliasBtn.UseVisualStyleBackColor = true;
		this.editAliasBtn.Click += new System.EventHandler(editAliasBtn_Click);
		this.deleteAliasBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.deleteAliasBtn.Enabled = false;
		this.deleteAliasBtn.Location = new System.Drawing.Point(57, 310);
		this.deleteAliasBtn.Name = "deleteAliasBtn";
		this.deleteAliasBtn.Size = new System.Drawing.Size(29, 23);
		this.deleteAliasBtn.TabIndex = 3;
		this.deleteAliasBtn.Text = "-";
		this.deleteAliasBtn.UseVisualStyleBackColor = true;
		this.deleteAliasBtn.Click += new System.EventHandler(deleteAliasBtn_Click);
		this.createAliasBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.createAliasBtn.Location = new System.Drawing.Point(22, 310);
		this.createAliasBtn.Name = "createAliasBtn";
		this.createAliasBtn.Size = new System.Drawing.Size(29, 23);
		this.createAliasBtn.TabIndex = 2;
		this.createAliasBtn.Text = "+";
		this.createAliasBtn.UseVisualStyleBackColor = true;
		this.createAliasBtn.Click += new System.EventHandler(createAliasBtn_Click);
		this.aliases.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.aliases.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnAliasName, this.columnAliasPath });
		this.aliases.FullRowSelect = true;
		this.aliases.GridLines = true;
		this.aliases.HideSelection = false;
		this.aliases.Location = new System.Drawing.Point(22, 24);
		this.aliases.MultiSelect = false;
		this.aliases.Name = "aliases";
		this.aliases.Size = new System.Drawing.Size(383, 276);
		this.aliases.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.aliases.TabIndex = 1;
		this.aliases.UseCompatibleStateImageBehavior = false;
		this.aliases.View = System.Windows.Forms.View.Details;
		this.columnAliasName.Text = "Name";
		this.columnAliasName.Width = 143;
		this.columnAliasPath.Text = "Directory";
		this.columnAliasPath.Width = 236;
		this.advancedTab.Controls.Add(this.apacheSettings);
		this.advancedTab.Controls.Add(this.customApacheSettingsLbl);
		this.advancedTab.Controls.Add(this.panel3);
		this.advancedTab.Controls.Add(this.optionsResetBtn);
		this.advancedTab.Controls.Add(this.optionsExecuteCGI);
		this.advancedTab.Controls.Add(this.optionsSymlinksIfOwnerMatch);
		this.advancedTab.Controls.Add(this.optionsFollowSymlinks);
		this.advancedTab.Controls.Add(this.optionsIncludes);
		this.advancedTab.Controls.Add(this.optionsIndexes);
		this.advancedTab.Controls.Add(this.optionsForDirectoryLbl);
		this.advancedTab.Controls.Add(this.directoryOptionsDescriptionLbl);
		this.advancedTab.Location = new System.Drawing.Point(4, 22);
		this.advancedTab.Name = "advancedTab";
		this.advancedTab.Size = new System.Drawing.Size(428, 348);
		this.advancedTab.TabIndex = 3;
		this.advancedTab.Text = "Advanced";
		this.advancedTab.UseVisualStyleBackColor = true;
		this.apacheSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.apacheSettings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader3 });
		this.apacheSettings.ContextMenuStrip = this.apacheSettingsContextMenu;
		this.apacheSettings.FullRowSelect = true;
		this.apacheSettings.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
		this.apacheSettings.Items.AddRange(new System.Windows.Forms.ListViewItem[1] { listViewItem });
		this.apacheSettings.LabelEdit = true;
		this.apacheSettings.Location = new System.Drawing.Point(26, 208);
		this.apacheSettings.Name = "apacheSettings";
		this.apacheSettings.Size = new System.Drawing.Size(377, 114);
		this.apacheSettings.TabIndex = 10;
		this.apacheSettings.UseCompatibleStateImageBehavior = false;
		this.apacheSettings.View = System.Windows.Forms.View.Details;
		this.columnHeader3.Text = "Value";
		this.columnHeader3.Width = 370;
		this.apacheSettingsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addToolStripMenuItem, this.editToolStripMenuItem1, this.deleteToolStripMenuItem });
		this.apacheSettingsContextMenu.Name = "apacheSettingsContextMenu";
		this.apacheSettingsContextMenu.Size = new System.Drawing.Size(108, 70);
		this.apacheSettingsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(apacheSettingsContextMenu_Opening);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(addToolStripMenuItem_Click);
		this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
		this.editToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
		this.editToolStripMenuItem1.Text = "Edit";
		this.editToolStripMenuItem1.Click += new System.EventHandler(editToolStripMenuItem1_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		this.customApacheSettingsLbl.AutoSize = true;
		this.customApacheSettingsLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.customApacheSettingsLbl.Location = new System.Drawing.Point(23, 181);
		this.customApacheSettingsLbl.Name = "customApacheSettingsLbl";
		this.customApacheSettingsLbl.Size = new System.Drawing.Size(133, 13);
		this.customApacheSettingsLbl.TabIndex = 9;
		this.customApacheSettingsLbl.Text = "Custom Apache settings";
		this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel3.BackColor = System.Drawing.Color.Gainsboro;
		this.panel3.Location = new System.Drawing.Point(26, 166);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(377, 1);
		this.panel3.TabIndex = 8;
		this.optionsResetBtn.Location = new System.Drawing.Point(24, 116);
		this.optionsResetBtn.Name = "optionsResetBtn";
		this.optionsResetBtn.Size = new System.Drawing.Size(145, 23);
		this.optionsResetBtn.TabIndex = 1;
		this.optionsResetBtn.Text = "Use defaults";
		this.optionsResetBtn.UseVisualStyleBackColor = true;
		this.optionsResetBtn.Click += new System.EventHandler(optionsResetBtn_Click);
		this.optionsExecuteCGI.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.optionsExecuteCGI.AutoSize = true;
		this.optionsExecuteCGI.Location = new System.Drawing.Point(257, 111);
		this.optionsExecuteCGI.Name = "optionsExecuteCGI";
		this.optionsExecuteCGI.Size = new System.Drawing.Size(83, 17);
		this.optionsExecuteCGI.TabIndex = 6;
		this.optionsExecuteCGI.Text = "ExecuteCGI";
		this.optionsExecuteCGI.UseVisualStyleBackColor = true;
		this.optionsSymlinksIfOwnerMatch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.optionsSymlinksIfOwnerMatch.AutoSize = true;
		this.optionsSymlinksIfOwnerMatch.Location = new System.Drawing.Point(257, 91);
		this.optionsSymlinksIfOwnerMatch.Name = "optionsSymlinksIfOwnerMatch";
		this.optionsSymlinksIfOwnerMatch.Size = new System.Drawing.Size(144, 17);
		this.optionsSymlinksIfOwnerMatch.TabIndex = 5;
		this.optionsSymlinksIfOwnerMatch.Text = "SymlinksIfOwnerMatch";
		this.optionsSymlinksIfOwnerMatch.UseVisualStyleBackColor = true;
		this.optionsFollowSymlinks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.optionsFollowSymlinks.AutoSize = true;
		this.optionsFollowSymlinks.Location = new System.Drawing.Point(257, 71);
		this.optionsFollowSymlinks.Name = "optionsFollowSymlinks";
		this.optionsFollowSymlinks.Size = new System.Drawing.Size(107, 17);
		this.optionsFollowSymlinks.TabIndex = 4;
		this.optionsFollowSymlinks.Text = "FollowSymLinks";
		this.optionsFollowSymlinks.UseVisualStyleBackColor = true;
		this.optionsIncludes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.optionsIncludes.AutoSize = true;
		this.optionsIncludes.Location = new System.Drawing.Point(257, 51);
		this.optionsIncludes.Name = "optionsIncludes";
		this.optionsIncludes.Size = new System.Drawing.Size(69, 17);
		this.optionsIncludes.TabIndex = 3;
		this.optionsIncludes.Text = "Includes";
		this.optionsIncludes.UseVisualStyleBackColor = true;
		this.optionsIndexes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.optionsIndexes.AutoSize = true;
		this.optionsIndexes.Location = new System.Drawing.Point(257, 31);
		this.optionsIndexes.Name = "optionsIndexes";
		this.optionsIndexes.Size = new System.Drawing.Size(65, 17);
		this.optionsIndexes.TabIndex = 2;
		this.optionsIndexes.Text = "Indexes";
		this.optionsIndexes.UseVisualStyleBackColor = true;
		this.optionsForDirectoryLbl.AutoSize = true;
		this.optionsForDirectoryLbl.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.optionsForDirectoryLbl.Location = new System.Drawing.Point(21, 24);
		this.optionsForDirectoryLbl.Name = "optionsForDirectoryLbl";
		this.optionsForDirectoryLbl.Size = new System.Drawing.Size(115, 13);
		this.optionsForDirectoryLbl.TabIndex = 0;
		this.optionsForDirectoryLbl.Text = "Options for directory";
		this.directoryOptionsDescriptionLbl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.directoryOptionsDescriptionLbl.ForeColor = System.Drawing.Color.DimGray;
		this.directoryOptionsDescriptionLbl.Location = new System.Drawing.Point(21, 47);
		this.directoryOptionsDescriptionLbl.Name = "directoryOptionsDescriptionLbl";
		this.directoryOptionsDescriptionLbl.Size = new System.Drawing.Size(220, 66);
		this.directoryOptionsDescriptionLbl.TabIndex = 7;
		this.directoryOptionsDescriptionLbl.Text = "If you want special setings for your website you can choose it here. If you don't know anything about Apache click the \"Options all\" button.";
		this.chooseCertificateDialog.Title = "Choose certificate";
		this.chooseCertificateKeyDialog.Title = "Choose certificate";
		this.showWebsiteBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.showWebsiteBtn.Enabled = false;
		this.showWebsiteBtn.Location = new System.Drawing.Point(401, 461);
		this.showWebsiteBtn.Name = "showWebsiteBtn";
		this.showWebsiteBtn.Size = new System.Drawing.Size(97, 23);
		this.showWebsiteBtn.TabIndex = 11;
		this.showWebsiteBtn.Text = "Show website";
		this.showWebsiteBtn.UseVisualStyleBackColor = true;
		this.showWebsiteBtn.Click += new System.EventHandler(showWebsiteBtn_Click);
		this.vhosts.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.vhosts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader1, this.columnHeader2 });
		this.vhosts.EditableSubitems = null;
		this.vhosts.FullRowSelect = true;
		this.vhosts.GridLines = true;
		this.vhosts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
		this.vhosts.HideSelection = false;
		this.vhosts.Location = new System.Drawing.Point(14, 81);
		this.vhosts.MultiSelect = false;
		this.vhosts.Name = "vhosts";
		this.vhosts.Size = new System.Drawing.Size(366, 373);
		this.vhosts.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.vhosts.TabIndex = 7;
		this.vhosts.UseCompatibleStateImageBehavior = false;
		this.vhosts.View = System.Windows.Forms.View.Details;
		this.vhosts.SelectedIndexChanged += new System.EventHandler(vhosts_SelectedIndexChanged);
		this.columnHeader1.Text = "Server name";
		this.columnHeader1.Width = 149;
		this.columnHeader2.Text = "Document root";
		this.columnHeader2.Width = 213;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
		base.ClientSize = new System.Drawing.Size(851, 493);
		base.Controls.Add(this.showWebsiteBtn);
		base.Controls.Add(this.vhosts);
		base.Controls.Add(this.editPanel);
		base.Controls.Add(this.filterTextbox);
		base.Controls.Add(this.deleteVhostBtn);
		base.Controls.Add(this.addVhostBtn);
		base.Controls.Add(this.headerPanel);
		base.Controls.Add(this.saveBtn);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.KeyPreview = true;
		this.MinimumSize = new System.Drawing.Size(867, 519);
		base.Name = "Editor";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Main";
		base.Load += new System.EventHandler(Editor_Load);
		this.headerPanel.ResumeLayout(false);
		this.headerPanel.PerformLayout();
		this.menuPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.logo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.powerPicture).EndInit();
		((System.ComponentModel.ISupportInitialize)this.phpmyadminPicture).EndInit();
		this.filesContextMenu.ResumeLayout(false);
		this.settingsContextMenu.ResumeLayout(false);
		this.helpContextMenu.ResumeLayout(false);
		this.editPanel.ResumeLayout(false);
		this.generelTab.ResumeLayout(false);
		this.generelTab.PerformLayout();
		this.SSLTab.ResumeLayout(false);
		this.SSLTab.PerformLayout();
		this.aliasesTab.ResumeLayout(false);
		this.advancedTab.ResumeLayout(false);
		this.advancedTab.PerformLayout();
		this.apacheSettingsContextMenu.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
