using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using XWM.Application;
using XWM.CustomControls;
using XWM.Data;
using XWM.EventArgs;
using XWM.File;

namespace XWM;

public class ConfigurationEditor : Form
{
	protected string ConfigurationFile;

	protected IniEntryCollection Configuration;

	private IContainer components;

	private TextBox filter;

	private Button saveBtn;

	private Button cancelBtn;

	private Panel panel2;

	private Panel panel1;

	private TabControl tabControl1;

	private TabPage allTab;

	private ListViewCustomControl configList;

	private ColumnHeader keyColumn;

	private ColumnHeader valueColumn;

	private Button openBtn;

	private ColumnHeader commentColumn;

	public ConfigurationEditor(string configurationFile)
	{
		InitializeComponent();
		Text = Config.Instance.GetTranslation("ConfigurationEditor");
		saveBtn.Text = Config.Instance.GetTranslation("Save");
		cancelBtn.Text = Config.Instance.GetTranslation("Cancel");
		openBtn.Text = Config.Instance.GetTranslation("Open");
		Text = Text + " - " + configurationFile;
		base.Icon = (Icon)Resources.Get().GetObject("config");
		ConfigurationFile = configurationFile;
		if (Config.Instance.ConfigurationEditorWidth > 0 && Config.Instance.ConfigurationEditorHeight > 0)
		{
			base.Width = Config.Instance.ConfigurationEditorWidth;
			base.Height = Config.Instance.ConfigurationEditorHeight;
		}
		base.Closing += delegate
		{
			Config.Instance.ConfigurationEditorWidth = base.Width;
			Config.Instance.ConfigurationEditorHeight = base.Height;
			Config.Instance.Save();
		};
	}

