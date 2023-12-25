using AoCLibrary;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Advent23
{
    internal class Day25 : IDayRunner
	{
		public bool IsReal => false;

		// Day https://adventofcode.com/2023/day/25
		// Input https://adventofcode.com/2023/day/25/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 54);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic
            /*
            var strs = new List<string>();
            var graph = new Graph25();
            graph.Edges.Add(new Edge25("0", "1"));
            graph.Edges.Add(new Edge25("0", "2"));
            graph.Edges.Add(new Edge25("0", "3"));
            graph.Edges.Add(new Edge25("1", "3"));
            graph.Edges.Add(new Edge25("2", "3"));
            var min = int.MaxValue;
            for (int i = 0; i < 20; i++)
            {
                var val = graph.KargerMinCut();
                ElfHelper.DayLog("Val = " + val);
                if (val < min)
                {
                    min = val;
                }
            }*/


            var graph = new Graph25();
            foreach (var line in lines)
			{
                var parts = Utils.Split(':', line);
                var name = parts[0];
                var connections = Utils.Split(' ', parts[1]);
                foreach(var conn in connections)
                    graph.Edges.Add(new Edge25(name, conn));
			}
            graph.GroupSizes(new List<Edge25>());

            var minCuts = new List<Edge25>();
            while(true)
            {
                var cuts = graph.KargerMinCut();
                ElfHelper.DayLog("Val = " + string.Join(',', cuts));
                if (cuts.Count() < minCuts.Count() || !minCuts.Any())
                {
                    minCuts = cuts.ToList();
                    if (minCuts.Count() == 3)
                        break;
                }

            }
            var sizes = graph.GroupSizes(minCuts);
            rv = 1;
            foreach (var size in sizes)
                rv *= size;
			/*
            foreach(var comp in comps)
			{
				var contains = comps.Where(c => c.Connections.Contains(comp.Name)).Select(c => c.Name);
				comp.ExtraConnections.AddRange(contains);
			}
			for (var i = 0; i < comps.Count(); i++)
			{
				
				for (var j = 0; j < comps.Count(); j++)
				{
					if (i == j)
						continue;
				}
			}*/
            check.Compare(rv);
			return rv;
		}
		public object? Star2()
		{
			var key = new StarCheckKey(StarEnum.Star2, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 0L);
			else
				check = new StarCheck(key, 0L);

			var lines = Program.GetLines(check.Key);
			var rv = 0L;
			// magic

			check.Compare(rv);
			return rv;
		}
	}
    public class Edge25
    {
        public string From { get; }
        public string To { get; }
        public Edge25(string from, string to)
        {
			From = from;
			To = to;
        }
        public override bool Equals(object? obj)
        {
            if (obj is not Edge25 other) 
                return false;
            return (From == other.From && To == other.To);
        }
        public override string ToString()
        {
            return $"{From} to {To}";
        }
    }
    public class Graph25
    {
		// graph is represented as an array of edges.
		// Since the graph is undirected, the edge
		// from src to dest is also edge from dest
		// to src. Both are counted as 1 edge here.
        public Graph25()
        {

        }
        public List<Edge25> Edges { get; } = [];
        readonly Random _rnd = new Random();
        public List<int> GroupSizes(List<Edge25> cuts)
        {
            var verts = GetVerts();
            var rv= new List<int>();
            foreach (var vert in verts)
            {
                var path = GetPath(vert, cuts, new List<string>());
                if (!rv.Contains(path.Count()))
                    rv.Add(path.Count());
            }
            return rv;
        }
        IEnumerable<Edge25> EdgesWithout(List<Edge25> cuts)
        {
            return Edges.Where(e => !cuts.Any(c => c.Equals(e)));
        }
        List<string> GetPath(string vert, List<Edge25> cuts, List<string> path)
        {
            var rv = new List<string>();
            if (path.Contains(vert))
                return rv;
            rv.Add(vert);
            path.Add(vert);

            var heads = new List<string>();
            foreach (var edge in EdgesWithout(cuts).Where(e => e.To == vert))
                rv.AddRange(GetPath(edge.From, cuts, path));
            foreach (var edge in EdgesWithout(cuts).Where(e => e.From == vert))
                rv.AddRange(GetPath(edge.To, cuts, path));
            return rv;
        }
        List<string> GetVerts()
        {
            var strs = new List<string>();
            foreach (var edge in Edges)
            {
                strs.Add(edge.From);
                strs.Add(edge.To);
            }

            return strs.Distinct().ToList();
        }
        public List<Edge25> KargerMinCut()
        {
            var verts = GetVerts();

            var subsets = new Subsets25();
            foreach(var str in verts)
                subsets.Add(str, new Subset25(str, 0));

            int vertices = subsets.Count();

            while (vertices > 2)
            {
                // Pick a random edge
                int i = _rnd.Next(0, Edges.Count());
                var rndEdge = Edges[i];
                var subset1 = subsets.Find(rndEdge.From);
                var subset2 = subsets.Find(rndEdge.To);

                if (subset1 == subset2)
                    continue;
                else
                {
                    //ElfHelper.DayLog($"Contracting edge {rndEdge}");
                    vertices--;
                
                    subsets.Union(subset1, subset2);
                }
            }

            var rv = new List<Edge25>();
            foreach(var edge in Edges)
            {
                var subset1 = subsets.Find(edge.From);
                var subset2 = subsets.Find(edge.To);
                if (subset1 != subset2)
                    rv.Add(edge);
            }
            return rv;
        }
    }
    public class Subsets25 : Dictionary<string, Subset25>    // comp, rank
    {
        public string Find(string name)
        {
            Utils.Assert(this.ContainsKey(name), "Not in dict");
            if (this[name].Parent != name)
                this[name].Parent = Find(this[name].Parent);

            return this[name].Parent;
        }
        public void Union(string x, string y)
        {
            var xroot = Find(x);
            var yroot = Find(y);

            if (this[xroot].Rank < this[yroot].Rank)
            {
                this[xroot].Parent = yroot;
            }
            else if (this[xroot].Rank > this[yroot].Rank)
            {
                this[yroot].Parent = xroot;
            }
            else
            {
                this[yroot].Parent = xroot;
                this[xroot].Rank++;
            }
        }
    }
    public class Subset25b
    {
        public int Parent { get; set; }
        public int Rank { get; set; }
        public Subset25b(int parent, int rank)
        {
            Parent = parent;
            Rank = rank;
        }
        public override string ToString()
        {
            return Parent + " r:" + Rank;
        }
    }
    public class Subset25
    {
        public string Parent { get; set; }
        public int Rank { get; set; }
        public Subset25(string parent, int rank)
        {
            Parent = parent;
            Rank = rank;
        }
        public override string ToString()
        {
            return Parent + " r:" + Rank;
        }
    }

    public class Comp25
    {
		public Comp25(string line) 
		{
			var parts = Utils.Split(':', line);
            Name = parts[0];
			Connections = Utils.Split(' ', parts[1]);
		}
        public override string ToString()
        {
            return $"{Name} c:{string.Join(' ', Connections)} e:{string.Join(' ', ExtraConnections)}";
        }


        public string Name { get; }
        public string[] Connections { get; }
		public List<string> ExtraConnections { get; internal set; } = [];
    }
}
