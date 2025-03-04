using System.Windows.Forms;

namespace XWM.EventArgs;

public class CustomLabelEditEventArgs : LabelEditEventArgs
{
	public int Index { get; set; }

	public CustomLabelEditEventArgs(int item)
		: base(item)
	{
	}

	public CustomLabelEditEventArgs(int item, int index, string label)
		: base(item, label)
	{
		Index = index;
	}
}
