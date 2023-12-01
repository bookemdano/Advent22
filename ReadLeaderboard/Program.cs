// See https://aka.ms/new-console-template for more information
using AoCLibrary;
using System.Data;
using System.Diagnostics;

internal class Program : ILogger
{
    private static async Task Main(string[] args)
    {
        var program = new Program();
        await program.Runner();
    }
    async Task Runner()
    { 
        AoCResult? last = null;
		var next = DateTime.MinValue;
        while (true)
        {
            Log("Checking");
			if (DateTime.Now < next)
				continue;
            var res = await Communicator.Read($"https://adventofcode.com/{DateTime.Today.Year}/leaderboard/private/view/1403088.json");
			next = DateTime.Now.AddMinutes(10);

            var aocResult = AoCHelper.Deserialize(res.Json);
			Debug.Assert(aocResult != null);
			File.WriteAllText(@"c:\temp\data\aoc.json", AoCHelper.Serialize(aocResult));
            if (aocResult.HasChanges(last, this))
            {
                var ordered = aocResult.AllMembers().OrderByDescending(m => m.LocalScore);
                var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                int i = 0;
                foreach (var showable in showables)
                    Log($"{++i}. {showable}");
            }
            last = aocResult;
            Thread.Sleep(TimeSpan.FromMinutes(1));
        }
    }


    public void Log(string str)
    {
        Console.WriteLine(DateTime.Now.ToString() + ": " + str);
    }
}



