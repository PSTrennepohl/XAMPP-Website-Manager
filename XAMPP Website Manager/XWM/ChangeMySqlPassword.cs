using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using XWM.Application;
using XWM.Data;
using XWM.Xampp;

namespace XWM;

public class ChangeMySqlPassword : Form
{
	private IContainer components;

	private Label newPasswordLbl;

	private Label repeatPasswordLbl;

	private TextBox newPasswordTxt;

	private TextBox repeatPasswordTxt;

	private Panel panel2;

	private Panel panel1;

	private Button cancelBtn;

	private Button saveBtn;

	private TextBox currentPasswordTxt;

	private Label currentPasswordLbl;

	private Label currentPasswordHintLbl;

	private Label mysqlPasswordWarningLbl;

	private Panel panel3;

	private Panel loading;

	private PictureBox loadingPicture;

	public ChangeMySqlPassword()
	{
		InitializeComponent();
		Text = XWM.Application.Config.Instance.GetTranslation("ChangeMySQLPassword");
		mysqlPasswordWarningLbl.Text = XWM.Application.Config.Instance.GetTranslation("ChangeMySQLPasswordWarning");
		saveBtn.Text = XWM.Application.Config.Instance.GetTranslation("Save");
		cancelBtn.Text = XWM.Application.Config.Instance.GetTranslation("Cancel");
		currentPasswordLbl.Text = XWM.Application.Config.Instance.GetTranslation("CurrentPassword");
		currentPasswordHintLbl.Text = XWM.Application.Config.Instance.GetTranslation("CurrentPasswordHint");
		newPasswordLbl.Text = XWM.Application.Config.Instance.GetTranslation("NewPassword");
		repeatPasswordLbl.Text = XWM.Application.Config.Instance.GetTranslation("RepeatPassword");
		loadingPicture.Image = (Image)Resources.Get().GetObject("loading");
		loading.Visible = false;
		loading.BringToFront();
	}

	private void cancelBtn_Click(object sender, System.EventArgs e)
	{
		Close();
	}

