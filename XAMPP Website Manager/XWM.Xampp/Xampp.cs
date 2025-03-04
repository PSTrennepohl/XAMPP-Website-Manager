using System.IO;
using System.Linq;
using XWM.Application;
using XWM.File;

namespace XWM.Xampp;

internal static class Xampp
{
	public static string GetVersion()
	{
		string text = XWM.Application.Config.Instance.XamppDirectory + "\\properties.ini";
		if (System.IO.File.Exists(text))
		{
			IniEntry iniEntry = IniParser.Parse(text).FirstOrDefault((IniEntry p) => p.Key.ToLower() == "base_stack_version");
			if (iniEntry != null)
			{
				return iniEntry.Value;
			}
		}
		return null;
	}
}
