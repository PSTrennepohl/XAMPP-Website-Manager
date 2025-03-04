using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;
using XWM.Application;
using XWM.Data;

namespace XWM;

public class Update : Form
{
	protected Uri DownloadUrl;

	private WebClient webClient;

	private readonly string _updateArchive;

	private IContainer components;

	private Button startDownload;

	private WebBrowser webBrowser1;

	private ProgressBar progressBar1;

	private Panel loadingPanel;

	private PictureBox loadingPicture;

	private Label downloadStatus;

	private Panel panel2;

	private Panel panel1;

	public Update()
	{
		InitializeComponent();
		_updateArchive = Path.GetTempPath() + "\\XamppUpdate.zip";
		startDownload.Visible = false;
		Text = Config.Instance.GetTranslation("UpdateAvailable");
		startDownload.Text = Config.Instance.GetTranslation("InstallUpdate");
		loadingPicture.Image = (Image)Resources.Get().GetObject("loading");
		loadingPanel.Visible = true;
		loadingPanel.BringToFront();
		downloadStatus.Visible = false;
		base.Closing += delegate
		{
			if (this.webClient != null && this.webClient.IsBusy)
			{
				this.webClient.CancelAsync();
				this.webClient.Dispose();
			}
		};
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
								DownloadUrl = new Uri(xmlNode2.InnerText);
								XmlNode xmlNode3 = xmlDocument.DocumentElement.SelectSingleNode("/info/changelog");
								if (xmlNode3 != null)
								{
									object @object = Resources.Get().GetObject("changelog");
									if (@object != null)
									{
										webBrowser1.DocumentText = @object.ToString().Replace("[CHANGELOG]", xmlNode3.InnerText.Replace("<p>\ufeff</p>", string.Empty));
									}
								}
								loadingPanel.Visible = false;
								loadingPanel.SendToBack();
								startDownload.Visible = true;
							}
						}
					}
				}
			};
			webClient.DownloadStringAsync(address);
		}
		catch (Exception)
		{
			Close();
		}*/
	}

	protected void SetDownloadStatus(double downloadedMb, double maxMb)
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action<double, double>(SetDownloadStatus), new double[2] { downloadedMb, maxMb });
		}
		else
		{
			downloadStatus.Text = $"{downloadedMb}MB / {maxMb}MB";
		}
	}

	private void button1_Click(object sender, System.EventArgs e)
	{
		progressBar1.Visible = true;
		startDownload.Visible = false;
		downloadStatus.Visible = true;
		downloadStatus.Text = Config.Instance.GetTranslation("Connecting");
		using (this.webClient = new WebClient())
		{
			progressBar1.Maximum = 100;
			this.webClient.DownloadFileAsync(DownloadUrl, _updateArchive);
			this.webClient.DownloadProgressChanged += delegate(object o, DownloadProgressChangedEventArgs args)
			{
				progressBar1.Value = args.ProgressPercentage;
				SetDownloadStatus(Math.Round((double)args.BytesReceived / 1024.0 / 1024.0), Math.Round((double)args.TotalBytesToReceive / 1024.0 / 1024.0));
			};
			this.webClient.DownloadFileCompleted += delegate(object o, AsyncCompletedEventArgs args)
			{
				if (!args.Cancelled)
				{
					progressBar1.Value = 100;
					try
					{
						using ZipFile source = ZipFile.Read(_updateArchive);
						string[] updateFiles = new string[2] { "updateextractor.exe", "ionic.zip.dll" };
						List<ZipEntry> list = source.Where((ZipEntry f) => updateFiles.Contains(f.FileName.ToLower())).ToList();
						if (list.Count > 0)
						{
							string text = Path.GetTempPath() + "\\" + System.Windows.Forms.Application.ProductName + "_" + System.Windows.Forms.Application.ProductVersion;
							foreach (ZipEntry item in list)
							{
								item.Extract(text, ExtractExistingFileAction.OverwriteSilently);
							}
							Process process = new Process();
							process.StartInfo.FileName = text + "\\UpdateExtractor";
							process.StartInfo.Arguments = "-extract " + _updateArchive + " -start \"" + System.Windows.Forms.Application.ExecutablePath + "\" -destination \"" + AppDomain.CurrentDomain.BaseDirectory + "\"";
							process.Start();
							System.Windows.Forms.Application.Exit();
						}
					}
					catch (Exception)
					{
						MessageBox.Show(this, Config.Instance.GetTranslation("FailedToDownloadUpdate"), Config.Instance.GetTranslation("Error"));
					}
					progressBar1.Value = 0;
					progressBar1.Visible = false;
					downloadStatus.Visible = false;
					startDownload.Visible = true;
				}
				else if (System.IO.File.Exists(_updateArchive))
				{
					System.IO.File.Delete(_updateArchive);
				}
			};
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
		this.startDownload = new System.Windows.Forms.Button();
		this.webBrowser1 = new System.Windows.Forms.WebBrowser();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.loadingPanel = new System.Windows.Forms.Panel();
		this.loadingPicture = new System.Windows.Forms.PictureBox();
		this.downloadStatus = new System.Windows.Forms.Label();
		this.panel2 = new System.Windows.Forms.Panel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.loadingPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.startDownload.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.startDownload.Location = new System.Drawing.Point(488, 6);
		this.startDownload.Name = "startDownload";
		this.startDownload.Size = new System.Drawing.Size(135, 28);
		this.startDownload.TabIndex = 0;
		this.startDownload.Text = "Installer opdatering";
		this.startDownload.UseVisualStyleBackColor = true;
		this.startDownload.Click += new System.EventHandler(button1_Click);
		this.webBrowser1.IsWebBrowserContextMenuEnabled = false;
		this.webBrowser1.Location = new System.Drawing.Point(0, 0);
		this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser1.Name = "webBrowser1";
		this.webBrowser1.Size = new System.Drawing.Size(608, 231);
		this.webBrowser1.TabIndex = 1;
		this.progressBar1.Location = new System.Drawing.Point(33, 7);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(489, 26);
		this.progressBar1.Step = 100;
		this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
		this.progressBar1.TabIndex = 2;
		this.progressBar1.Visible = false;
		this.loadingPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.loadingPanel.BackColor = System.Drawing.Color.White;
		this.loadingPanel.Controls.Add(this.loadingPicture);
		this.loadingPanel.Location = new System.Drawing.Point(0, 0);
		this.loadingPanel.Name = "loadingPanel";
		this.loadingPanel.Size = new System.Drawing.Size(611, 277);
		this.loadingPanel.TabIndex = 6;
		this.loadingPanel.Visible = false;
		this.loadingPicture.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.loadingPicture.Location = new System.Drawing.Point(273, 91);
		this.loadingPicture.Name = "loadingPicture";
		this.loadingPicture.Size = new System.Drawing.Size(65, 64);
		this.loadingPicture.TabIndex = 15;
		this.loadingPicture.TabStop = false;
		this.downloadStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.downloadStatus.BackColor = System.Drawing.Color.Transparent;
		this.downloadStatus.ForeColor = System.Drawing.Color.Gray;
		this.downloadStatus.Location = new System.Drawing.Point(524, 13);
		this.downloadStatus.Name = "downloadStatus";
		this.downloadStatus.Size = new System.Drawing.Size(96, 20);
		this.downloadStatus.TabIndex = 16;
		this.downloadStatus.Text = "200MB / 1000MB";
		this.downloadStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.panel2.Location = new System.Drawing.Point(-29, 230);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(653, 1);
		this.panel2.TabIndex = 17;
		this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel1.Controls.Add(this.progressBar1);
		this.panel1.Controls.Add(this.downloadStatus);
		this.panel1.Controls.Add(this.startDownload);
		this.panel1.Location = new System.Drawing.Point(-26, 232);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(660, 45);
		this.panel1.TabIndex = 16;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(607, 275);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.webBrowser1);
		base.Controls.Add(this.loadingPanel);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		base.Name = "Update";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Update available";
		this.loadingPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).EndInit();
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
