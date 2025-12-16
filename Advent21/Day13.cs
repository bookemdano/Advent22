using AoCLibrary;
using System.IO.MemoryMappedFiles;
using System.Linq;
namespace Advent21;
// #working
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
		rv = ParseAndFold(lines, key);

        res.CheckGuess(rv);
        return res;
    }
    int ParseAndFold(IEnumerable<string> lines, StarCheckKey key)
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
                        newPts.Add(pt);
                    }
                    else if (pt.X > crease)
                    {
                        var diff = pt.X - crease;
                        newPts.Add(new Point(pt.X, crease - diff));
                    }
                }
                for (int x = 0; x < crease; x++)
                {
                    var targetX = maxX - x - 1;
                    for (int y = 0; y < maxY; y++)
                    {
                        var pt = new Point(x, y);
                        if (pts.Contains(pt))
                            newPts.Add(pt);
                        else
                        {
                            var ptTarget = new Point(targetX, y);
                            if (pts.Contains(ptTarget))
                                newPts.Add(pt);
                        }
                    }
                }
            }
            else if (fold.Contains("y"))
            {
                for (int y = 0; y < crease; y++)
                {
                    var targetY = maxY - y - 1;
                    for (int x = 0; x < maxX; x++)
                    {
                        var pt = new Point(x, y);
                        if (pts.Contains(pt))
                            newPts.Add(pt);
                        else
                        {
                            var ptTarget = new Point(x, targetY);
                            if (pts.Contains(ptTarget))
                                newPts.Add(pt);
                        }
                    }
                }
            }
            pts = newPts;
            //ElfHelper.DayLogPlus("Fold1\n" + GridMap13.Map(pts));
            if (key.Star == StarEnum.Star1)
                return pts.Count();
        }
        if (key.Star == StarEnum.Star2)
        {
            Map(key.ToString(), pts);
            if (key.IsReal == false)
                return 0;
            
        }
        return pts.Count();
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
			res.Check = new StarCheck(key, 0L);
		else
			res.Check = new StarCheck(key, -1L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        rv = ParseAndFold(lines, key);

        res.CheckGuess(rv);
        return res;
	}
}

