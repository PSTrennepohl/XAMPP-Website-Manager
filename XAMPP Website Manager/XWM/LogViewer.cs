using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using XWM.Application;
using XWM.Data;

namespace XWM;

public class LogViewer : Form
{
	public static int MaxLines = 2000;

	protected Thread RunningThread;

	private IContainer components;

	private Button clearBtn;

	private Button openBtn;

	private Button closeBtn;

	private ListBox logView;

	private Panel panel2;

	private Panel panel1;

	private Button stopBtn;

	private Panel loadingPanel;

	private PictureBox loadingPicture;

	private ContextMenuStrip logViewerContextMenuStrip;

	private ToolStripMenuItem copyToolStripMenuItem;

	private Button refreshBtn;

	private System.Windows.Forms.Timer logTimer;

	protected string File { get; set; }

	protected FileSystemWatcher FileWatcher { get; set; }

	public LogViewer(string file)
	{
		InitializeComponent();
		File = file;
		Text = Config.Instance.GetTranslation("LogViewer") + " - " + File;
		refreshBtn.Text = Config.Instance.GetTranslation("Refresh");
		stopBtn.Text = Config.Instance.GetTranslation("Stop");
		clearBtn.Text = Config.Instance.GetTranslation("Clear");
		closeBtn.Text = Config.Instance.GetTranslation("Close");
		openBtn.Text = Config.Instance.GetTranslation("Open");
		copyToolStripMenuItem.Text = Config.Instance.GetTranslation("Copy");
		base.Icon = (Icon)Resources.Get().GetObject("log_file");
		loadingPicture.Image = (Image)Resources.Get().GetObject("loading");
		loadingPanel.Visible = false;
		if (Config.Instance.LogViewerWidth > 0 && Config.Instance.LogViewerHeight > 0)
		{
			base.Width = Config.Instance.LogViewerWidth;
			base.Height = Config.Instance.LogViewerHeight;
		}
		base.Closing += delegate
		{
			Config.Instance.LogViewerWidth = base.Width;
			Config.Instance.LogViewerHeight = base.Height;
			Config.Instance.Save();
		};
		base.Resize += delegate
		{
			if (logView.Items.Count > 0)
			{
				logView.TopIndex = logView.Items.Count - 1;
				logView.SelectedIndex = logView.Items.Count - 1;
			}
		};
	}

