// See https://aka.ms/new-console-template for more information
using AoCLibrary;
using System.Data;
using System.Text.Json;

internal class Program : ILogger
{
    private static async Task Main(string[] args)
    {
        var program = new Program();
        await program.Runner();
    }
    async Task Runner()
    { 
        var last = AoCHelper.OldExport(this);
        var first = true;
        while (true)
        {
            Log("Checking");
            var res = await Communicator.Read($"https://adventofcode.com/{DateTime.Today.Year}/leaderboard/private/view/1403088.json");
            if (!res.RealRead && !first)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                continue;
            }
            if (res.RealRead)
            {
                AoCHelper.OldExport(this);
                Log("Real read!");
            }
            //Log(json);
            var aocResult = AoCHelper.Deserialize(res.Json);

            if (aocResult.HasChanges(last, this) || res.RealRead == true || first == true)
            {
                //var showables = aocResult.AllMembers.OrderByDescending(m => m.LocalScore).Take(10).ToArray();
                var ordered = aocResult.AllMembers().OrderByDescending(m => m.LocalScore);
                var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                int i = 0;
                foreach (var showable in showables)
                    Log($"{++i}. {showable}");
            }
            first = false;
            last = aocResult;
            Thread.Sleep(TimeSpan.FromMinutes(1));
        }
    }


    public void Log(string str)
    {
        Console.WriteLine(DateTime.Now.ToString() + ": " + str);
    }
}



