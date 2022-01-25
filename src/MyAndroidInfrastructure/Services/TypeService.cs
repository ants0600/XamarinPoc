/// <summary>
/// todo: static members only, no need to mock up anything here
/// </summary>
public static class TypeService
{
	public static double ParseDouble(string text, double defaultValue)
	{
		text = $"{text}";
		return double.TryParse(text, out double value) ? value : defaultValue;
	}

	public static double ParseDouble(string text)
	{
		text = $"{text}";
		return double.TryParse(text, out double value) ? value : default;
	}
	public static int ParseInteger(string text, int defaultValue)
	{
		text = $"{text}";
		return int.TryParse(text, out int value) ? value : defaultValue;
	}

	public static int ParseInteger(string text)
	{
		text = $"{text}";
		return int.TryParse(text, out int value) ? value : default;
	}

	public static long ParseLong(string text, long defaultValue)
	{
		text = $"{text}";
		return long.TryParse(text, out long value) ? value : defaultValue;
	}

	public static long ParseLong(string text)
	{
		text = $"{text}";
		return long.TryParse(text, out long value) ? value : default;
	}

}