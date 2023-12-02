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
        ElfResult? last = null;
		var next = DateTime.MinValue;
        while (true)
        {
            Log("Checking Next-" + next);
			if (DateTime.Now < next)
			{
				Thread.Sleep(TimeSpan.FromMinutes(1));
				continue;
			}
			var elfResult = await ElfHelper.Read(false);
			Debug.Assert(elfResult != null);
			//var res = await Communicator.Read($"https://adventofcode.com/{DateTime.Today.Year}/leaderboard/private/view/1403088.json");
			next = elfResult.Timestamp.AddMinutes(15);
			Log("Read " + elfResult.Timestamp);
            if (elfResult.HasChanges(last, this))
            {
                var ordered = elfResult.AllMembers().OrderByDescending(m => m.LocalScore);
                var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                int i = 0;
                foreach (var showable in showables)
                    Log($"{++i}. {showable}");
            }
			else
				Log("Data unchanged.");

			last = elfResult;
        }
    }
    public void Log(string str)
    {
		ElfHelper.Log(str);
    }
}



