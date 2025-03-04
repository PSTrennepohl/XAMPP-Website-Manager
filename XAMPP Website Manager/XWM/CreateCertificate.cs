using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using XWM.Application;
using XWM.Data;
using XWM.EventArgs;
using XWM.Validation;

namespace XWM;

public class CreateCertificate : Form
{
	private static CreateCertificate _instance;

	private IContainer components;

	private TextBox organisation;

	private Label organisationLbl;

	private Button createBtn;

	private Label cityLbl;

	private TextBox city;

	private Label stateLbl;

	private TextBox state;

	private TextBox countryCode;

	private Label countryCodeLbl;

	private Label maxLettersCountryCodeLbl;

	private Label emailLbl;

	private TextBox email;

	private SaveFileDialog saveCertificate;

	private System.Windows.Forms.Timer validation;

	private PictureBox loadingPicture;

	private Panel loading;

	private Panel panel2;

	private Panel panel3;

	public event CertificateCreatedEventHandler CertificateCreated;

	public CreateCertificate()
	{
		InitializeComponent();
		countryCode.MaxLength = 2;
		validation.Start();
		saveCertificate.Filter = Config.Instance.GetTranslation("Certificates") + " | *.cert";
		saveCertificate.DefaultExt = "cert";
		Text = Config.Instance.GetTranslation("CreateCertificate");
		countryCodeLbl.Text = Config.Instance.GetTranslation("CountryCode");
		maxLettersCountryCodeLbl.Text = Config.Instance.GetTranslation("CountryCodeMaxLetters");
		organisationLbl.Text = Config.Instance.GetTranslation("Organisation");
		cityLbl.Text = Config.Instance.GetTranslation("City");
		stateLbl.Text = Config.Instance.GetTranslation("State");
		emailLbl.Text = Config.Instance.GetTranslation("Email");
		createBtn.Text = Config.Instance.GetTranslation("Create");
		saveCertificate.Title = Config.Instance.GetTranslation("SaveCertificate");
		loadingPicture.Image = (Image)Resources.Get().GetObject("loading");
		loading.Visible = false;
		loading.BringToFront();
	}

