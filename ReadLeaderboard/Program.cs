// See https://aka.ms/new-console-template for more information
using ReadLeaderboard;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class Program
{
    private static async Task Main(string[] args)
    {
        await RunThrough();

        async Task RunThrough()
        {
            var last = ReadAll();
            var first = true;
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            while (true)
            {
                Console.WriteLine("Checking at " + DateTime.Now);
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
                    ReadAll();
                    Console.WriteLine("Real read! " + DateTime.Now);
                }
                //Console.WriteLine(json);
                var aocResult = JsonSerializer.Deserialize<AoCResult>(json, options);

                if (aocResult.Compare(last) || realRead == true || first == true)
                {
                    //var showables = aocResult.AllMembers.OrderByDescending(m => m.LocalScore).Take(10).ToArray();
                    var ordered = aocResult.AllMembers.OrderByDescending(m => m.LocalScore);
                    var showables = ordered.Where(m => m.LocalScore > 0).ToArray();
                    int i = 0;
                    foreach (var showable in showables)
                        Console.WriteLine($"{++i}. {showable}");
                }
                first = false;
                last = aocResult;
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        // returns last
        AoCResult ReadAll()
        {
            if (!Directory.Exists(Communicator.Dir))
                return null;
            var files = Directory.GetFiles(Communicator.Dir, "url*.json").Order();
            if (!files.Any())
                return null;
            var outs = new List<string>();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

            AoCResult aocResult = null;
            AoCResult prevResult = null;

            var json = File.ReadAllText(files.Last());
            var finalResult = JsonSerializer.Deserialize<AoCResult>(json, options);
            var lastLine = "";
            foreach (var file in files)
            {
                json = File.ReadAllText(file);
                aocResult = JsonSerializer.Deserialize<AoCResult>(json, options);
                if (!outs.Any())
                {
                    var headerParts = new List<string>();
                    headerParts.Add("Date");
                    foreach (var member in aocResult.AllMembers)
                    {
                        if (finalResult.FindbyName(member.Name).LocalScore > 0)
                            headerParts.Add(member.Name);
                    }
                    outs.Add(string.Join(",", headerParts));
                }

                var parts = new List<string>();
                foreach (var member in aocResult.AllMembers)
                {
                    if (finalResult.FindbyName(member.Name).LocalScore > 0)
                    {
                        //parts.Add(member.LocalScore.ToString());
                        var prev = prevResult?.FindbyName(member.Name);
                        if (prev == null)
                            parts.Add(member.LocalScore.ToString());
                        else
                            parts.Add((member.LocalScore - prev.LocalScore).ToString());
                    }
                }
                prevResult = aocResult;
                var line = string.Join(",", parts);
                if (line != lastLine)
                {

                    parts.Insert(0, Communicator.TimeFromFile(file).ToString());
                    outs.Add(string.Join(",", parts));
                }
                lastLine = line;

            }
            try
            {
                File.WriteAllLines(Path.Combine(Communicator.Dir, "scores.csv"), outs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error " + ex.Message);
            }
            return aocResult;
        }
    }
}

public class AoCResult
{
    public Member FindbyName(string name)
    {
        return AllMembers.First(m => m.Name == name);
    }
    public MemberGroup Members { get; set; }
    public Member[] AllMembers
    {
        get
        {
            return Members.AllMembers;
        }
    }
    internal bool Compare(AoCResult last)
    {
        if (last == null)
            return false;
        bool rv = false;
        var allMembers = AllMembers;
        var lastAllMembers = last.AllMembers;
        for (int i = 0; i < allMembers.Length; i++)
        {
            var m = allMembers[i];
            var lastM = lastAllMembers[i];
            if (m.LocalScore != lastM.LocalScore)
            {
                Console.WriteLine($"{m.Name} Gained {m.LocalScore - lastM.LocalScore}!");
                rv = true;
            }
        }
        return rv;
    }
}
public class MemberGroup
{
    public Member[] AllMembers
    {
        get
        {
            return new Member[] {
                Member00, Member01, Member02, Member03, Member04, Member05, Member06, Member07, Member08, Member09,
                Member10, Member11, Member12, Member13, Member14, Member15, Member16, Member17, Member18, Member19,
                Member20, Member21};
        }
    }

    [JsonPropertyName("2428221")]
    public Member Member00 { get; set; }
    [JsonPropertyName("2497986")]
    public Member Member01 { get; set; }
    [JsonPropertyName("2439451")]
    public Member Member02 { get; set; }    // Dan
    [JsonPropertyName("2439454")]
    public Member Member03 { get; set; }
    [JsonPropertyName("1403088")]
    public Member Member04 { get; set; }
    [JsonPropertyName("2481606")]
    public Member Member05 { get; set; }
    [JsonPropertyName("1555842")]
    public Member Member06 { get; set; }
    [JsonPropertyName("2298841")]
    public Member Member07 { get; set; }
    [JsonPropertyName("1839258")]
    public Member Member08 { get; set; }
    [JsonPropertyName("1539979")]
    public Member Member09 { get; set; }
    [JsonPropertyName("678584")]
    public Member Member10 { get; set; }    // Ali
    [JsonPropertyName("1659760")]
    public Member Member11 { get; set; }
    [JsonPropertyName("1861750")]
    public Member Member12 { get; set; }
    [JsonPropertyName("1650120")]
    public Member Member13 { get; set; }
    [JsonPropertyName("1538218")]
    public Member Member14 { get; set; }    // Derrick
    [JsonPropertyName("2454059")]
    public Member Member15 { get; set; }
    [JsonPropertyName("1538420")]
    public Member Member16 { get; set; }
    [JsonPropertyName("1646913")]
    public Member Member17 { get; set; }
    [JsonPropertyName("2452095")]
    public Member Member18 { get; set; }
    [JsonPropertyName("1562200")]
    public Member Member19 { get; set; }
    [JsonPropertyName("850621")]
    public Member Member20 { get; set; }
    [JsonPropertyName("2500712")]
    public Member Member21 { get; set; }
}

public class Member
{
    public string Name { get; set; }
    public int Stars { get; set; }

    [JsonPropertyName("Local_Score")]
    public int LocalScore { get; set; }
    [JsonPropertyName("Global_Score")]
    public int GlobalScore { get; set; }
    [JsonPropertyName("last_star_ts")]
    public int LastStarTs { get; set; }

    public DateTime LastTime
    {
        get
        {
            return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToDouble(LastStarTs)).AddHours(-5);
        }
    }
    public override string ToString()
    {
        var avg = "-";
        if (Stars > 0)
            avg = ((double) LocalScore / (Stars / 2.0)).ToString("0.0");
        var timeString = LastTime.ToString("M/d HH:mm");
        if (DateTime.Today == LastTime.Date)
            timeString = LastTime.ToString("HH:mm");
        var days = (DateTime.Today - new DateTime(2022, 11, 30)).TotalDays;
        if (days > 25) 
            days = 25;
        var starString = Stars.ToString();
        if (Stars == days * 2)
            starString = "*";   //⭐";

        return $"{starString} {Name} {LocalScore} {avg} {timeString}";
    }
}