using System.Net;

namespace AoCLibrary
{
	public class Communicator
	{
		static readonly string _cookieString = "53616c7465645f5f4a5c8551a649899923835062e76be6e6079b68e6574d76ad1caeaae367dc68d8d51241b20417f9ce26396c795150a6e4d91eeeedf3d89a03";
		//static string _cookieString2022 = "53616c7465645f5f3b43899863152185d2d59143fe9023d92a6d55ab884b6f6f680eb77a696a29670c5ce9d701f913454f82916b42d7bf759aeb3c84cfceefaa";
        static public string Dir { get; } = @"c:\temp\data\elf";
		static Communicator()
		{
			Directory.CreateDirectory(Dir);
		}
        static public async Task<string> Read(string url)
        {
            var uri = new Uri(url);
			ElfHelper.Log($"Read({uri})");
            var cookieContainer = new CookieContainer();
			using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
			using var client = new HttpClient(handler) { BaseAddress = uri };
			cookieContainer.Add(uri, new Cookie("session", _cookieString));
			try
			{
				var rv = await client.GetStringAsync(uri);
				if (rv.StartsWith('{'))
				{
					var cacheDir = Path.Combine(Dir, "cache");
					Directory.CreateDirectory(Dir);
					var filename = Path.Combine(cacheDir, $"url{DateTime.Now:yyyyMMdd HHmmss}.json");
					File.WriteAllText(filename, rv);
				}
				return rv;
			}
			catch (Exception ex)
			{
				return ex.ToString();
			}
			//cookieContainer.Add(new Cookie("session", _cookieString));
			//client.DefaultRequestHeaders.Add("Set-Cookie", "session=" + cookieString);
			/*client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            */
		}

        static public DateTime TimeFromFile(string filename)
        {
            var str = StringBetween(filename, "url", ".json");
            return DateTime.ParseExact(str, "yyyyMMdd HHmm", null);
        }
        static string StringBetween(string str, string start, string end)
        {
            var startI = str.IndexOf(start);
            startI += start.Length;
            var endI = str.IndexOf(end, startI);
			return str[startI..endI];
        }
    }
}