	protected void SetLines(object[] lines)
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action<object[]>(SetLines), new object[1] { lines });
			return;
		}
		logView.Items.AddRange(lines);
		logView.TopIndex = logView.Items.Count - 1;
		logView.SelectedIndex = logView.Items.Count - 1;
		loadingPanel.SendToBack();
		loadingPanel.Visible = false;
	}

	protected void ReadLines(bool update = true)
	{
		List<object> list = new List<object>();
		string text = ReadLastLinesInFile(File, MaxLines, Encoding.UTF8, Environment.NewLine);
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		string[] array = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
		foreach (string text2 in array)
		{
			if (!logView.Items.Contains(text2))
			{
				if (update && !string.IsNullOrEmpty(text2))
				{
					AddLine(text2);
				}
				else
				{
					list.Add(text2);
				}
			}
		}
		if (!update)
		{
			ClearLines();
			SetLines(list.ToArray());
		}
	}

	private void LogViewer_Load(object sender, System.EventArgs e)
	{
		try
		{
			if (!System.IO.File.Exists(File))
			{
				System.IO.File.Create(File);
			}
			ShowLoader();
			RunningThread = new Thread((ThreadStart)delegate
			{
				ReadLines(update: false);
			});
			RunningThread.Start();
			logTimer.Enabled = true;
		}
		catch (Exception)
		{
			MessageBox.Show(Config.Instance.GetTranslation("ErrorLogFileDontExist"), Config.Instance.GetTranslation("Error"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	protected string ReadLastLinesInFile(string path, long numberOfTokens, Encoding encoding, string tokenSeparator)
	{
		int byteCount = encoding.GetByteCount(Environment.NewLine);
		byte[] bytes = encoding.GetBytes(tokenSeparator);
		try
		{
			using FileStream fileStream = System.IO.File.Open(File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			long num = 0L;
			long num2 = fileStream.Length / byteCount;
			for (long num3 = byteCount; num3 < num2; num3 += byteCount)
			{
				fileStream.Seek(-num3, SeekOrigin.End);
				fileStream.Read(bytes, 0, bytes.Length);
				if (encoding.GetString(bytes) == tokenSeparator)
				{
					num++;
					if (num == numberOfTokens)
					{
						byte[] array = new byte[fileStream.Length - fileStream.Position];
						fileStream.Read(array, 0, array.Length);
						return encoding.GetString(array);
					}
				}
			}
			fileStream.Seek(0L, SeekOrigin.Begin);
			bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, bytes.Length);
			return encoding.GetString(bytes);
		}
		catch (Exception)
		{
		}
		return string.Empty;
	}

	protected void ClearLines()
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action(ClearLines));
		}
		else if (logView != null && logView.Items.Count > 0)
		{
			logView.Items.Clear();
		}
	}

	protected void AddLine(string line)
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action<string>(AddLine), line);
			return;
		}
		bool num = logView.SelectedIndex == logView.Items.Count - 1;
		logView.Items.Add(line);
		loadingPanel.SendToBack();
		loadingPanel.Visible = false;
		if (num)
		{
			logView.SelectedIndex = -1;
			logView.TopIndex = logView.Items.Count - 1;
			logView.SelectedIndex = logView.Items.Count - 1;
		}
	}

	protected void ShowLoader()
	{
		if (base.InvokeRequired)
		{
			Invoke(new Action(ShowLoader));
			return;
		}
		loadingPanel.Visible = true;
		loadingPanel.BringToFront();
	}

	private void closeBtn_Click(object sender, System.EventArgs e)
	{
		Close();
	}

	private void stopBtn_Click(object sender, System.EventArgs e)
	{
		logTimer.Enabled = !logTimer.Enabled;
		FileWatcher.EnableRaisingEvents = !FileWatcher.EnableRaisingEvents;
		stopBtn.Text = ((!FileWatcher.EnableRaisingEvents) ? Config.Instance.GetTranslation("Start") : Config.Instance.GetTranslation("Stop"));
	}

	private void openBtn_Click(object sender, System.EventArgs e)
	{
		Process.Start(Config.Instance.GetDefaultEditor(), File);
	}

	private void clearBtn_Click(object sender, System.EventArgs e)
	{
		logView.Items.Clear();
	}

	private void logViewerContextMenuStrip_Opening(object sender, CancelEventArgs e)
	{
		copyToolStripMenuItem.Visible = logView.SelectedItems.Count > 0;
	}

	private void copyToolStripMenuItem_Click(object sender, System.EventArgs e)
	{
		Clipboard.Clear();
		if (logView.SelectedItems.Count > 0)
		{
			string[] array = new string[logView.SelectedItems.Count];
			for (int i = 0; i < logView.SelectedItems.Count; i++)
			{
				array[i] = logView.SelectedItems[i].ToString();
			}
			Clipboard.SetText(string.Join(Environment.NewLine, array));
		}
	}

	private void btnRefresh_Click(object sender, System.EventArgs e)
	{
		ShowLoader();
		if (RunningThread.IsAlive)
		{
			RunningThread.Abort();
			RunningThread = null;
		}
		RunningThread = new Thread((ThreadStart)delegate
		{
			ReadLines();
		});
		RunningThread.Start();
	}

	private void logTimer_Tick(object sender, System.EventArgs e)
	{
		RunningThread = new Thread((ThreadStart)delegate
		{
			ReadLines();
		});
		RunningThread.Start();
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
		this.clearBtn = new System.Windows.Forms.Button();
		this.openBtn = new System.Windows.Forms.Button();
		this.closeBtn = new System.Windows.Forms.Button();
		this.logView = new System.Windows.Forms.ListBox();
		this.logViewerContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel2 = new System.Windows.Forms.Panel();
		this.panel1 = new System.Windows.Forms.Panel();
		this.refreshBtn = new System.Windows.Forms.Button();
		this.stopBtn = new System.Windows.Forms.Button();
		this.loadingPanel = new System.Windows.Forms.Panel();
		this.loadingPicture = new System.Windows.Forms.PictureBox();
		this.logTimer = new System.Windows.Forms.Timer(this.components);
		this.logViewerContextMenuStrip.SuspendLayout();
		this.panel1.SuspendLayout();
		this.loadingPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).BeginInit();
		base.SuspendLayout();
		this.clearBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.clearBtn.Location = new System.Drawing.Point(174, 8);
		this.clearBtn.Name = "clearBtn";
		this.clearBtn.Size = new System.Drawing.Size(75, 23);
		this.clearBtn.TabIndex = 1;
		this.clearBtn.Text = "Clear";
		this.clearBtn.UseVisualStyleBackColor = true;
		this.clearBtn.Click += new System.EventHandler(clearBtn_Click);
		this.openBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.openBtn.Location = new System.Drawing.Point(255, 8);
		this.openBtn.Name = "openBtn";
		this.openBtn.Size = new System.Drawing.Size(75, 23);
		this.openBtn.TabIndex = 2;
		this.openBtn.Text = "Open";
		this.openBtn.UseVisualStyleBackColor = true;
		this.openBtn.Click += new System.EventHandler(openBtn_Click);
		this.closeBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.closeBtn.Location = new System.Drawing.Point(582, 8);
		this.closeBtn.Name = "closeBtn";
		this.closeBtn.Size = new System.Drawing.Size(75, 23);
		this.closeBtn.TabIndex = 3;
		this.closeBtn.Text = "Close";
		this.closeBtn.UseVisualStyleBackColor = true;
		this.closeBtn.Click += new System.EventHandler(closeBtn_Click);
		this.logView.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.logView.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.logView.ContextMenuStrip = this.logViewerContextMenuStrip;
		this.logView.Items.AddRange(new object[2] { "Entry 1", "Entry 2" });
		this.logView.Location = new System.Drawing.Point(0, 1);
		this.logView.Name = "logView";
		this.logView.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
		this.logView.Size = new System.Drawing.Size(669, 312);
		this.logView.TabIndex = 5;
		this.logViewerContextMenuStrip.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.logViewerContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.copyToolStripMenuItem });
		this.logViewerContextMenuStrip.Name = "logViewerContextMenuStrip";
		this.logViewerContextMenuStrip.Size = new System.Drawing.Size(101, 26);
		this.logViewerContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(logViewerContextMenuStrip_Opening);
		this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		this.copyToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
		this.copyToolStripMenuItem.Text = "Copy";
		this.copyToolStripMenuItem.Click += new System.EventHandler(copyToolStripMenuItem_Click);
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
		this.panel2.Location = new System.Drawing.Point(-3, 314);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(676, 1);
		this.panel2.TabIndex = 16;
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel1.Controls.Add(this.refreshBtn);
		this.panel1.Controls.Add(this.stopBtn);
		this.panel1.Controls.Add(this.closeBtn);
		this.panel1.Controls.Add(this.openBtn);
		this.panel1.Controls.Add(this.clearBtn);
		this.panel1.Location = new System.Drawing.Point(0, 316);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(683, 45);
		this.panel1.TabIndex = 15;
		this.refreshBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.refreshBtn.Location = new System.Drawing.Point(93, 8);
		this.refreshBtn.Name = "refreshBtn";
		this.refreshBtn.Size = new System.Drawing.Size(75, 23);
		this.refreshBtn.TabIndex = 5;
		this.refreshBtn.Text = "Refresh";
		this.refreshBtn.UseVisualStyleBackColor = true;
		this.refreshBtn.Click += new System.EventHandler(btnRefresh_Click);
		this.stopBtn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.stopBtn.Location = new System.Drawing.Point(12, 8);
		this.stopBtn.Name = "stopBtn";
		this.stopBtn.Size = new System.Drawing.Size(75, 23);
		this.stopBtn.TabIndex = 4;
		this.stopBtn.Text = "Stop";
		this.stopBtn.UseVisualStyleBackColor = true;
		this.stopBtn.Click += new System.EventHandler(stopBtn_Click);
		this.loadingPanel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.loadingPanel.BackColor = System.Drawing.Color.White;
		this.loadingPanel.Controls.Add(this.loadingPicture);
		this.loadingPanel.Location = new System.Drawing.Point(0, 1);
		this.loadingPanel.Name = "loadingPanel";
		this.loadingPanel.Size = new System.Drawing.Size(669, 357);
		this.loadingPanel.TabIndex = 5;
		this.loadingPicture.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.loadingPicture.Location = new System.Drawing.Point(302, 131);
		this.loadingPicture.Name = "loadingPicture";
		this.loadingPicture.Size = new System.Drawing.Size(65, 64);
		this.loadingPicture.TabIndex = 15;
		this.loadingPicture.TabStop = false;
		this.logTimer.Interval = 1000;
		this.logTimer.Tick += new System.EventHandler(logTimer_Tick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.White;
		base.ClientSize = new System.Drawing.Size(669, 357);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.logView);
		base.Controls.Add(this.loadingPanel);
		this.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.MinimumSize = new System.Drawing.Size(685, 396);
		base.Name = "LogViewer";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Log viewer";
		base.Load += new System.EventHandler(LogViewer_Load);
		this.logViewerContextMenuStrip.ResumeLayout(false);
		this.panel1.ResumeLayout(false);
		this.loadingPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.loadingPicture).EndInit();
		base.ResumeLayout(false);
	}
}
