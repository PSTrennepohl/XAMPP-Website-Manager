using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace XWM.File;

public class Vhosts
{
	protected static void WritePhpHost(StreamWriter writer, Vhost host)
	{
		if (host.PhpVersion.HasValue)
		{
			writer.WriteLine("\t# PHP: " + host.PhpVersion);
			string text = host.GetPhpPath().Replace("\\", "/");
			writer.WriteLine("\tSetEnv MIBDIRS \"{0}/extras/mibs\"", text);
			writer.WriteLine("\tSetEnv PHP_PEAR_SYSCONF_DIR \"{0}\"", text);
			writer.WriteLine("\tSetEnv PHPRC \"{0}\"", text);
			writer.WriteLine("");
			writer.WriteLine("\t<IfModule actions_module>");
			writer.WriteLine("\t\tAction application/x-httpd-{0} \"/{0}/php-cgi.exe\"", host.PhpVersion);
			writer.WriteLine("\t</IfModule>");
			writer.WriteLine("");
			writer.WriteLine("\tScriptAlias /{0}/ \"{1}/\"", host.PhpVersion, text);
			writer.WriteLine("\t<Directory \"{0}\">", text);
			writer.WriteLine("\t\tAllowOverride None");
			writer.WriteLine("\t\tOptions None");
			writer.WriteLine("\t\tRequire all denied");
			writer.WriteLine("\t\t<Files \"php-cgi.exe\">");
			writer.WriteLine("\t\t\tRequire all granted");
			writer.WriteLine("\t\t</Files>");
			writer.WriteLine("\t</Directory>");
			writer.WriteLine("");
			writer.WriteLine("\t<FilesMatch \"\\.php$\">");
			writer.WriteLine("\t\tSetHandler application/x-httpd-{0}", host.PhpVersion);
			writer.WriteLine("\t</FilesMatch>");
			writer.WriteLine("");
		}
	}

	protected static void WriteOptions(StreamWriter writer, Vhost host)
	{
		if (host.FollowSymLinks || host.DirectoryIndex || host.Includes || host.ExecuteCgi || host.SymLinksIfOwnerMatch)
		{
			writer.Write("\t\tOptions ");
			if (host.DirectoryIndex)
			{
				writer.Write("Indexes ");
			}
			if (host.FollowSymLinks)
			{
				writer.Write("FollowSymLinks ");
			}
			if (host.SymLinksIfOwnerMatch)
			{
				writer.Write("SymLinksIfOwnerMatch ");
			}
			if (host.Includes)
			{
				writer.Write("Includes ");
			}
			if (host.ExecuteCgi)
			{
				writer.Write("ExecCGI");
			}
			writer.WriteLine("");
		}
	}

	protected static void WriteAliases(StreamWriter writer, Vhost host)
	{
		if (host.Aliases.Count <= 0)
		{
			return;
		}
		writer.WriteLine("");
		writer.WriteLine("\t# Aliases");
		foreach (HostAlias alias in host.Aliases)
		{
			writer.WriteLine("\tAlias {0} \"{1}\"", alias.AliasName, alias.Directory);
			writer.WriteLine("\t<Directory \"" + alias.Directory + "\">");
			WriteOptions(writer, host);
			writer.WriteLine("\t\tRequire all granted\n\t\tAllowOverride All\n\t</Directory>");
		}
		writer.WriteLine("");
	}

	public static void WriteVhosts(string vhostFile, List<Vhost> list)
	{
		using (StreamWriter streamWriter = new StreamWriter(vhostFile + ".tmp"))
		{
			streamWriter.WriteLine("#");
			streamWriter.WriteLine("# " + System.Windows.Forms.Application.ProductName);
			streamWriter.WriteLine("# version: " + System.Windows.Forms.Application.ProductVersion);
			streamWriter.WriteLine("#");
			streamWriter.WriteLine("# ------------------------------------------------------------");
			streamWriter.WriteLine("# DO NOT EDIT BELOW THIS LINE!");
			streamWriter.WriteLine("# THIS FILES CONTAINS INFORMATION ABOUT YOUR WEBSITES.");
			streamWriter.WriteLine("# DO NOT MODIFY THIS FILE UNLESS YOU KNOW WHAT YOU ARE DOING!");
			streamWriter.WriteLine("# ------------------------------------------------------------");
			streamWriter.WriteLine("");
			streamWriter.WriteLine("NameVirtualHost *");
			streamWriter.WriteLine("");
			List<int> list2 = (from host in list
				where host.Port.HasValue && !new int?[2] { 80, 443 }.Contains(host.Port)
				select host.Port.Value).ToList();
			if (list2.Count > 0)
			{
				streamWriter.WriteLine("# OPEN CUSTOM PORTS");
				foreach (int item in list2)
				{
					streamWriter.WriteLine("Listen {0}", item);
				}
				streamWriter.WriteLine("");
			}
			foreach (Vhost item2 in list)
			{
				int num = item2.Port ?? 80;
				streamWriter.WriteLine("# WEBSITE: " + item2.ServerName);
				streamWriter.WriteLine("<VirtualHost {0}:{1}>", item2.ServerName, num);
				streamWriter.WriteLine("\tDocumentRoot \"" + item2.DocumentRoot + "\"");
				streamWriter.WriteLine("\tServerName " + item2.ServerName);
				streamWriter.WriteLine("");
				if (item2.CustomSettings.Count > 0)
				{
					foreach (string customSetting in item2.CustomSettings)
					{
						streamWriter.WriteLine("\t" + customSetting + "\t# Custom setting");
					}
				}
				streamWriter.WriteLine("");
				WritePhpHost(streamWriter, item2);
				streamWriter.WriteLine("\t<Directory \"" + item2.DocumentRoot + "\">");
				WriteOptions(streamWriter, item2);
				streamWriter.WriteLine("\t\tRequire all granted\n\t\tAllowOverride All\n\t</Directory>");
				WriteAliases(streamWriter, item2);
				streamWriter.WriteLine("</VirtualHost>");
				streamWriter.WriteLine("");
				if (!item2.SslEnabled || item2.CertificateFilePath == null)
				{
					continue;
				}
				streamWriter.WriteLine("# ENABLING SSL FOR WEBSITE: " + item2.ServerName);
				streamWriter.WriteLine("<VirtualHost {0}:443>", item2.ServerName);
				streamWriter.WriteLine("\tSSLEngine on");
				streamWriter.WriteLine("\tSSLCertificateFile " + item2.CertificateFilePath);
				streamWriter.WriteLine("\tSSLCertificateKeyFile " + item2.CertificateKeyPath);
				streamWriter.WriteLine("\tDocumentRoot \"" + item2.DocumentRoot + "\"");
				streamWriter.WriteLine("\tServerName " + item2.ServerName);
				streamWriter.WriteLine("");
				if (item2.CustomSettings.Count > 0)
				{
					foreach (string customSetting2 in item2.CustomSettings)
					{
						streamWriter.WriteLine("\t" + customSetting2 + "\t# Custom setting");
					}
				}
				streamWriter.WriteLine("");
				WritePhpHost(streamWriter, item2);
				streamWriter.WriteLine("\t<Directory \"" + item2.DocumentRoot + "\">\n\t\tOptions Indexes FollowSymLinks Includes ExecCGI\n\t\tRequire all granted\n\t\tAllowOverride All\n\t</Directory>");
				WriteAliases(streamWriter, item2);
				streamWriter.WriteLine("</VirtualHost>");
				streamWriter.WriteLine("");
			}
		}
		System.IO.File.Copy(vhostFile + ".tmp", vhostFile, overwrite: true);
		System.IO.File.Delete(vhostFile + ".tmp");
	}

