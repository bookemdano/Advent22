using System.Net;

namespace AoCLibrary
{
	public class Communicator
    {
		// Inspect->Network->1->Cookie->Sesssion
		static readonly string _cookieString = "53616c7465645f5f671468d8e8f7ebc0b1136dded2a0ed6bd437f0c2532b37f9f95ca0826d0a505585418b3966e4558ced29f0241060f8558eaa98ed8bb93419";

        //static readonly string _cookieString24 = "53616c7465645f5fe2da2f189b1cb93f80a6bae187e26621b99038f319bfca19382c50b221e4323270285083a5ca9e54c7f0e2bd3d5c32d7b440847360346550";
		//static readonly string _cookieString2023 = "53616c7465645f5f4a5c8551a649899923835062e76be6e6079b68e6574d76ad1caeaae367dc68d8d51241b20417f9ce26396c795150a6e4d91eeeedf3d89a03";
		//static string _cookieString2022 = "53616c7465645f5f3b43899863152185d2d59143fe9023d92a6d55ab884b6f6f680eb77a696a29670c5ce9d701f913454f82916b42d7bf759aeb3c84cfceefaa";

		static public async Task<string?> ReadAsync(string url, bool returnError)
		{
			var uri = new Uri(url);
			Utils.MonthLog($"Read({uri})");
			var cookieContainer = new CookieContainer();
			using var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
			using var client = new HttpClient(handler) { BaseAddress = uri };
			cookieContainer.Add(uri, new Cookie("session", _cookieString));
			try
			{
				var rv = await client.GetStringAsync(uri);
				if (rv.StartsWith('{'))
				{
					var cacheDir = Path.Combine(Utils.Dir, "cache");
					Directory.CreateDirectory(cacheDir);
					var filename = Path.Combine(cacheDir, $"url{DateTime.Now:yyyyMMdd HHmmss}.json");
					File.WriteAllText(filename, rv);
				}
				return rv;
			}
			catch (Exception ex)
			{
				if (returnError)
					return ex.ToString();
				else
					return null;
			}
			//cookieContainer.Add(new Cookie("session", _cookieString));
			//client.DefaultRequestHeaders.Add("Set-Cookie", "session=" + cookieString);
			/*client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            */
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
