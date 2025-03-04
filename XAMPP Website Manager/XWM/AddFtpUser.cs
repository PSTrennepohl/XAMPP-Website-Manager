using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using XWM.Application;
using XWM.Ftp;

namespace XWM;

public partial class AddFtpUser : Form
{
    protected User User;

    private IContainer components;

    private Panel panel2;

    private Panel panel1;

    private Button cancelBtn;

    private Button createBtn;

    private TextBox usernameTxt;

    private TextBox passwordTxt;

    private Label usernameLbl;

    private Label passwordLbl;

    private System.Windows.Forms.Timer validator;

    public AddFtpUser()
    {
        InitializeComponent();
        createBtn.Text = Config.Instance.GetTranslation("Create");
        Text = Config.Instance.GetTranslation("CreateUser");
        SetLanguage();
        validator.Start();
    }

    public AddFtpUser(User user)
    {
        InitializeComponent();
        User = user;
        usernameTxt.Text = user.Username;
        createBtn.Text = Config.Instance.GetTranslation("Update");
        Text = Config.Instance.GetTranslation("EditUser");
        SetLanguage();
        validator.Start();
    }

    protected void SetLanguage()
    {
        usernameLbl.Text = Config.Instance.GetTranslation("Username");
        passwordLbl.Text = Config.Instance.GetTranslation("Password");
        cancelBtn.Text = Config.Instance.GetTranslation("Cancel");
    }

    private void cancelBtn_Click(object sender, System.EventArgs e)
    {
        Close();
    }

    private void createBtn_Click(object sender, System.EventArgs e)
    {
        if (Config.Instance.FtpUsers.FirstOrDefault((User u) => u.Username.ToLower() == usernameTxt.Text.ToLower()) != null && User == null)
        {
            MessageBox.Show(this, Config.Instance.GetTranslation("ErrorUserAlreadyExists"), Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return;
        }
        if (User == null)
        {
            UserStore.Users.Add(new User
            {
                Username = usernameTxt.Text,
                Password = passwordTxt.Text,
                HomeDir = Config.Instance.FtpStoragePath
            });
        }
        else
        {
            User.Username = usernameTxt.Text;
            if (!string.IsNullOrEmpty(passwordTxt.Text))
            {
                User.Password = passwordTxt.Text;
            }
        }
        UserStore.Save();
        Close();
    }

    private void validator_Tick(object sender, System.EventArgs e)
    {
        Regex regex = new Regex("[^A-Z0-9]", RegexOptions.IgnoreCase);
        createBtn.Enabled = !string.IsNullOrEmpty(usernameTxt.Text) && ((User == null && !string.IsNullOrEmpty(passwordTxt.Text)) || User != null) && !regex.Match(usernameTxt.Text).Success;
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
        this.panel2 = new System.Windows.Forms.Panel();
        this.panel1 = new System.Windows.Forms.Panel();
        this.cancelBtn = new System.Windows.Forms.Button();
        this.createBtn = new System.Windows.Forms.Button();
        this.usernameTxt = new System.Windows.Forms.TextBox();
        this.passwordTxt = new System.Windows.Forms.TextBox();
        this.usernameLbl = new System.Windows.Forms.Label();
        this.passwordLbl = new System.Windows.Forms.Label();
        this.validator = new System.Windows.Forms.Timer(this.components);
        this.panel1.SuspendLayout();
        base.SuspendLayout();
        this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
        this.panel2.Location = new System.Drawing.Point(-6, 150);
        this.panel2.Name = "panel2";
        this.panel2.Size = new System.Drawing.Size(372, 1);
        this.panel2.TabIndex = 16;
        this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
        this.panel1.Controls.Add(this.cancelBtn);
        this.panel1.Controls.Add(this.createBtn);
        this.panel1.Location = new System.Drawing.Point(-3, 152);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(379, 45);
        this.panel1.TabIndex = 15;
        this.cancelBtn.Location = new System.Drawing.Point(178, 8);
        this.cancelBtn.Name = "cancelBtn";
        this.cancelBtn.Size = new System.Drawing.Size(86, 23);
        this.cancelBtn.TabIndex = 5;
        this.cancelBtn.Text = "Cancel";
        this.cancelBtn.UseVisualStyleBackColor = true;
        this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
        this.createBtn.Location = new System.Drawing.Point(271, 8);
        this.createBtn.Name = "createBtn";
        this.createBtn.Size = new System.Drawing.Size(86, 23);
        this.createBtn.TabIndex = 4;
        this.createBtn.Text = "Create";
        this.createBtn.UseVisualStyleBackColor = true;
        this.createBtn.Click += new System.EventHandler(createBtn_Click);
        this.usernameTxt.Location = new System.Drawing.Point(147, 32);
        this.usernameTxt.Name = "usernameTxt";
        this.usernameTxt.Size = new System.Drawing.Size(169, 22);
        this.usernameTxt.TabIndex = 17;
        this.passwordTxt.Location = new System.Drawing.Point(147, 74);
        this.passwordTxt.Name = "passwordTxt";
        this.passwordTxt.Size = new System.Drawing.Size(169, 22);
        this.passwordTxt.TabIndex = 18;
        this.passwordTxt.UseSystemPasswordChar = true;
        this.usernameLbl.Location = new System.Drawing.Point(32, 32);
        this.usernameLbl.Name = "usernameLbl";
        this.usernameLbl.Size = new System.Drawing.Size(113, 22);
        this.usernameLbl.TabIndex = 19;
        this.usernameLbl.Text = "Username";
        this.usernameLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.passwordLbl.Location = new System.Drawing.Point(32, 73);
        this.passwordLbl.Name = "passwordLbl";
        this.passwordLbl.Size = new System.Drawing.Size(113, 22);
        this.passwordLbl.TabIndex = 20;
        this.passwordLbl.Text = "Password";
        this.passwordLbl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.validator.Tick += new System.EventHandler(validator_Tick);
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.White;
        base.ClientSize = new System.Drawing.Size(362, 193);
        base.Controls.Add(this.passwordLbl);
        base.Controls.Add(this.usernameLbl);
        base.Controls.Add(this.passwordTxt);
        base.Controls.Add(this.usernameTxt);
        base.Controls.Add(this.panel2);
        base.Controls.Add(this.panel1);
        this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
        base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        base.MaximizeBox = false;
        base.MinimizeBox = false;
        base.Name = "AddFtpUser";
        base.ShowIcon = false;
        base.ShowInTaskbar = false;
        base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Add FTP user";
        this.panel1.ResumeLayout(false);
        base.ResumeLayout(false);
        base.PerformLayout();
    }
}
