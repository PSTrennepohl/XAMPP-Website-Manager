using Microsoft.VisualBasic.ApplicationServices;

namespace XWM;

public class SingleInstanceController : WindowsFormsApplicationBase
{
	public SingleInstanceController()
	{
		base.IsSingleInstance = true;
		base.StartupNextInstance += this_StartupNextInstance;
	}

	private void this_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
	{
		if (base.MainForm is Main main)
		{
			main.ShowEditor();
			main.Focus();
			main.Activate();
		}
	}

	protected override void OnCreateMainForm()
	{
		base.MainForm = Main.Instance();
	}
}
