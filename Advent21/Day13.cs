using AoCLibrary;
namespace Advent21;

internal class Day13 : IRunner
{
	// Day https://adventofcode.com/2021/day/13
	// Input https://adventofcode.com/2021/day/13/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 17L);
		else
			res.Check = new StarCheck(key, 638L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var pts = ParseAndFold(lines, key);
        rv = pts.Count();

        res.CheckGuess(rv);
        return res;
    }
    List<Point> ParseAndFold(IEnumerable<string> lines, StarCheckKey key)
    {
        var beforeBreak = true;
        var pts = new List<Point>();
        var folds = new List<string>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                beforeBreak = false;
                continue;
            }
            if (beforeBreak)
                pts.Add(new Point(line));
            else
                folds.Add(line);
        }
        var maxX = (int)pts.Max(l => l.X) + 1;
        var maxY = (int)pts.Max(l => l.Y) + 1;

        foreach (var fold in folds)
        {
            var crease = int.Parse(fold.Substring(13));

            var newPts = new List<Point>();
            if (fold.Contains("x"))
            {
                foreach(var pt in pts)
                {
                    if (pt.X < crease)
                    {
                        if (!newPts.Contains(pt))
                            newPts.Add(pt);
                    }
                    else if (pt.X > crease)
                    {
                        var diff = pt.X - crease;
                        var newPt = new Point(crease - diff, pt.Y);
                        if (!newPts.Contains(newPt))
                            newPts.Add(newPt);
                    }
                }
            }
            else if (fold.Contains("y"))
            {
                foreach (var pt in pts)
                {
                    if (pt.Y < crease)
                    {
                        if (!newPts.Contains(pt))
                            newPts.Add(pt);
                    }
                    else if (pt.Y > crease)
                    {
                        var diff = pt.Y - crease;
                        var newPt = new Point(pt.X, crease - diff);
                        if (!newPts.Contains(newPt))
                            newPts.Add(newPt);
                    }
                }
            }
            pts = newPts;
            if (key.Star == StarEnum.Star1)
                break;
        }
        return pts;
    }
    static void Map(string str, List<Point> pts)
    {
        var maxX = (int)pts.Max(l => l.X) + 1;
        var maxY = (int)pts.Max(l => l.Y) + 1;
        var map = new GridMapXY(maxX, maxY);
        foreach (var pt in pts)
            map.Set(pt, '#');
        ElfHelper.DayLogPlus(str + "\n" + map);
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, "0");
		else
			res.Check = new StarCheck(key, "CJCKBAPB");

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = "";
        // magic
        var pts = ParseAndFold(lines, key);
        if (key.Star == StarEnum.Star2)
        {
            Map(key.ToString(), pts);
            // you have to manually read the output to get the answer
            if (!isReal)
                rv = "0";
            else
                rv = "CJCKBAPB";
        }

        res.CheckGuess(rv);
        return res;
	}
}

