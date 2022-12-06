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
        var last = AoCHelper.Export(this);
        var first = true;
        while (true)
        {
            Log("Checking");
            var result = await Communicator.Read("https://adventofcode.com/2022/leaderboard/private/view/1403088.json");
            var json = result.Item1;
            var realRead = result.Item2;
            if (!realRead && !first)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                continue;
            }
            if (realRead)
            {
                AoCHelper.Export(this);
                Log("Real read!");
            }
            //Log(json);
            var aocResult = AoCHelper.Deserialize(json);

            if (aocResult.HasChanges(last, this) || realRead == true || first == true)
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



