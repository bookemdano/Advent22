using AoCLibrary;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Advent25;

internal class Day09 : IDayRunner
{
    // Day https://adventofcode.com/2025/day/9
    // Input https://adventofcode.com/2025/day/9/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 50L);
        else
            res.Check = new StarCheck(key, 4735268538L);

        var lines = Program.GetLines(key);
        //var text = Program.GetText(key);
        var rv = 0L;
        // magic
        var pts = Point.ReadAll(lines);
        for (var i = 0; i < pts.Count; i++)
        {
            for (var j = 0; j < pts.Count; j++)
            {
                if (i == j)
                    continue;
                var area = Area(pts[i], pts[j]);
                if (area > rv)
                    rv = area;
            }
        }
        res.CheckGuess(rv);
        return res;
    }
    long Area(Point pt1, Point pt2)
    {
        return ((Math.Abs(pt1.X - pt2.X) + 1) * (Math.Abs(pt1.Y - pt2.Y) + 1));
    }
   
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
            res.Check = new StarCheck(key, 24L);
        else
            res.Check = new StarCheck(key, 1537458069L);

        var lines = Program.GetLines(key);
        //var text = Program.GetText(key);

        var rv = 0L;
        // magic
        var pts = Point.ReadAll(lines);

        for (var i = 0; i < pts.Count; i++)
        {
            ElfHelper.DayLogPlus($"{i} of {pts.Count}");
            for (var j = i+1; j < pts.Count; j++)
            {
                var area = Area(pts[i], pts[j]);
                if (area > rv)
                {
                    if (IsInside(pts[i], pts[j], pts))
                    {
                        ElfHelper.DayLogPlus($"New biggest area {pts[i]} to {pts[j]} of {area}");
                        rv = area;
                    }
                }
            }
        }
        // too high 4595056840
        res.CheckGuess(rv);
        return res;
    }

    private bool IsInside(Point pt1, Point pt2, List<Point> edges)
    {
        var ptMin = new Point(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y));
        var ptMax = new Point(Math.Max(pt1.X, pt2.X), Math.Max(pt1.Y, pt2.Y));
        if (!Between(ptMin, edges))
            return false;
        if (!Between(ptMax, edges))
            return false;
        if (!Between(new Point(ptMax.X, ptMin.Y), edges))
            return false;
        if (!Between(new Point(ptMin.X, ptMax.Y), edges))
            return false;

        for (var x = ptMin.X; x <= ptMax.X; x++)
        {
            if (!Between(new Point(x, ptMin.Y), edges))
                return false;
            if (!Between(new Point(x, ptMax.Y), edges))
                return false;
        }
        for(var y = ptMin.Y; y <= ptMax.Y; y++)
        {
            if (!Between(new Point(ptMin.X, y), edges))
                return false;
            if (!Between(new Point(ptMax.X, y), edges))
                return false;
        }
        return true;
    }

    private bool Between(Point pt, List<Point> corners)
    {
        bool inside = false;
        var last = corners[corners.Count - 1];
        foreach (var corner in corners)
        {
            if (IsPointOnSegment(pt, corner, last))
                return true;
            bool intersect = ((corner.Y > pt.Y) != (last.Y > pt.Y)) &&
                             (pt.X < (last.X - corner.X) * (pt.Y - corner.Y) / (double)(last.Y - corner.Y) + corner.X);
            if (intersect)
                inside = !inside;
            last = corner;
        }
        
        return inside;
    }
  
    private static bool IsPointOnSegment(Point pt, Point pt1, Point pt2)
    {
        // Cross product = 0 → collinear
        if (pt.X == pt1.X && pt1.X == pt2.X)
        {
            if (pt.Y >= Math.Min(pt1.Y, pt2.Y) && pt.Y <= Math.Max(pt1.Y, pt2.Y))
                return true;
        }
        else if (pt.Y == pt1.Y && pt1.Y == pt2.Y)
        {
            if (pt.X >= Math.Min(pt1.X, pt2.X) && pt.X <= Math.Max(pt1.X, pt2.X))
                return true;
        }
        return false;
    }
}