	private void createSSL_Click(object sender, System.EventArgs e)
	{
		saveCertificate.FileOk += delegate
		{
			string filename = saveCertificate.FileName.Replace(".cert", string.Empty);
			string[] parameters = new string[7]
			{
				Config.Instance.XamppDirectory,
				countryCode.Text,
				state.Text,
				city.Text,
				organisation.Text,
				email.Text,
				filename
			};
			try
			{
				loading.Visible = true;
				new Thread((ThreadStart)delegate
				{
					Process process = new Process
					{
						StartInfo = 
						{
							FileName = AppDomain.CurrentDomain.BaseDirectory + "\\createcert.bat",
							Arguments = string.Join(" ", parameters),
							WorkingDirectory = Directory.GetCurrentDirectory(),
							UseShellExecute = false,
							RedirectStandardOutput = true,
							CreateNoWindow = true
						}
					};
					process.Start();
					while (!process.StandardOutput.EndOfStream)
					{
						string text = process.StandardOutput.ReadLine();
						if (text != null && text.ToLower() == "success")
						{
							OnCertificateCreated(new CreateCertificateEventArgs
							{
								CertificateFilePath = filename + ".cert",
								CertificateKeyPath = filename + ".key"
							});
						}
					}
				}).Start();
			}
			catch (Exception)
			{
				loading.Visible = false;
				MessageBox.Show(Config.Instance.GetTranslation("ErrorCreatingCertificateText"), Config.Instance.GetTranslation("ErrorCreatingCertificate"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		};
		saveCertificate.ShowDialog(this);
	}

	protected virtual void OnCertificateCreated(CreateCertificateEventArgs e)
	{
		if (this.CertificateCreated != null)
		{
			this.CertificateCreated(this, e);
		}
	}

	private void check_Tick(object sender, System.EventArgs e)
	{
		createBtn.Enabled = countryCode.Text.Length == 2 && organisation.Text != string.Empty && state.Text != string.Empty && city.Text != string.Empty && Validator.IsValidEmail(email.Text);
	}

	public static CreateCertificate Instance()
	{
		return _instance ?? (_instance = new CreateCertificate());
	}

	public static void Reset()
	{
		_instance = null;
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
		this.organisation = new System.Windows.Forms.TextBox();
		this.organisationLbl = new System.Windows.Forms.Label();
		this.createBtn = new System.Windows.Forms.Button();
		this.cityLbl = new System.Windows.Forms.Label();
		this.city = new System.Windows.Forms.TextBox();
		this.stateLbl = new System.Windows.Forms.Label();
		this.state = new System.Windows.Forms.TextBox();
		this.countryCode = new System.Windows.Forms.TextBox();
		this.countryCodeLbl = new System.Windows.Forms.Label();
		this.maxLettersCountryCodeLbl = new System.Windows.Forms.Label();
		this.emailLbl = new System.Windows.Forms.Label();
		this.email = new System.Windows.Forms.TextBox();
		this.saveCertificate = new System.Windows.Forms.SaveFileDialog();
		this.validation = new System.Windows.Forms.Timer(this.components);
		this.loadingPicture = new System.Windows.Forms.PictureBox();
		this.loading = new System.Windows.Forms.Panel();
		this.panel2 = new System.Windows.Forms.Panel();
		this.panel3 = new System.Windows.Forms.Panel();
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).BeginInit();
		this.loading.SuspendLayout();
		this.panel3.SuspendLayout();
		base.SuspendLayout();
		this.organisation.Location = new System.Drawing.Point(120, 51);
		this.organisation.Name = "organisation";
		this.organisation.Size = new System.Drawing.Size(198, 22);
		this.organisation.TabIndex = 2;
		this.organisationLbl.Location = new System.Drawing.Point(15, 50);
		this.organisationLbl.Name = "organisationLbl";
		this.organisationLbl.Size = new System.Drawing.Size(85, 22);
		this.organisationLbl.TabIndex = 1;
		this.organisationLbl.Text = "Organisation";
		this.organisationLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.createBtn.Enabled = false;
		this.createBtn.Location = new System.Drawing.Point(292, 10);
		this.createBtn.Name = "createBtn";
		this.createBtn.Size = new System.Drawing.Size(75, 23);
		this.createBtn.TabIndex = 6;
		this.createBtn.Text = "Create";
		this.createBtn.UseVisualStyleBackColor = true;
		this.createBtn.Click += new System.EventHandler(createSSL_Click);
		this.cityLbl.Location = new System.Drawing.Point(15, 82);
		this.cityLbl.Name = "cityLbl";
		this.cityLbl.Size = new System.Drawing.Size(85, 22);
		this.cityLbl.TabIndex = 4;
		this.cityLbl.Text = "City";
		this.cityLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.city.Location = new System.Drawing.Point(120, 83);
		this.city.Name = "city";
		this.city.Size = new System.Drawing.Size(198, 22);
		this.city.TabIndex = 3;
		this.stateLbl.Location = new System.Drawing.Point(15, 113);
		this.stateLbl.Name = "stateLbl";
		this.stateLbl.Size = new System.Drawing.Size(85, 22);
		this.stateLbl.TabIndex = 6;
		this.stateLbl.Text = "State";
		this.stateLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.state.Location = new System.Drawing.Point(120, 114);
		this.state.Name = "state";
		this.state.Size = new System.Drawing.Size(198, 22);
		this.state.TabIndex = 4;
		this.countryCode.Location = new System.Drawing.Point(120, 20);
		this.countryCode.Name = "countryCode";
		this.countryCode.Size = new System.Drawing.Size(39, 22);
		this.countryCode.TabIndex = 1;
		this.countryCodeLbl.Location = new System.Drawing.Point(16, 18);
		this.countryCodeLbl.Name = "countryCodeLbl";
		this.countryCodeLbl.Size = new System.Drawing.Size(85, 22);
		this.countryCodeLbl.TabIndex = 9;
		this.countryCodeLbl.Text = "Country code";
		this.countryCodeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.maxLettersCountryCodeLbl.AutoSize = true;
		this.maxLettersCountryCodeLbl.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.maxLettersCountryCodeLbl.Location = new System.Drawing.Point(173, 24);
		this.maxLettersCountryCodeLbl.Name = "maxLettersCountryCodeLbl";
		this.maxLettersCountryCodeLbl.Size = new System.Drawing.Size(60, 12);
		this.maxLettersCountryCodeLbl.TabIndex = 10;
		this.maxLettersCountryCodeLbl.Text = "Max 2 letters";
		this.emailLbl.Location = new System.Drawing.Point(15, 145);
		this.emailLbl.Name = "emailLbl";
		this.emailLbl.Size = new System.Drawing.Size(85, 22);
		this.emailLbl.TabIndex = 12;
		this.emailLbl.Text = "Email";
		this.emailLbl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.email.Location = new System.Drawing.Point(120, 146);
		this.email.Name = "email";
		this.email.Size = new System.Drawing.Size(198, 22);
		this.email.TabIndex = 5;
		this.saveCertificate.Title = "Save certificate";
		this.validation.Tick += new System.EventHandler(check_Tick);
		this.loadingPicture.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.loadingPicture.Location = new System.Drawing.Point(142, 80);
		this.loadingPicture.Name = "loadingPicture";
		this.loadingPicture.Size = new System.Drawing.Size(65, 64);
		this.loadingPicture.TabIndex = 14;
		this.loadingPicture.TabStop = false;
		this.loading.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.loading.Controls.Add(this.loadingPicture);
		this.loading.Location = new System.Drawing.Point(0, -2);
		this.loading.Name = "loading";
		this.loading.Size = new System.Drawing.Size(343, 248);
		this.loading.TabIndex = 15;
		this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.panel2.Location = new System.Drawing.Point(-41, 199);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(412, 1);
		this.panel2.TabIndex = 17;
		this.panel3.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel3.Controls.Add(this.createBtn);
		this.panel3.Location = new System.Drawing.Point(-38, 201);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(419, 45);
		this.panel3.TabIndex = 16;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(340, 246);
		base.Controls.Add(this.panel3);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.emailLbl);
		base.Controls.Add(this.email);
		base.Controls.Add(this.maxLettersCountryCodeLbl);
		base.Controls.Add(this.countryCodeLbl);
		base.Controls.Add(this.countryCode);
		base.Controls.Add(this.stateLbl);
		base.Controls.Add(this.state);
		base.Controls.Add(this.cityLbl);
		base.Controls.Add(this.city);
		base.Controls.Add(this.organisationLbl);
		base.Controls.Add(this.organisation);
		base.Controls.Add(this.loading);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "CreateCertificate";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Create certificate";
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).EndInit();
		this.loading.ResumeLayout(false);
		this.panel3.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
