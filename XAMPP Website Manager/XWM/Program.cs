using System;
using System.Windows.Forms;

namespace XWM;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		System.Windows.Forms.Application.EnableVisualStyles();
		System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		new SingleInstanceController().Run(commandLineArgs);
	}
}
