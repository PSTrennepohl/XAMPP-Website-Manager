using System.Collections.Generic;

namespace XWM.File;

public class Vhost
{
	public string DocumentRoot { get; set; }

	public string ServerName { get; set; }

	public bool SslEnabled { get; set; }

	public string CertificateFilePath { get; set; }

	public string CertificateKeyPath { get; set; }

	public PhpConfiguration.PhpVersion? PhpVersion { get; set; }

	public List<HostAlias> Aliases { get; set; }

	public bool ExecuteCgi { get; set; }

	public bool DirectoryIndex { get; set; }

	public bool FollowSymLinks { get; set; }

	public bool SymLinksIfOwnerMatch { get; set; }

	public bool Includes { get; set; }

	public List<string> CustomSettings { get; set; }

	public int? Port { get; set; }

	public bool EnableFtp { get; set; }

	public Vhost()
	{
		Aliases = new List<HostAlias>();
		CustomSettings = new List<string>();
		ExecuteCgi = true;
		DirectoryIndex = true;
		FollowSymLinks = true;
		SymLinksIfOwnerMatch = false;
		Includes = true;
	}

	public string GetPhpPath()
	{
		return PhpConfiguration.GetDirectory(PhpVersion);
	}

	public string GetPhpIni()
	{
		return PhpConfiguration.GetIni(PhpVersion);
	}
}
