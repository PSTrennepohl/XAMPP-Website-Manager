using System.IO;
using System.Runtime.InteropServices;

namespace XWM.File;

public class SymLink
{
	public enum SymbolicLink
	{
		File,
		Directory
	}

	[DllImport("kernel32.dll")]
	private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

	public static void Create(string source, string destination, SymbolicLink type)
	{
		CreateSymbolicLink(destination, source, type);
	}

	public static bool Exists(string source)
	{
		return System.IO.File.Exists(source);
	}
}
