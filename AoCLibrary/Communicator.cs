using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AoCLibrary
{
    public class Communicator
    {
        static string _cookieString = "53616c7465645f5f3b43899863152185d2d59143fe9023d92a6d55ab884b6f6f680eb77a696a29670c5ce9d701f913454f82916b42d7bf759aeb3c84cfceefaa";
        static public string Dir { get; } = @"c:\temp\data";
        static public async Task<Tuple<string, bool>> Read(string url, bool overrideThrottle = false)
        {
            Directory.CreateDirectory(Dir);
            var filename = Path.Combine(Dir, FileName());
            if (overrideThrottle == false &&  File.Exists(filename))
            {
                return new Tuple<string, bool>(File.ReadAllText(filename), false);
            }
            var uri = new Uri(url);
            var cookieContainer = new CookieContainer();
            using (var handler  = new HttpClientHandler() { CookieContainer = cookieContainer }) 
            {
                using var client = new HttpClient(handler) { BaseAddress = uri };
                {
                    cookieContainer.Add(uri, new Cookie("session", _cookieString));
                    try
                    {
                        var rv =  await client.GetStringAsync(uri);
                        File.WriteAllText(filename, rv);
                        return new Tuple<string, bool>(rv, true);
                    }
                    catch (Exception ex)
                    {
                        return new Tuple<string, bool>(ex.ToString(), true);
                    }
                }
            }
            //cookieContainer.Add(new Cookie("session", _cookieString));
            //client.DefaultRequestHeaders.Add("Set-Cookie", "session=" + cookieString);
            /*client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            */
        }

        static string FileName()
        {
            var now = DateTime.Now;
            int quarter =  15 * (int) (now.Minute / 15.0);
            return $"url{now.ToString("yyyyMMdd HH")}{quarter.ToString("00")}.json";
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
            return str.Substring(startI, endI - startI);
        }
    }
}
