using AoCLibrary;
using System.Diagnostics;
using System.Reflection;

namespace Advent25;

internal class Program
{
    static async Task Main()
    {
        Utils.AppName = "RUN";

        //var runner = GetDayRunner("14");
        var runner = GetDayRunner(ElfHelper.DayString);
        if (runner == null)
            ElfHelper.DayLog("No runner found for Day" + ElfHelper.DayString);
        else
        {
            await RunAsync(runner);
        }
    }
    static async Task RunAsync(IDayRunner runner)
    {
        ElfHelper.ResetDayLog();

        ElfHelper.MonthLogPlus($"Run() {runner.GetType().Name}");

        var res = await RunIt(runner, false, StarEnum.Star1);
        if (res.StarSuccess == true)
            res = await RunIt(runner, true, StarEnum.Star1);
        
        if (res.StarSuccess == true)
            res = await RunIt(runner, false, StarEnum.Star2);
        if (res.StarSuccess == true)
            await RunIt(runner, true, StarEnum.Star2);
    }

    private static async Task<RunnerResult> RunIt(IDayRunner runner, bool isReal, StarEnum star)
    {
        // get input file if we don't have it yet.
        if (isReal && !IsFileThere(InputFile(isReal, StarEnum.NA)))
        {
            var str = await ElfHelper.WriteInputFileAsync(ElfHelper.Day);
            var filename = InputFile(isReal, StarEnum.NA);
            File.WriteAllText(filename, str);
        }

        RunnerResult res;

        var sw = Stopwatch.StartNew();
        if (star == StarEnum.Star1)
            res = runner.Star1(isReal);
        else
            res = runner.Star2(isReal);
        res.Ts = sw.Elapsed;
        ElfHelper.DayLog($"r1:{res}");
        return res;
    }

    static IDayRunner? GetDayRunner(string dayString)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var advent = assemblies[1];
        if (advent?.FullName?.Contains($"Advent{ElfHelper.Year2}") != true)
        {
            ElfHelper.DayLog("No assembly[1] found!");
            return null;
        }
        var className = $"Advent{ElfHelper.Year2}.Day{dayString}";
        var dayClass = advent.GetType(className);
        if (dayClass == null)
        {
            ElfHelper.DayLog($"No class {className} found in {advent}");
            //await ElfHelper.WriteStubFiles(ElfHelper.Day, false);
            return null;
        }
        var o = Activator.CreateInstance(dayClass);
        if (o is IDayRunner rv)
            return rv;
        return null;
    }

    static bool IsFileThere(string file)
    {
        if (!File.Exists(file))
            return false;
        var info = new FileInfo(file);
        return (info.Length > 0);
    }
    static internal string InputFile(bool real, StarEnum star, int? part = null)
    {
        if (real)
            return Path.Combine("Assets", $"Day{ElfHelper.DayString}.txt");


        var partString = string.Empty;
        if (part != null)
            partString = $"Part{part + 1}";

        string filename = Path.Combine("Assets", $"Day{ElfHelper.DayString}Fake{partString}.txt");
        if (!IsFileThere(filename))
        {
            filename = Path.Combine("Assets", $"Day{ElfHelper.DayString}Fake{star}{partString}.txt");
            if (!IsFileThere(filename)) // try start 1
                filename = Path.Combine("Assets", $"Day{ElfHelper.DayString}Fake{StarEnum.Star1}{partString}.txt");
        }
        return filename;
    }
    static Dictionary<string, string[]> _dictLines = [];
    static internal string[] GetLines(StarCheckKey key, bool raw = false)
    {
        return GetLines(key.Star, key.IsReal, key.Part);
    }
    static internal string[] GetLines(StarEnum star, bool real, int? part = null, bool raw = false)
    {
        var filename = InputFile(real, star, part);
        if (!_dictLines.ContainsKey(filename))
        {
            ElfHelper.MonthLogPlus("ReadLines- " + filename);
            var lines = File.ReadAllLines(filename);
            if (!raw)
                _dictLines[filename] = lines.ToArray();
            else
                _dictLines[filename] = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
        }
        return _dictLines[filename];
    }
    static Dictionary<string, string> _dictText = [];
    static internal string GetText(StarCheckKey key)
    {
        return GetText(key.Star, key.IsReal, key.Part);
    }
    static internal string GetText(StarEnum star, bool real, int? part)
    {
        var filename = InputFile(real, star, part);
        if (!_dictText.ContainsKey(filename))
        {
            ElfHelper.MonthLogPlus("ReadText- " + filename);
            _dictText[filename] = File.ReadAllText(filename);
        }
        return _dictText[filename];
    }
}
