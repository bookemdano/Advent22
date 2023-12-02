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
            Log("Checking Next-" + next);
			if (DateTime.Now < next)
			{
				Thread.Sleep(TimeSpan.FromMinutes(1));
				continue;
			}
            var res = await Communicator.Read($"https://adventofcode.com/{DateTime.Today.Year}/leaderboard/private/view/1403088.json");
			next = DateTime.Now.AddMinutes(15);

            var aocResult = ElfHelper.Deserialize(res.Json);
			Debug.Assert(aocResult != null);
			File.WriteAllText(@"c:\temp\data\aoc.json", ElfHelper.Serialize(aocResult));
            if (aocResult.HasChanges(last, this))
            {
                var ordered = aocResult.AllMembers().OrderByDescending(m => m.LocalScore);
                var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                int i = 0;
                foreach (var showable in showables)
                    Log($"{++i}. {showable}");
            }
			else
				Log("Data unchanged.");

			last = aocResult;
        }
    }


    public void Log(string str)
    {
        Console.WriteLine(DateTime.Now.ToString() + ": " + str);
    }
}