	public static List<Vhost> ReadVhosts(string vhostFile)
	{
		List<Vhost> list = new List<Vhost>();
		using StreamReader streamReader = new StreamReader(vhostFile);
		while (!streamReader.EndOfStream)
		{
			string text = streamReader.ReadLine();
			if (text == null || (text.Trim().IndexOf("#", StringComparison.Ordinal) == 0 && !text.Contains("# PHP: ")))
			{
				continue;
			}
			if (text.Contains("<VirtualHost"))
			{
				Match match = new Regex("\\<VirtualHost\\ .*\\:([0-9]*?)\\>", RegexOptions.IgnoreCase).Match(text.Trim());
				if (match.Success && match.Groups.Count > 0)
				{
					string value = match.Groups[1].Value;
					if (!new string[1] { "443" }.Contains(value))
					{
						Vhost vhost = new Vhost();
						int.TryParse(value, out var result);
						if (result != 80)
						{
							vhost.Port = result;
						}
						list.Add(vhost);
					}
				}
			}
			if (list.Count <= 0)
			{
				continue;
			}
			Vhost vhost2 = list.Last();
			if (text.Contains("DocumentRoot"))
			{
				vhost2.DocumentRoot = text.Replace("DocumentRoot", "").Replace("\"", string.Empty).Trim();
			}
			if (text.Contains("ServerName"))
			{
				vhost2.ServerName = text.Replace("ServerName", "").Trim();
			}
			if (text.Contains("# PHP: "))
			{
				Enum.TryParse<PhpConfiguration.PhpVersion>(text.Replace("# PHP: ", string.Empty).Trim(), out var result2);
				vhost2.PhpVersion = result2;
			}
			if (text.Contains("# Custom setting") && !vhost2.SslEnabled)
			{
				vhost2.CustomSettings.Add(text.Replace("# Custom setting", string.Empty).Trim());
			}
			if (text.Contains("SSLCertificateKeyFile"))
			{
				vhost2.SslEnabled = true;
				vhost2.CertificateKeyPath = text.Replace("SSLCertificateKeyFile", "").Trim();
			}
			if (text.Contains("SSLCertificateFile"))
			{
				vhost2.SslEnabled = true;
				vhost2.CertificateFilePath = text.Replace("SSLCertificateFile", "").Trim();
			}
			if (text.Contains("Options"))
			{
				string[] array = text.Replace("Options ", string.Empty).Trim().Split(' ');
				for (int i = 0; i < array.Length; i++)
				{
					switch (array[i].Trim().ToLower())
					{
					case "followsymlinks":
						vhost2.FollowSymLinks = true;
						break;
					case "indexes":
						vhost2.DirectoryIndex = true;
						break;
					case "symlinksifownermatch":
						vhost2.SymLinksIfOwnerMatch = true;
						break;
					case "execcgi":
						vhost2.ExecuteCgi = true;
						break;
					case "includes":
						vhost2.Includes = true;
						break;
					}
				}
			}
			if (!text.Contains("Alias "))
			{
				continue;
			}
			Match match2 = new Regex("^Alias\\ (\\/.*?)\\ \"([\\\\\\/]*?.*?)\"$", RegexOptions.IgnoreCase).Match(text.Trim());
			if (match2.Success && match2.Groups.Count == 3)
			{
				GroupCollection matches = match2.Groups;
				if (vhost2.Aliases.FirstOrDefault((HostAlias a) => a.AliasName.ToLower() == matches[1].Value.ToLower()) == null)
				{
					HostAlias item = new HostAlias
					{
						AliasName = matches[1].Value,
						Directory = matches[2].Value
					};
					vhost2.Aliases.Add(item);
				}
			}
		}
		return list;
	}
}
