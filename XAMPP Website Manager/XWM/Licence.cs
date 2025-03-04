using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using XWM.Application;
using XWM.Data;

namespace XWM;

public class Licence : Form
{
	private IContainer components;

	private TextBox licenceTxt;

	private Button closeBtn;

	public Licence()
	{
		InitializeComponent();
		licenceTxt.Text = Resources.Get().GetString("licence");
		Text = Config.Instance.GetTranslation("Licence");
		closeBtn.Text = Config.Instance.GetTranslation("Close");
	}

	private void closeBtn_Click(object sender, System.EventArgs e)
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
		this.licenceTxt = new System.Windows.Forms.TextBox();
		this.closeBtn = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.licenceTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.licenceTxt.Location = new System.Drawing.Point(12, 12);
		this.licenceTxt.Multiline = true;
		this.licenceTxt.Name = "licenceTxt";
		this.licenceTxt.ReadOnly = true;
		this.licenceTxt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
		this.licenceTxt.Size = new System.Drawing.Size(486, 278);
		this.licenceTxt.TabIndex = 1;
		this.licenceTxt.Text = "[ LICENCE-TEXT ]";
		this.closeBtn.Location = new System.Drawing.Point(425, 301);
		this.closeBtn.Name = "closeBtn";
		this.closeBtn.Size = new System.Drawing.Size(75, 23);
		this.closeBtn.TabIndex = 0;
		this.closeBtn.Text = "Close";
		this.closeBtn.UseVisualStyleBackColor = true;
		this.closeBtn.Click += new System.EventHandler(closeBtn_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(510, 334);
		base.Controls.Add(this.closeBtn);
		base.Controls.Add(this.licenceTxt);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Licence";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Licence";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
