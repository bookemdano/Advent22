using AoCLibrary;

namespace Advent25;

internal class Day12 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/12
	// Input https://adventofcode.com/2025/day/12/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 2L);
		else
			res.Check = new StarCheck(key, 499L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
		var shapes = new List<Shape12>();
		var areas = new List<Area12>();
		for(int i = 0; i < lines.Length; i++)
		{
			var line = lines[i];
			if (line.Contains('x'))
			{
                //grid
                areas.Add(new Area12(line));
			}
			else if (line.Contains(':'))
			{
				shapes.Add(new Shape12(new GridMapXY(lines[(i+1)..(i+4)])));
				i += 4;
			}
		}

        Area12.Shapes = shapes;
		foreach (var area in areas)
		{
            ElfHelper.DayLogPlus("Area " + area);
            if (area.Fit(0))
            {
                ElfHelper.DayLogPlus("Fit!");
                rv++;
            }
		}
        // 1000 too high
        res.CheckGuess(rv);
        return res;
    }
    class Shape12
    {
        public List<GridMapXY> SubShapes { get; } = [];
        public Shape12(GridMapXY shape)
        {
            var newShape = new GridMapXY(shape);
            for (int iRotate = 0; iRotate < 4; iRotate++)
            {
                if (!SubShapes.Contains(newShape))
                    SubShapes.Add(newShape);
                var test = newShape.FlipVert();
                if (!SubShapes.Contains(test))
                    SubShapes.Add(test);
                test = newShape.FlipHorz();
                if (!SubShapes.Contains(test))
                    SubShapes.Add(test);
                newShape = newShape.Rotate();
            }
        }

        public int Cols => SubShapes[0].Cols;
        public int Rows => SubShapes[0].Rows;
    }

	class Area12
	{
        public Area12(string line)
        {
			var parts = line.Split(": ");
			var size = parts[0].Split('x');
            _grid = new GridMapXY(int.Parse(size[0]), int.Parse(size[1]));
			var shapes = parts[1].Split(' ');
            _shapeCounts = shapes.Select(s => int.Parse(s)).ToArray();
        }

        public Area12(Area12 area)
        {
            _grid = new GridMapXY(area._grid);
            _shapeCounts = area._shapeCounts.ToArray();
        }

        GridMapXY _grid;
		int[] _shapeCounts;
        public override string ToString()
        {
			return $"({_grid.Rows},{_grid.Cols}) {string.Join(',', _shapeCounts)}";
        }
        static public List<Shape12> Shapes = [];

        internal bool TryHere(GridMapXY shape, int targetX, int targetY)
        {
            for (int x = 0; x < shape.Cols; x++)
            {
                for (int y = 0; y < shape.Rows; y++)
                {
                    if (shape.DirectGet(x, y) == '.')
                        continue;
                    if (shape.DirectGet(x, y) == '#' && _grid.DirectGet(x + targetX, y + targetY) == '#')
                        return false;
                }
            }
            return true;

        }
        internal bool Fit(int iLevel)
        {
            if (iLevel < 2)
            {
                ElfHelper.DayLogPlus($"{this} l:{iLevel}");
            }
            var newAreas = new List<Area12>();
            var rv = 0L;
			for(int iShape = 0; iShape < _shapeCounts.Count(); iShape++)
			{
				var count = _shapeCounts[iShape];
                if (count > 0)
                {
                    bool foundOne = false;
                    var shape = Shapes[iShape];
                    for (int x = 0; x <= _grid.Cols - shape.Cols; x++)
                    {
                        for (int y = 0; y <= _grid.Rows - shape.Rows; y++)
                        {
                            foreach(var subShape in shape.SubShapes)
                            {
                                if (TryHere(subShape, x, y))
                                {
                                    foundOne = true;
                                    var newArea = Area12.Update(this, iShape, subShape, x, y);
                                    if (newArea.ShapesLeft == 0)
                                        return true;
                                    else if (newArea.Fit(iLevel++))
                                        return true;
                                }
                            }
                        }
                    }
                    if (foundOne == false)
                    {
                        //ElfHelper.DayLogPlus(_grid);
                        break;
                    }
                }
			}
            return false;
        }
        int ShapesLeft => _shapeCounts.Sum();
        private static Area12 Update(Area12 area, int iShape, GridMapXY subShape, int targetX, int targetY)
        {
            var rv = new Area12(area);
            rv._grid.Union(subShape, targetX, targetY);
            rv._shapeCounts[iShape]--;
            return rv;

        }

        internal bool EasyFit()
        {
            int amnt = (_grid.Cols/ 3) * (_grid.Rows / 3);
            return (amnt >= ShapesLeft);
        }
        /*
public long Process(Path11 path)
{
if (path.Head == "out")
{
if (path.VistedEnough)
  return 1;
else
  return 0;
}
if (_cache.ContainsKey(path))
return _cache[path];

var conn = conns.Single(c => c.Source == path.Head);
var sum = 0L;
foreach (var connOut in conn.Outs)
{
var newPath = new Path11(path);
newPath.SetHead(connOut);
sum += Process(newPath);
}
_cache[path] = sum;

return sum;
}*/
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 0L);
		else
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
		// magic

        res.CheckGuess(rv);
        return res;
	}
}

