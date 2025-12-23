using AoCLibrary;

namespace Advent20;

internal class Day07 : IRunner
{
	// Day https://adventofcode.com/2020/day/7
	// Input https://adventofcode.com/2020/day/7/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 4L);
		else
			res.Check = new StarCheck(key, 213L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);
		var rv = 0L;
		// magic
		var allBags = lines.Select(l => new Bag7(l)).ToList();

        var checkBags = new Stack<string>();
        checkBags.Push("shiny gold");
        var foundBags = new List<Bag7>();
        while(checkBags.Any())
        {
            var bag = checkBags.Pop();
            var bags = allBags.Where(r => r.Contains(bag)).ToList();
            foreach(var outerBag in bags)
            {
                if (!foundBags.Contains(outerBag))
                {
                    foundBags.Add(outerBag);
                    checkBags.Push(outerBag.Bag);
                }
            }
        }
        rv = foundBags.Count();

        res.CheckGuess(rv);
        return res;
    }
	class Bag7
	{
        public string Bag { get; }
        public Dictionary<string, int> InnerBags { get; } = [];
        public Bag7(string line)
        {
            //light red bags contain 1 bright white bag, 2 muted yellow bags
            var parts = line.Split(" bags contain ");
            Bag = parts[0];
            if (line.Contains("no other bags"))
                return;
            var contains = parts[1].Split(',');
            foreach(var contain in contains)
            {
                var subParts = contain.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                InnerBags[subParts[1] + " " + subParts[2]] = int.Parse(subParts[0]);
            }
        }
        public bool Contains(string bag)
        {
            return InnerBags.ContainsKey(bag);
        }
        public override string ToString()
        {
            return $"{Bag} contains {string.Join(',', InnerBags)}";
        }
        public int GetValue(List<Bag7> allBags)
        {
            var rv = 1;
            foreach (var bag in InnerBags)
            {
                var subBag = allBags.Single(b => b.Bag == bag.Key);
                rv += bag.Value * subBag.GetValue(allBags);
            }
            return rv;
        }
    }
    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 32L);
		else
			res.Check = new StarCheck(key, 38426L);

		var lines = RunHelper.GetLines(key);
		//var text = RunHelper.GetText(key);

		var rv = 0L;
        // magic
        var allBags = lines.Select(l => new Bag7(l)).ToList();

        var bag = allBags.Single(r => r.Bag == "shiny gold");

        rv = bag.GetValue(allBags) - 1;

        res.CheckGuess(rv);
        return res;
	}

}

