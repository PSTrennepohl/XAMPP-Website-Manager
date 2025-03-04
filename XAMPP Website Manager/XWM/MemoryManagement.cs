using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace XWM;

public class MemoryManagement
{
	[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
	private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);

	[DllImport("psapi.dll")]
	private static extern int EmptyWorkingSet(IntPtr hwProc);

	public static void FlushMemory()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
		if (Environment.OSVersion.Platform == PlatformID.Win32NT)
		{
			SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
		}
	}

	public static void MinimizeFootprint()
	{
		EmptyWorkingSet(Process.GetCurrentProcess().Handle);
	}
}
