using AoCLibrary;

namespace Advent19;

internal class Program
{
    static async Task Main()
    {
        Utils.AppName = "RUN";
        ElfHelper.OverrideYear(2019);
        ElfHelper.UpdateCurrentDay();
        var runner = RunHelper.GetDayRunner(ElfHelper.DayString);
        if (runner == null)
            ElfHelper.DayLog("No runner found for Day" + ElfHelper.DayString);
        else
        {
            await RunHelper.RunAsync(runner);
        }
    }
}
