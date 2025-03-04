using System;

namespace XWM.EventArgs;

public class CreateAliasEventArgs : System.EventArgs
{
	public string AliasName { get; set; }

	public string DirectoryPath { get; set; }
}
