using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;

namespace Advent22.Days
{
    internal class Day16
    {
        public class Conn
        {
            public Conn(string name)
            {
                Name = name;
            }
            public string Name { get; }
            public int Distance { get; set; }
            public int Index
            {
                get
                {
                    return ValveList.GetIndex(Name);
                }
            }

        }
        public class ValveList
        {
            static List<string> _names = new List<string>();
            internal static int GetIndex(string name)
            {
                return _names.IndexOf(name);
            }
            public ValveList(string[] input)
            {
                Valves = new List<Valve>();
                foreach (var line in input)
                {
                    var valve = Valve.Parse(line);
                    _names.Add(valve.Name);
                    if (valve != null)
                        Valves.Add(valve);
                }

                Graph = new int[Valves.Count, Valves.Count];
                for (int x = 0; x < Valves.Count; x++)
                {
                    var xName = Valves[x].Name;
                    for (int y = 0; y < Valves.Count; y++)
                    {
                        if (x == y)
                        {
                            Graph[x, y] = 0;
                            continue;
                        }

                        var yName = Valves[y].Name;
                        if (Valves[x].Connections.Any(c => c.Name == yName))
                            Graph[x, y] = 1;
                        else
                            Graph[x, y] = 9;
                    }
                }
                Distances = FloydWarshall(Graph, Valves.Count);
                foreach (var valve in Valves)
                {
                    foreach (var conn in valve.Connections)
                        conn.Distance = Distances[valve.Index, conn.Index];
                }
                Draw();
            }

            // https://www.csharpstar.com/floyd-warshall-algorithm-csharp/
            public static int[,] FloydWarshall(int[,] graph, int verticesCount)
            {
                int[,] distance = new int[verticesCount, verticesCount];
                for (int i = 0; i < verticesCount; ++i)
                    for (int j = 0; j < verticesCount; ++j)
                        distance[i, j] = graph[i, j];
                for (int k = 0; k < verticesCount; ++k)
                {
                    for (int i = 0; i < verticesCount; ++i)
                    {
                        for (int j = 0; j < verticesCount; ++j)
                        {
                            if (distance[i, k] + distance[k, j] < distance[i, j])
                                distance[i, j] = distance[i, k] + distance[k, j];
                        }
                    }
                }
                return distance;
            }
            void Draw()
            {
                var n = Valves.Count;
                if (n > 10)
                    return;
                for (int x = 0; x < n; x++)
                {
                    var line = "";
                    for (int y = 0; y < n; y++)
                    {
                        line += " " + Graph[x, y];
                    }
                    Helper.Log(line);
                }
                if (Distances != null)
                {
                    Helper.Log("Distances");
                    for (int x = 0; x < n; x++)
                    {
                        var line = "";
                        for (int y = 0; y < n; y++)
                        {
                            line += " " + Distances[x, y];
                        }
                        Helper.Log(line);
                    }
                }
            }
            public List<Valve> Valves { get; set; }
            public int[,] Graph { get; }
            public int[,] Distances { get; }

            internal List<Valve> GetUseful()
            {
                return Valves.Where(v => v.Rate > 0).ToList();
            }

            internal int GetFlow(int daysRemaining, IEnumerable<Valve> goodValves, Valve from)
            {
                int max = 0;
                foreach (var to in goodValves)
                {
                    var days = daysRemaining - Distances[from.Index, to.Index] - 1;
                    if (days > 0)
                    {
                        var add = days * to.Rate + GetFlow(days, goodValves.Where(v => to.Name != v.Name), to);
                        if (add > max)
                            max = add;
                    }
                }
                return max;
            }
            internal int GetFlowDouble(int daysRemaining, IEnumerable<Valve> goodValves, Valve from)
            {
                int max = 0;
                foreach (var to in goodValves)
                {
                    var days = daysRemaining - Distances[from.Index, to.Index] - 1;
                    if (days > 0)
                    {
                        var add = days * to.Rate + GetFlow(days, goodValves.Where(v => to.Name != v.Name), to);
                        if (add > max)
                            max = add;
                    }
                }
                return max;
            }
        }
        public class DFSGraph
        {
            LinkedList<int>[] _edges;
            private List<int> _vals;
            int _total = 0;
            public DFSGraph(int size, List<int> vals)
            {
                _edges = new LinkedList<int>[size];
                _vals = vals;
            }
            public void AddEdge(int from, int to)
            {
                if (_edges[from] == null)
                {
                    _edges[from] = new LinkedList<int>();
                    _edges[from].AddFirst(to);
                }
                else
                {
                    var last = _edges[from].Last;
                    _edges[from].AddAfter(last, to);
                }
            }
            public void InternalDoIt(int from, bool[] seen)
            {
                seen[from] = true;

                if (_edges[from] != null)
                {
                    foreach (var v in _edges[from])
                    {
                        if (seen[v] != true)
                        {
                            InternalDoIt(v, seen);
                        }
                    }
                }
            }

            internal void DoIt(int from)
            {
                var seen = new bool[_edges.Length + 1];
                InternalDoIt(from, seen);
            }
        }
        public class Valve
        {
            public string Name { get; set; }
            public int Rate { get; set; }
            public List<Conn> Connections { get; set; }
            public bool Opened { get; private set; }
            public int Index
            {
                get
                {
                    return ValveList.GetIndex(Name);
                }
            }
            static public Valve Parse(string line)
            {

                var halves = line.Split(';');
                var parts = halves[0].Split("=; ".ToCharArray());
                var rate = int.Parse(parts[5]);
                var rv = new Valve();
                rv.Rate = rate;
                rv.Name = parts[1];
                parts = halves[1].Substring(23).Trim().Split(", ");
                rv.Connections = halves[1].Substring(23).Trim().Split(", ").ToList().Select(s => new Conn(s)).ToList();
                return rv;
            }

            public override string ToString()
            {
                var rv = $"{Name} r:{Rate} {(Opened ? "O" : "X")}";
                if (Connections?.Any() == true)
                    rv += $" to:{string.Join(",", Connections)}";
                return rv;
            }


        }
        static public void Run()
        {
            Day1();
            Day2();
        }
        static void Day1()
        {
            var input = File.ReadAllLines("Day16.txt");
            var valveList = new ValveList(input);
            var pressure = valveList.GetFlow(30,
                                   valveList.GetUseful(),
                                   valveList.Valves[0]);
            Helper.Log("Star1 Score: " + pressure);
        }
        static void Day2()
        {
            var input = File.ReadAllLines("Day16.txt");
            var valveList = new ValveList(input);
            var pressure2 = valveList.GetFlowDouble(26,
                                   valveList.GetUseful(),
                                   valveList.Valves[0]);
            Helper.Log("Star2 Score: " + pressure2);
        }
    }
}

