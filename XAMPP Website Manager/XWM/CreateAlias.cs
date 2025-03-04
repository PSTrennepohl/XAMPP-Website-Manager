using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using XWM.Application;
using XWM.EventArgs;
using XWM.File;

namespace XWM;

public class CreateAlias : Form
{
	private IContainer components;

	private Label nameLbl;

	private TextBox aliasName;

	private TextBox directoryPath;

	private Label directoryLbl;

	private Button browseBtn;

	private Panel panel1;

	private Button createBtn;

	private Panel panel2;

	private Button cancelBtn;

	private FolderBrowserDialog directoryBrowser;

	private System.Windows.Forms.Timer validator;

	private Label nameDescriptionLbl;

	public event AliasCreatedEventHandler AliasCreated;

	public CreateAlias(HostAlias alias = null)
	{
		InitializeComponent();
		createBtn.Enabled = false;
		validator.Start();
		Text = Config.Instance.GetTranslation("CreateAlias");
		createBtn.Text = Config.Instance.GetTranslation("Create");
		nameLbl.Text = Config.Instance.GetTranslation("Name");
		directoryLbl.Text = Config.Instance.GetTranslation("Directory");
		nameDescriptionLbl.Text = Config.Instance.GetTranslation("AliasNameDescription");
		browseBtn.Text = Config.Instance.GetTranslation("Browse");
		cancelBtn.Text = Config.Instance.GetTranslation("Cancel");
		if (alias != null)
		{
			aliasName.Text = alias.AliasName;
			directoryPath.Text = alias.Directory;
			createBtn.Text = Config.Instance.GetTranslation("Update");
			Text = Config.Instance.GetTranslation("UpdateAlias");
		}
	}

	private void cancelBtn_Click(object sender, System.EventArgs e)
	{
		Close();
	}

	private void browseBtn_Click(object sender, System.EventArgs e)
	{
		if (directoryBrowser.ShowDialog(this) == DialogResult.OK && Directory.Exists(directoryBrowser.SelectedPath))
		{
			directoryPath.Text = directoryBrowser.SelectedPath;
		}
	}

	private void validator_Tick(object sender, System.EventArgs e)
	{
		createBtn.Enabled = aliasName.TextLength > 1 && aliasName.Text[0] == '/' && directoryPath.Text != string.Empty && Directory.Exists(directoryPath.Text);
	}

	protected virtual void OnCertificateCreated(CreateAliasEventArgs e)
	{
		if (this.AliasCreated != null)
		{
			this.AliasCreated(this, e);
		}
	}

	private void createBtn_Click(object sender, System.EventArgs e)
	{
		if (aliasName.TextLength > 1 && aliasName.Text[0] == '/' && Directory.Exists(directoryPath.Text))
		{
			OnCertificateCreated(new CreateAliasEventArgs
			{
				AliasName = aliasName.Text,
				DirectoryPath = directoryPath.Text
			});
			Close();
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
		this.nameLbl = new System.Windows.Forms.Label();
		this.aliasName = new System.Windows.Forms.TextBox();
		this.directoryPath = new System.Windows.Forms.TextBox();
		this.directoryLbl = new System.Windows.Forms.Label();
		this.browseBtn = new System.Windows.Forms.Button();
		this.panel1 = new System.Windows.Forms.Panel();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.createBtn = new System.Windows.Forms.Button();
		this.panel2 = new System.Windows.Forms.Panel();
		this.directoryBrowser = new System.Windows.Forms.FolderBrowserDialog();
		this.validator = new System.Windows.Forms.Timer(this.components);
		this.nameDescriptionLbl = new System.Windows.Forms.Label();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.nameLbl.AutoSize = true;
		this.nameLbl.Location = new System.Drawing.Point(23, 30);
		this.nameLbl.Name = "nameLbl";
		this.nameLbl.Size = new System.Drawing.Size(36, 13);
		this.nameLbl.TabIndex = 0;
		this.nameLbl.Text = "Name";
		this.aliasName.Location = new System.Drawing.Point(95, 27);
		this.aliasName.Name = "aliasName";
		this.aliasName.Size = new System.Drawing.Size(191, 22);
		this.aliasName.TabIndex = 1;
		this.directoryPath.Location = new System.Drawing.Point(95, 97);
		this.directoryPath.Name = "directoryPath";
		this.directoryPath.Size = new System.Drawing.Size(191, 22);
		this.directoryPath.TabIndex = 2;
		this.directoryLbl.AutoSize = true;
		this.directoryLbl.Location = new System.Drawing.Point(23, 100);
		this.directoryLbl.Name = "directoryLbl";
		this.directoryLbl.Size = new System.Drawing.Size(53, 13);
		this.directoryLbl.TabIndex = 2;
		this.directoryLbl.Text = "Directory";
		this.browseBtn.Location = new System.Drawing.Point(294, 96);
		this.browseBtn.Name = "browseBtn";
		this.browseBtn.Size = new System.Drawing.Size(75, 24);
		this.browseBtn.TabIndex = 3;
		this.browseBtn.Text = "Browse";
		this.browseBtn.UseVisualStyleBackColor = true;
		this.browseBtn.Click += new System.EventHandler(browseBtn_Click);
		this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel1.Controls.Add(this.cancelBtn);
		this.panel1.Controls.Add(this.createBtn);
		this.panel1.Location = new System.Drawing.Point(-3, 164);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(419, 45);
		this.panel1.TabIndex = 5;
		this.cancelBtn.Location = new System.Drawing.Point(221, 8);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(86, 23);
		this.cancelBtn.TabIndex = 5;
		this.cancelBtn.Text = "Cancel";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.createBtn.Location = new System.Drawing.Point(314, 8);
		this.createBtn.Name = "createBtn";
		this.createBtn.Size = new System.Drawing.Size(86, 23);
		this.createBtn.TabIndex = 4;
		this.createBtn.Text = "Create";
		this.createBtn.UseVisualStyleBackColor = true;
		this.createBtn.Click += new System.EventHandler(createBtn_Click);
		this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.panel2.Location = new System.Drawing.Point(-6, 162);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(412, 1);
		this.panel2.TabIndex = 14;
		this.validator.Tick += new System.EventHandler(validator_Tick);
		this.nameDescriptionLbl.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.nameDescriptionLbl.ForeColor = System.Drawing.Color.DimGray;
		this.nameDescriptionLbl.Location = new System.Drawing.Point(93, 57);
		this.nameDescriptionLbl.Name = "nameDescriptionLbl";
		this.nameDescriptionLbl.Size = new System.Drawing.Size(280, 32);
		this.nameDescriptionLbl.TabIndex = 15;
		this.nameDescriptionLbl.Text = "Path that will be visible in the browser with the content specified from the directory below";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(405, 205);
		base.Controls.Add(this.nameDescriptionLbl);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.directoryPath);
		base.Controls.Add(this.directoryLbl);
		base.Controls.Add(this.aliasName);
		base.Controls.Add(this.nameLbl);
		base.Controls.Add(this.browseBtn);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "CreateAlias";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Create alias";
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