	protected void RecieveResponse(int exitCode)
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action<int>(RecieveResponse), exitCode);
			return;
		}
		loading.Visible = false;
		if (exitCode > 0)
		{
			MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("ErrorFailedToChangeMySQLPassword"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		ChangePhpmyadminPassword(newPasswordTxt.Text);
		XWM.Xampp.Config.RestartRequired();
		MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("MySQLPasswordHasBeenChanged"), XWM.Application.Config.Instance.GetTranslation("Success"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		Close();
	}

	private void ChangeMysqlPassword(string oldPassword, string password)
	{
		loading.Visible = true;
		if (!XWM.Xampp.Config.MysqlRunning())
		{
			XWM.Xampp.Config.StartMySql();
		}
		Mysql.ChangePassword(oldPassword, password, delegate(Process process)
		{
			RecieveResponse(process.ExitCode);
		});
	}

	private void ChangePhpmyadminPassword(string password)
	{
		try
		{
			Phpmyadmin.ChangeMysqlPassword(password);
		}
		catch (Exception ex)
		{
			MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("ErrorFailedToChangeMySQLPasswordPhpMyAdmin", ex.Message), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void saveBtn_Click(object sender, System.EventArgs e)
	{
		if (newPasswordTxt.Text != repeatPasswordTxt.Text)
		{
			MessageBox.Show(this, XWM.Application.Config.Instance.GetTranslation("ThePasswordsDoesntMatch"), XWM.Application.Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			ChangeMysqlPassword(currentPasswordTxt.Text, newPasswordTxt.Text);
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
		this.newPasswordLbl = new System.Windows.Forms.Label();
		this.repeatPasswordLbl = new System.Windows.Forms.Label();
		this.newPasswordTxt = new System.Windows.Forms.TextBox();
		this.repeatPasswordTxt = new System.Windows.Forms.TextBox();
		this.panel2 = new System.Windows.Forms.Panel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.saveBtn = new System.Windows.Forms.Button();
		this.currentPasswordTxt = new System.Windows.Forms.TextBox();
		this.currentPasswordLbl = new System.Windows.Forms.Label();
		this.currentPasswordHintLbl = new System.Windows.Forms.Label();
		this.mysqlPasswordWarningLbl = new System.Windows.Forms.Label();
		this.panel3 = new System.Windows.Forms.Panel();
		this.loading = new System.Windows.Forms.Panel();
		this.loadingPicture = new System.Windows.Forms.PictureBox();
		this.panel1.SuspendLayout();
		this.panel3.SuspendLayout();
		this.loading.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).BeginInit();
		base.SuspendLayout();
		this.newPasswordLbl.AutoSize = true;
		this.newPasswordLbl.Location = new System.Drawing.Point(26, 172);
		this.newPasswordLbl.Name = "newPasswordLbl";
		this.newPasswordLbl.Size = new System.Drawing.Size(83, 13);
		this.newPasswordLbl.TabIndex = 0;
		this.newPasswordLbl.Text = "New password";
		this.repeatPasswordLbl.AutoSize = true;
		this.repeatPasswordLbl.Location = new System.Drawing.Point(26, 236);
		this.repeatPasswordLbl.Name = "repeatPasswordLbl";
		this.repeatPasswordLbl.Size = new System.Drawing.Size(96, 13);
		this.repeatPasswordLbl.TabIndex = 1;
		this.repeatPasswordLbl.Text = "Repeat password";
		this.newPasswordTxt.Location = new System.Drawing.Point(29, 197);
		this.newPasswordTxt.Name = "newPasswordTxt";
		this.newPasswordTxt.Size = new System.Drawing.Size(168, 22);
		this.newPasswordTxt.TabIndex = 2;
		this.newPasswordTxt.UseSystemPasswordChar = true;
		this.repeatPasswordTxt.Location = new System.Drawing.Point(29, 261);
		this.repeatPasswordTxt.Name = "repeatPasswordTxt";
		this.repeatPasswordTxt.Size = new System.Drawing.Size(168, 22);
		this.repeatPasswordTxt.TabIndex = 3;
		this.repeatPasswordTxt.UseSystemPasswordChar = true;
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.panel2.Location = new System.Drawing.Point(-5, 311);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(372, 1);
		this.panel2.TabIndex = 16;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel1.Controls.Add(this.cancelBtn);
		this.panel1.Controls.Add(this.saveBtn);
		this.panel1.Location = new System.Drawing.Point(-2, 313);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(379, 45);
		this.panel1.TabIndex = 15;
		this.cancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.cancelBtn.Location = new System.Drawing.Point(171, 8);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(86, 23);
		this.cancelBtn.TabIndex = 5;
		this.cancelBtn.Text = "Cancel";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.saveBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.saveBtn.Location = new System.Drawing.Point(264, 8);
		this.saveBtn.Name = "saveBtn";
		this.saveBtn.Size = new System.Drawing.Size(86, 23);
		this.saveBtn.TabIndex = 4;
		this.saveBtn.Text = "Save";
		this.saveBtn.UseVisualStyleBackColor = true;
		this.saveBtn.Click += new System.EventHandler(saveBtn_Click);
		this.currentPasswordTxt.Location = new System.Drawing.Point(29, 111);
		this.currentPasswordTxt.Name = "currentPasswordTxt";
		this.currentPasswordTxt.Size = new System.Drawing.Size(168, 22);
		this.currentPasswordTxt.TabIndex = 1;
		this.currentPasswordTxt.UseSystemPasswordChar = true;
		this.currentPasswordLbl.AutoSize = true;
		this.currentPasswordLbl.Location = new System.Drawing.Point(25, 87);
		this.currentPasswordLbl.Name = "currentPasswordLbl";
		this.currentPasswordLbl.Size = new System.Drawing.Size(99, 13);
		this.currentPasswordLbl.TabIndex = 17;
		this.currentPasswordLbl.Text = "Current password";
		this.currentPasswordHintLbl.Font = new System.Drawing.Font("Segoe UI", 7.75f);
		this.currentPasswordHintLbl.ForeColor = System.Drawing.Color.Gray;
		this.currentPasswordHintLbl.Location = new System.Drawing.Point(26, 141);
		this.currentPasswordHintLbl.Name = "currentPasswordHintLbl";
		this.currentPasswordHintLbl.Size = new System.Drawing.Size(320, 31);
		this.currentPasswordHintLbl.TabIndex = 18;
		this.currentPasswordHintLbl.Text = "The default password for MySQL on XAMPP is empty/blank.";
		this.mysqlPasswordWarningLbl.BackColor = System.Drawing.Color.Transparent;
		this.mysqlPasswordWarningLbl.Location = new System.Drawing.Point(14, 14);
		this.mysqlPasswordWarningLbl.Name = "mysqlPasswordWarningLbl";
		this.mysqlPasswordWarningLbl.Size = new System.Drawing.Size(334, 46);
		this.mysqlPasswordWarningLbl.TabIndex = 19;
		this.mysqlPasswordWarningLbl.Text = "Here you can change the password for MySQL. Please note that in doing so, you can risk not being able to connect as root if you forget the new password.";
		this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel3.BackColor = System.Drawing.SystemColors.ButtonFace;
		this.panel3.Controls.Add(this.mysqlPasswordWarningLbl);
		this.panel3.Location = new System.Drawing.Point(-3, -3);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(364, 70);
		this.panel3.TabIndex = 20;
		this.loading.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.loading.BackColor = System.Drawing.Color.White;
		this.loading.Controls.Add(this.loadingPicture);
		this.loading.Location = new System.Drawing.Point(-3, -2);
		this.loading.Name = "loading";
		this.loading.Size = new System.Drawing.Size(364, 359);
		this.loading.TabIndex = 21;
		this.loadingPicture.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.loadingPicture.Location = new System.Drawing.Point(153, 135);
		this.loadingPicture.Name = "loadingPicture";
		this.loadingPicture.Size = new System.Drawing.Size(65, 64);
		this.loadingPicture.TabIndex = 14;
		this.loadingPicture.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(358, 355);
		base.Controls.Add(this.currentPasswordHintLbl);
		base.Controls.Add(this.currentPasswordTxt);
		base.Controls.Add(this.currentPasswordLbl);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.repeatPasswordTxt);
		base.Controls.Add(this.newPasswordTxt);
		base.Controls.Add(this.repeatPasswordLbl);
		base.Controls.Add(this.newPasswordLbl);
		base.Controls.Add(this.panel3);
		base.Controls.Add(this.loading);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "ChangeMySqlPassword";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Change MySQL password";
		this.panel1.ResumeLayout(false);
		this.panel3.ResumeLayout(false);
		this.loading.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
