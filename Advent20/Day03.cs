using AoCLibrary;
namespace Advent20;

internal class Day03 : IRunner
{
	// Day https://adventofcode.com/2020/day/3
	// Input https://adventofcode.com/2020/day/3/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 7L);
		else
			res.Check = new StarCheck(key, 280L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var map = new GridMap(lines);

		rv = CountTrees(map, 1, 3);

        res.CheckGuess(rv);
        return res;
    }
	int CountTrees(GridMap map, int moveRows, int moveCols)
	{
        var head = new Loc(0, 0);
        var trees = map.FindAll('#');
        int hits = 0;
        while (head.Row < map.Rows)
        {
            head = head.Move(moveRows, moveCols);
            if (head.Col >= map.Cols)
                head = new Loc(head.Row, head.Col % map.Cols);
            if (trees.Contains(head))
                hits++;
        }
        return hits;
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 336L);
		else
			res.Check = new StarCheck(key, 4355551200L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var map = new GridMap(lines);
        long treeProduct = 1;
        treeProduct *= CountTrees(map, 1, 1);
        treeProduct *= CountTrees(map, 1, 3);
        treeProduct *= CountTrees(map, 1, 5);
        treeProduct *= CountTrees(map, 1, 7);
        treeProduct *= CountTrees(map, 2, 1);
        rv = treeProduct;

        res.CheckGuess(rv);
        return res;
	}
}

