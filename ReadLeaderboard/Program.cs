using AoCLibrary;
using System.Data;
using System.Diagnostics;

internal class Program
{
    private static async Task Main(string[] args)
	{
		Utils.AppName = "LC";
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
			next = elfResult.Timestamp.AddMinutes(15);
			Log("Read " + elfResult.Timestamp);
			var changes = elfResult.HasChanges(last);
			if (last == null || changes.Any())
            {
                var ordered = elfResult.AllMembers(true).OrderByDescending(m => m.LocalScore);
                var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                int i = 0;
                foreach (var showable in showables)
                    Log($"{++i}. {showable} {showable.Places()}");
				foreach (var change in changes)
					Log(change);
            }
			else
				Log("Data unchanged.");

			last = elfResult;
        }
    }
    void Log(string str)
    {
        Utils.Log(str);
    }
}



