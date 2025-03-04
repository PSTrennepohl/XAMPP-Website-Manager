using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XWM.Application;
using XWM.Xampp;

namespace XWM;

public class About : Form
{
	private IContainer components;

	private PictureBox pictureBox1;

	private Label productName;

	private RichTextBox aboutText;

	private Label version;

	private Button showLicenceBtn;

	private Button okBtn;

	private Label label1;

	private Label xamppVersionLbl;

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // Garantir visibilidade
    public new string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    public About()
	{
		InitializeComponent();
		productName.Text = System.Windows.Forms.Application.ProductName;
		version.Text = System.Windows.Forms.Application.ProductVersion;
		xamppVersionLbl.Text = XWM.Xampp.Xampp.GetVersion();
		Text = XWM.Application.Config.Instance.GetTranslation("About");
		okBtn.Text = XWM.Application.Config.Instance.GetTranslation("OK");
		showLicenceBtn.Text = XWM.Application.Config.Instance.GetTranslation("Licence");
		aboutText.Text = (string)Editor.Resources.GetObject("AboutText");
		object @object = Editor.Resources.GetObject("logo");
		if (@object != null)
		{
			pictureBox1.Image = (Bitmap)@object;
		}
	}

	private void showLicenceBtn_Click(object sender, System.EventArgs e)
	{
		new Licence().ShowDialog(this);
	}

	private void okBtn_Click(object sender, System.EventArgs e)
	{
		Close();
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
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.productName = new System.Windows.Forms.Label();
		this.aboutText = new System.Windows.Forms.RichTextBox();
		this.version = new System.Windows.Forms.Label();
		this.showLicenceBtn = new System.Windows.Forms.Button();
		this.okBtn = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.xamppVersionLbl = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.pictureBox1.Location = new System.Drawing.Point(12, 12);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(355, 92);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 0;
		this.pictureBox1.TabStop = false;
		this.productName.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.productName.Location = new System.Drawing.Point(72, 112);
		this.productName.Name = "productName";
		this.productName.Size = new System.Drawing.Size(152, 20);
		this.productName.TabIndex = 1;
		this.productName.Text = "XAMPP Website Manager";
		this.productName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.aboutText.BackColor = System.Drawing.Color.White;
		this.aboutText.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.aboutText.Location = new System.Drawing.Point(12, 198);
		this.aboutText.Name = "aboutText";
		this.aboutText.ReadOnly = true;
		this.aboutText.Size = new System.Drawing.Size(355, 220);
		this.aboutText.TabIndex = 2;
		this.aboutText.Text = "[About text goes here...]";
		this.version.Location = new System.Drawing.Point(227, 112);
		this.version.Name = "version";
		this.version.Size = new System.Drawing.Size(50, 20);
		this.version.TabIndex = 3;
		this.version.Text = "1.0";
		this.version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.showLicenceBtn.Location = new System.Drawing.Point(10, 425);
		this.showLicenceBtn.Name = "showLicenceBtn";
		this.showLicenceBtn.Size = new System.Drawing.Size(75, 23);
		this.showLicenceBtn.TabIndex = 4;
		this.showLicenceBtn.Text = "Licence";
		this.showLicenceBtn.UseVisualStyleBackColor = true;
		this.showLicenceBtn.Click += new System.EventHandler(showLicenceBtn_Click);
		this.okBtn.Location = new System.Drawing.Point(294, 425);
		this.okBtn.Name = "okBtn";
		this.okBtn.Size = new System.Drawing.Size(75, 23);
		this.okBtn.TabIndex = 5;
		this.okBtn.Text = "OK";
		this.okBtn.UseVisualStyleBackColor = true;
		this.okBtn.Click += new System.EventHandler(okBtn_Click);
		this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(69, 131);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(155, 20);
		this.label1.TabIndex = 6;
		this.label1.Text = "XAMPP";
		this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.xamppVersionLbl.Location = new System.Drawing.Point(227, 131);
		this.xamppVersionLbl.Name = "xamppVersionLbl";
		this.xamppVersionLbl.Size = new System.Drawing.Size(47, 20);
		this.xamppVersionLbl.TabIndex = 7;
		this.xamppVersionLbl.Text = "1.0";
		this.xamppVersionLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(379, 455);
		base.Controls.Add(this.xamppVersionLbl);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.okBtn);
		base.Controls.Add(this.showLicenceBtn);
		base.Controls.Add(this.version);
		base.Controls.Add(this.aboutText);
		base.Controls.Add(this.productName);
		base.Controls.Add(this.pictureBox1);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "About";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "About";
		base.TopMost = true;
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
	}
}
