using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XWM.File;

public class IniParser
{
	public static IniEntryCollection Parse(string configurationFile)
	{
		IniEntryCollection iniEntryCollection = new IniEntryCollection();
		string[] array = System.IO.File.ReadAllLines(configurationFile);
		if (array.Any())
		{
			string group = null;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Contains(";") || text.Contains("#"))
				{
					continue;
				}
				Match match = new Regex("^\\[([\\w\\s\\-\\\\_]*?)\\]").Match(text);
				if (match.Success && match.Groups.Count > 0)
				{
					group = match.Groups[1].Value;
					continue;
				}
				string text2 = text;
				string comment = string.Empty;
				if (text.Contains("#"))
				{
					text2 = text2.Substring(0, text2.IndexOf("#") - 1);
					comment = text.Split('#')[1].Trim();
				}
				if (text.Contains(";"))
				{
					text2 = text2.Substring(0, text2.IndexOf(";") - 1);
					comment = text.Split(';')[1].Trim();
				}
				if (!string.IsNullOrEmpty(text2))
				{
					string key = text2;
					string value = null;
					if (text.Contains("="))
					{
						string[] array3 = text2.Split('=');
						key = array3[0].Trim();
						value = array3[1].Trim();
					}
					IniEntry item = new IniEntry
					{
						Group = group,
						Key = key,
						Value = value,
						Comment = comment
					};
					iniEntryCollection.Add(item);
				}
			}
		}
		return iniEntryCollection;
	}

	public static void Write(string configurationFile, IniEntryCollection entries)
	{
		if (System.IO.File.Exists(configurationFile))
		{
			System.IO.File.Delete(configurationFile);
		}
		string text = string.Empty;
		if (entries.Count > 0)
		{
			List<string> list = new List<string>();
			foreach (IniEntry item in entries.OrderBy((IniEntry e) => e.Group))
			{
				if (!list.Contains(item.Group))
				{
					string arg = ((list.Count == 0) ? string.Empty : Environment.NewLine);
					text += $"{arg}[{item.Group}]{Environment.NewLine}";
					list.Add(item.Group);
				}
				if (item.Key != null)
				{
					text += $"{item.Key}={item.Value}{Environment.NewLine}";
				}
				else if (string.IsNullOrEmpty(item.Value))
				{
					text += $"{item.Value}{Environment.NewLine}";
				}
			}
		}
		FileInfo fileInfo = new FileInfo(configurationFile);
		if (fileInfo.DirectoryName != null && !Directory.Exists(fileInfo.DirectoryName))
		{
			Directory.CreateDirectory(fileInfo.DirectoryName);
		}
		System.IO.File.WriteAllText(configurationFile, text.Trim());
		Parse(configurationFile);
	}

	public static void Update(string configurationFile, IniEntryCollection entries)
	{
		string[] array = System.IO.File.ReadAllLines(configurationFile);
		if (!array.Any())
		{
			return;
		}
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.IndexOf(";") != 0 && text.IndexOf("#") != 0)
			{
				string key = text;
				if (text.Contains("="))
				{
					string[] array3 = text.Split('=');
					key = array3[0].Trim();
				}
				if (!string.IsNullOrEmpty(key))
				{
					IniEntry iniEntry = entries.FirstOrDefault((IniEntry e) => e.Key == key);
					if (iniEntry != null)
					{
						string text2 = ((!string.IsNullOrEmpty(iniEntry.Comment)) ? ("\t# " + iniEntry.Comment) : string.Empty);
						if (iniEntry.Value == null)
						{
							array[num] = $"{iniEntry.Key} {text2}";
						}
						else
						{
							array[num] = $"{iniEntry.Key}={iniEntry.Value}{text2}";
						}
					}
					entries.Remove(iniEntry);
				}
			}
			num++;
		}
		num = 0;
		array2 = array;
		Func<IniEntry, bool> func = default(Func<IniEntry, bool>);
		foreach (string input in array2)
		{
			Match match = new Regex("^\\[([\\w\\s\\-\\\\_]*?)\\]$").Match(input);
			string group = null;
			if (match.Success && match.Groups.Count > 0)
			{
				group = match.Groups[1].Value.Trim();
			}
			if (group != null)
			{
				Func<IniEntry, bool> func2 = func;
				if (func2 == null)
				{
					func2 = (func = (IniEntry e) => e.Group == group);
				}
				foreach (IniEntry item in entries.Where(func2))
				{
					string arg = ((!string.IsNullOrEmpty(item.Comment)) ? ("\t# " + item.Comment) : string.Empty);
					array[num] = array[num] + Environment.NewLine + $"{item.Key}={item.Value}{arg}";
				}
			}
			num++;
		}
		System.IO.File.WriteAllLines(configurationFile, array.Where((string l) => !string.IsNullOrEmpty(l)));
	}
}