	private void PHPConfigurator_Load(object sender, System.EventArgs e)
	{
		if (!System.IO.File.Exists(ConfigurationFile))
		{
			MessageBox.Show(this, Config.Instance.GetTranslation("ErrorNotFound"), Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Close();
		}
		Configuration = IniParser.Parse(ConfigurationFile);
		if (Configuration.Count > 0)
		{
			tabControl1.SelectedIndexChanged += delegate
			{
				configList.Items.Clear();
				if (tabControl1.SelectedIndex > 0)
				{
					tabControl1.TabPages[0].Visible = true;
					{
						foreach (IniEntry item in Configuration.Where((IniEntry c) => c.Group == tabControl1.SelectedTab.Text))
						{
							ListViewItem listViewItem = new ListViewItem
							{
								Text = item.Key
							};
							listViewItem.SubItems.Add(item.Value);
							listViewItem.SubItems.Add(item.Comment);
							configList.Items.Add(listViewItem);
						}
						return;
					}
				}
				foreach (IniEntry item2 in Configuration)
				{
					ListViewItem listViewItem2 = new ListViewItem
					{
						Text = item2.Key
					};
					listViewItem2.SubItems.Add(item2.Value);
					listViewItem2.SubItems.Add(item2.Comment);
					configList.Items.Add(listViewItem2);
				}
			};
			foreach (IGrouping<string, IniEntry> item3 in from c in Configuration
				group c by c.Group into c
				orderby c.Key
				select c)
			{
				TabPage value = new TabPage(item3.Key);
				tabControl1.TabPages.Add(value);
			}
			foreach (IniEntry item4 in Configuration)
			{
				ListViewItem listViewItem3 = new ListViewItem
				{
					Text = item4.Key
				};
				listViewItem3.SubItems.Add(item4.Value);
				listViewItem3.SubItems.Add(item4.Comment);
				configList.Items.Add(listViewItem3);
			}
		}
		filter.GotFocus += delegate(object o, System.EventArgs args)
		{
			TextBox textBox = (TextBox)o;
			if (textBox.Text == Config.Instance.GetTranslation("Filter") || textBox.Text.Trim() == string.Empty)
			{
				textBox.ForeColor = Color.Black;
				textBox.Text = "";
			}
		};
		filter.LostFocus += delegate(object o, System.EventArgs args)
		{
			TextBox textBox2 = (TextBox)o;
			if (filter.Text == Config.Instance.GetTranslation("Filter") || textBox2.Text.Trim() == string.Empty)
			{
				textBox2.ForeColor = Color.Gray;
				textBox2.Text = Config.Instance.GetTranslation("Filter");
			}
		};
		filter.KeyUp += delegate
		{
			if (Configuration.Count > 0)
			{
				IEnumerable<IniEntry> enumerable = Configuration.Where((IniEntry c) => c.Key.ToLower().Contains(filter.Text.ToLower()) || c.Value.ToLower().Contains(filter.Text.ToLower()));
				configList.Items.Clear();
				foreach (IniEntry item5 in enumerable)
				{
					ListViewItem value2 = new ListViewItem
					{
						Text = item5.Key,
						SubItems = { item5.Value }
					};
					configList.Items.Add(value2);
				}
			}
		};
		configList.AfterItemEdit += delegate(object o, CustomLabelEditEventArgs args)
		{
			IniEntry iniEntry = Configuration.FirstOrDefault((IniEntry i) => i.Key.ToLower() == configList.Items[args.Item].Text.ToLower());
			if (iniEntry != null)
			{
				switch (configList.GetSubItemSelected())
				{
				case 1:
					iniEntry.Value = args.Label;
					break;
				case 2:
					iniEntry.Comment = args.Label;
					break;
				}
			}
		};
	}

	private void cancelBtn_Click(object sender, System.EventArgs e)
	{
		Close();
	}

	private void saveBtn_Click(object sender, System.EventArgs e)
	{
		IniParser.Update(ConfigurationFile, Configuration);
		Close();
	}

	private void openBtn_Click(object sender, System.EventArgs e)
	{
		Process.Start(Config.Instance.GetDefaultEditor(), ConfigurationFile);
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
		this.filter = new System.Windows.Forms.TextBox();
		this.saveBtn = new System.Windows.Forms.Button();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.panel2 = new System.Windows.Forms.Panel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.openBtn = new System.Windows.Forms.Button();
		this.tabControl1 = new System.Windows.Forms.TabControl();
		this.allTab = new System.Windows.Forms.TabPage();
		this.configList = new XWM.CustomControls.ListViewCustomControl();
		this.keyColumn = new System.Windows.Forms.ColumnHeader();
		this.valueColumn = new System.Windows.Forms.ColumnHeader();
		this.commentColumn = new System.Windows.Forms.ColumnHeader();
		this.panel1.SuspendLayout();
		this.tabControl1.SuspendLayout();
		this.allTab.SuspendLayout();
		base.SuspendLayout();
		this.filter.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.filter.Location = new System.Drawing.Point(108, 9);
		this.filter.Name = "filter";
		this.filter.Size = new System.Drawing.Size(190, 22);
		this.filter.TabIndex = 1;
		this.saveBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.saveBtn.Location = new System.Drawing.Point(604, 8);
		this.saveBtn.Name = "saveBtn";
		this.saveBtn.Size = new System.Drawing.Size(75, 24);
		this.saveBtn.TabIndex = 5;
		this.saveBtn.Text = "Save";
		this.saveBtn.UseVisualStyleBackColor = true;
		this.saveBtn.Click += new System.EventHandler(saveBtn_Click);
		this.cancelBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.cancelBtn.Location = new System.Drawing.Point(521, 8);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(75, 24);
		this.cancelBtn.TabIndex = 4;
		this.cancelBtn.Text = "Cancel";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.panel2.Location = new System.Drawing.Point(-18, 347);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(718, 1);
		this.panel2.TabIndex = 16;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel1.Controls.Add(this.filter);
		this.panel1.Controls.Add(this.openBtn);
		this.panel1.Controls.Add(this.saveBtn);
		this.panel1.Controls.Add(this.cancelBtn);
		this.panel1.Location = new System.Drawing.Point(-15, 349);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(725, 45);
		this.panel1.TabIndex = 15;
		this.openBtn.Location = new System.Drawing.Point(27, 8);
		this.openBtn.Name = "openBtn";
		this.openBtn.Size = new System.Drawing.Size(75, 24);
		this.openBtn.TabIndex = 3;
		this.openBtn.Text = "Open";
		this.openBtn.UseVisualStyleBackColor = true;
		this.openBtn.Click += new System.EventHandler(openBtn_Click);
		this.tabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl1.Controls.Add(this.allTab);
		this.tabControl1.Location = new System.Drawing.Point(12, 12);
		this.tabControl1.Name = "tabControl1";
		this.tabControl1.SelectedIndex = 0;
		this.tabControl1.Size = new System.Drawing.Size(652, 328);
		this.tabControl1.TabIndex = 17;
		this.allTab.Controls.Add(this.configList);
		this.allTab.Location = new System.Drawing.Point(4, 22);
		this.allTab.Name = "allTab";
		this.allTab.Padding = new System.Windows.Forms.Padding(3);
		this.allTab.Size = new System.Drawing.Size(644, 302);
		this.allTab.TabIndex = 0;
		this.allTab.Text = "All";
		this.allTab.UseVisualStyleBackColor = true;
		this.configList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.configList.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.configList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.keyColumn, this.valueColumn, this.commentColumn });
		this.configList.EditableSubitems = new int[1] { 1 };
		this.configList.FullRowSelect = true;
		this.configList.GridLines = true;
		this.configList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
		this.configList.Location = new System.Drawing.Point(6, 6);
		this.configList.Name = "configList";
		this.configList.Size = new System.Drawing.Size(632, 290);
		this.configList.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.configList.TabIndex = 2;
		this.configList.UseCompatibleStateImageBehavior = false;
		this.configList.View = System.Windows.Forms.View.Details;
		this.keyColumn.Text = "Key";
		this.keyColumn.Width = 197;
		this.valueColumn.Text = "Value";
		this.valueColumn.Width = 186;
		this.commentColumn.Text = "Comment";
		this.commentColumn.Width = 248;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(676, 392);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.tabControl1);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.MinimumSize = new System.Drawing.Size(692, 431);
		base.Name = "ConfigurationEditor";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Configuration editor";
		base.Load += new System.EventHandler(PHPConfigurator_Load);
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.tabControl1.ResumeLayout(false);
		this.allTab.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
