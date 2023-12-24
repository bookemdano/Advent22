using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace AoCLibrary
{
	static public class Utils
	{
        static public bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		static public string Dir { get; }
		static Utils()
		{
			if (IsWindows)
				Dir = @"c:\temp\data\elf";
			else
	            Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "data", "elf");
    
            Directory.CreateDirectory(Dir);
		}

		static public string AppName { get; set; } = string.Empty;
		public static void MonthLog(object o, Stopwatch? sw = null)
		{
			var str = $"{AppName} {o}";
			if (sw != null)
				str += $" {sw.Elapsed.TotalMilliseconds:0.0}ms";
			str = $"{DateTime.Now} {str}";
			File.AppendAllText(Path.Combine(Dir, $"elf{DateTime.Today:yyyyMM}.log"), str + Environment.NewLine);
			Console.WriteLine(str);
		}
		public static void CaptainsLog(object o)
		{
			var str = $"{DateTime.Now} {AppName} {o}";
			File.AppendAllText(Path.Combine(Dir, $"elf{DateTime.Today:yyyy}.log"), $"{DateTime.Now} {AppName} {o}" + Environment.NewLine);
			//Console.WriteLine(str);
		}

		static public string CompactNumber(int n, int maxLen)
		{
			const string poss = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ€$";
			var len = poss.Length;

            var rv = string.Empty;
            while (rv.Length < maxLen)
			{
				if (n > 0)
				{
                    var mod = n % len;
					rv = poss[mod] + rv;
                }
                else
					rv = "0" + rv;
                n = n / len;
            }
			return rv;
        }
        static public string CompactName(int n)
        {
            const string poss = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var len = poss.Length;

			var rv = string.Empty;
            while (n > 0 || rv.Length == 0)
            {
                var mod = n % len;
                rv = poss[mod] + rv;
                n = n / len;
            }
            return rv;
        }
        static public void Assert(long l1, long l2)
        {
			Assert(l1 == l2, $"{l1} != {l2}");
        }
        static public void Assert(bool b, string str)
        {
            if (b == false)
                MonthLog("ASSERT FAILED " + str);
        }
        static public string TimeString(DateTime dt)
		{
			if (DateTime.Today == dt.Date)
				return dt.ToString("HH:mm");
			else
				return dt.ToString("M/d HH:mm");
		}
		public static string Serialize<T>(T result)
		{
			return JsonSerializer.Serialize(result, _jsonOptions);
		}
		public static T? Deserialize<T>(string json)
		{
			return JsonSerializer.Deserialize<T>(json, _jsonOptions);
		}
		static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

		static public void Open(string filename)
		{
			try
			{
				var psi = new ProcessStartInfo
				{
					UseShellExecute = true,
					FileName = filename
				};
				Process.Start(psi);

			}
			catch (Exception ex)
			{
				MonthLog($"Open({filename}) " + ex);
			}
		}

		static public string Fraction(double origD, int denom)
		{
			var rv = "";
			var d = origD;
			if (d >= 1)
			{
				rv = ((int)d).ToString();
				d -= (int)d;
			}

			if (d == 0)
				return rv;
			else if (d <= 1 / 8.0 && denom > 8)
				return rv + "⅛";
			else if (d <= 1 / 4.0 && denom >= 4)
				return rv + "¼";
			else if (d <= 1 / 3.0 && denom >= 6)
				return rv + "⅓";
			else if (d <= 3 / 8.0 && denom >= 8)
				return rv + "⅜";
			else if (d <= 1 / 2.0 && denom >= 2)
				return rv + "½";
			else if (d <= 5 / 8.0 && denom >= 8)
				return rv + "⅝";
			else if (d <= 2 / 3.0 && denom >= 6)
				return rv + "⅔";
			else if (d <= 3 / 4.0 && denom >= 4)
				return rv + "¾";
			else if (d <= 7 / 8.0 && denom >= 8)
				return rv + "⅞";
			else
				return ((int)origD + 1).ToString(); ;
		}
		static internal string DeltaString(TimeSpan delta)
		{
			int n;
			string unit;
			var years = delta.TotalDays / 365.24;
			if (years > 5)
			{
				n = (int)years;
				unit = "year";
			}
			else if (years > 2)
			{
				return $"{Fraction(years, 2)} years";
			}
			else if (delta.TotalDays > 1)
			{
				unit = "day";
				n = (int)delta.TotalDays;
			}
			else if (delta.TotalHours > 1.5)
			{
				unit = "hour";
				n = (int)delta.TotalHours;
			}
			else if (delta.TotalMinutes > 1.5)
			{
				unit = "min";
				n = (int)delta.TotalMinutes;
			}
			else
			{
				unit = "sec";
				n = (int)delta.TotalSeconds;
			}
			if (n != 1)
				unit += "s";
			return $"{n} {unit}";
		}
		static public DateTime GetTime(int ts)
		{
			return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(ts)).AddHours(-5);
		}

        public static string RemoveLabel(string line)
        {
            return line.Split(':', StringSplitOptions.TrimEntries)[1];
        }

        public static string[] Split(char sep, string line)
        {
            return line.Split(sep, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

		public static long[] SplitLongs(char sep, string line)
		{
			return Split(sep, line).Select(s => long.Parse(s)).ToArray();
		}
		public static int[] SplitInts(char sep, string line)
		{
			return Split(sep, line).Select(s => int.Parse(s)).ToArray();
		}
	}
}
