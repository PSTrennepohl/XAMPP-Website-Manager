namespace XWM.File;

public class IniEntry
{
	public string Group { get; set; }

	public string Key { get; set; }

	public string Value { get; set; }

	public string Comment { get; set; }

	public bool GetBool(bool defaultValue)
	{
		if (bool.TryParse(Value, out var result))
		{
			return result;
		}
		return defaultValue;
	}

	public int GetInt(int defaultValue)
	{
		if (int.TryParse(Value, out var result))
		{
			return result;
		}
		return defaultValue;
	}

	public long GetLong(long defaultValue)
	{
		if (long.TryParse(Value, out var result))
		{
			return result;
		}
		return defaultValue;
	}
}
