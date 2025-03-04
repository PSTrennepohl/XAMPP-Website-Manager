using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using XWM.EventArgs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace XWM.CustomControls;

public class ListViewCustomControl : ListView
{
	public delegate void ItemEditEventHandler(object sender, CustomLabelEditEventArgs e);

	private ListViewItem _li;

	private int _x;

	private int _y;

	private string _subItemText;

	private int _subItemSelected;

	private readonly TextBox _editBox = new TextBox();

	private Font _font;

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // Garantir visibilidade no Designer
    public override Font Font
	{
		get
		{
			return base.Font;
		}
		set
		{
			base.Font = value;
			_editBox.Font = value;
			_font = value;
		}
	}

    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)] // Garantir visibilidade no Designerpublic int[] EditableSubitems { get; set; }

    public event ItemEditEventHandler AfterItemEdit;

	public ListViewCustomControl()
	{
		base.FullRowSelect = true;
		base.Size = new Size(0, 0);
		base.TabIndex = 0;
		base.View = View.Details;
		base.MouseDown += OnMouseDown;
		base.DoubleClick += OnDoubleClick;
		base.GridLines = true;
		_editBox.Size = new Size(0, 0);
		_editBox.Location = new Point(0, 0);
		base.Controls.AddRange(new Control[1] { _editBox });
		_editBox.KeyPress += EditBoxOnKeyPress;
		_editBox.LostFocus += EditBoxOnLostFocus;
		_editBox.Font = _font;
		_editBox.BackColor = Color.White;
		_editBox.BorderStyle = BorderStyle.Fixed3D;
		_editBox.Height = 11;
		_editBox.Hide();
        _editBox.Text = string.Empty;
        InitializeComponent();
    }
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public int[] EditableSubitems { get; set; }
    private void OnDoubleClick(object sender, System.EventArgs eventArgs)
	{
		int num = _x;
		int num2 = 0;
		int num3 = 0;
		_subItemSelected = 0;
		for (int i = 0; i < base.Columns.Count; i++)
		{
			if (num > num3 && num3 > 0)
			{
				_subItemSelected = i;
			}
			num3 += base.Columns[i].Width;
		}
		if (EditableSubitems != null && !EditableSubitems.Contains(_subItemSelected))
		{
			return;
		}
		_subItemText = _li.SubItems[_subItemSelected].Text;
		int num4 = 0;
		num3 = 0;
		num2 = 0;
		for (int j = 0; j < base.Columns.Count; j++)
		{
			num2 = num3;
			num3 += base.Columns[j].Width;
			if (j == _subItemSelected)
			{
				num4 = base.Columns[j].Width;
				break;
			}
		}
		_editBox.Size = new Size(num4, _li.Bounds.Bottom - _li.Bounds.Top);
		_editBox.Location = new Point(num2, _li.Bounds.Y);
		_editBox.Show();
		_editBox.Text = _subItemText;
		_editBox.SelectAll();
		_editBox.Focus();
	}

	private void OnMouseDown(object sender, MouseEventArgs mouseEventArgs)
	{
		_li = GetItemAt(mouseEventArgs.X, mouseEventArgs.Y);
		_x = mouseEventArgs.X;
		_y = mouseEventArgs.Y;
	}

	protected void SetValue()
	{
		CustomLabelEditEventArgs customLabelEditEventArgs = new CustomLabelEditEventArgs(_li.Index, _subItemSelected, _editBox.Text);
		OnAfterItemEdit(customLabelEditEventArgs);
		if (!customLabelEditEventArgs.CancelEdit)
		{
			_li.SubItems[_subItemSelected].Text = _editBox.Text;
		}
		_editBox.Hide();
		Focus();
	}

	private void EditBoxOnKeyPress(object sender, KeyPressEventArgs keyPressEventArgs)
	{
		if (keyPressEventArgs.KeyChar == '\r')
		{
			SetValue();
			keyPressEventArgs.Handled = true;
		}
		if (keyPressEventArgs.KeyChar == '\u001b')
		{
			_editBox.Hide();
			keyPressEventArgs.Handled = true;
		}
	}

	private void EditBoxOnLostFocus(object sender, System.EventArgs eventArgs)
	{
		SetValue();
	}

	protected void OnAfterItemEdit(CustomLabelEditEventArgs e)
	{
		if (this.AfterItemEdit != null)
		{
			this.AfterItemEdit(this, e);
		}
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		base.ResumeLayout(false);
	}

	public int GetSubItemSelected()
	{
		return _subItemSelected;
	}
}
