using System.Collections.Generic;
using System.Linq;

namespace XWM.File;

public class IniEntryCollection : List<IniEntry>
{
	public void AddOrReplace(string group, object value)
	{
		IniEntry iniEntry = this.FirstOrDefault((IniEntry e) => e.Group == group && e.Value == value.ToString());
		if (iniEntry == null)
		{
			iniEntry = new IniEntry
			{
				Group = group,
				Key = null,
				Value = ((value == null) ? string.Empty : value.ToString())
			};
			Add(iniEntry);
		}
		else
		{
			iniEntry.Value = ((value == null) ? string.Empty : value.ToString());
		}
	}

	public void Remove(string group, string key)
	{
		IniEntry iniEntry = this.SingleOrDefault((IniEntry e) => e.Group == group && e.Key == key);
		if (iniEntry != null)
		{
			Remove(iniEntry);
		}
	}

	public void AddOrReplace(string group, string key, object value)
	{
		IniEntry iniEntry = this.FirstOrDefault((IniEntry e) => e.Group == group && e.Key == key);
		if (iniEntry == null)
		{
			iniEntry = new IniEntry
			{
				Group = group,
				Key = key,
				Value = ((value == null) ? string.Empty : value.ToString())
			};
			Add(iniEntry);
		}
		else
		{
			iniEntry.Value = ((value == null) ? string.Empty : value.ToString());
		}
	}

	public object FindObject(string group, string key, object defaultValue)
	{
		IniEntry iniEntry = this.FirstOrDefault((IniEntry s) => s.Group == group && s.Key == key);
		if (iniEntry != null)
		{
			return iniEntry.Value;
		}
		return defaultValue;
	}

	public bool FindBool(string group, string key, bool defaultValue)
	{
		return this.FirstOrDefault((IniEntry s) => s.Group == group && s.Key == key)?.GetBool(defaultValue) ?? defaultValue;
	}

	public int FindInt(string group, string key, int defaultValue)
	{
		return this.FirstOrDefault((IniEntry s) => s.Group == group && s.Key == key)?.GetInt(defaultValue) ?? defaultValue;
	}

	public long FindLong(string group, string key, long defaultValue)
	{
		return this.FirstOrDefault((IniEntry s) => s.Group == group && s.Key == key)?.GetLong(defaultValue) ?? defaultValue;
	}

	public string FindString(string group, string key, string defaultValue)
	{
		object obj = FindObject(group, key, defaultValue);
		if (obj != null)
		{
			return obj.ToString();
		}
		return defaultValue;
	}
}
