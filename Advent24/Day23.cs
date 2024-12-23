using AoCLibrary;
using System.Data;
namespace Advent24;

internal class Day23 : IDayRunner
{
	public bool IsReal => true;

	// Day https://adventofcode.com/2024/day/23
	// Input https://adventofcode.com/2024/day/23/input
	public object? Star1()
	{
		var key = new StarCheckKey(StarEnum.Star1, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, 7L);
		else
			check = new StarCheck(key, 1175L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = 0L;
		// magic
		var sets = lines.Select(l => new Set23(l));

		var comps = new List<Comp23>();
		foreach (var set in sets)
		{
			var found1 = comps.FirstOrDefault(s => s.Name == set.C1);
			if (found1 == null)
			{
				found1 = new Comp23(set.C1);
				comps.Add(found1);
			}
			var found2 = comps.FirstOrDefault(s => s.Name == set.C2);
			if (found2 == null)
			{
				found2 = new Comp23(set.C2);
				comps.Add(found2);
			}
			found1.Connections.Add(found2);
			found2.Connections.Add(found1);
		}
		var ts = comps.Where(c => c.Name.StartsWith('t'));
		List<string> founds = [];
		foreach (var t in ts)
			founds.AddRange(t.Threes());
		rv = founds.Distinct().Count();

		check.Compare(rv);
		return rv;
	}
	public class Trail23
	{
		public List<Comp23> Comps { get; set; } = [];
		public Trail23()
		{

		}
		public Trail23(Comp23 comp) 
		{
			Comps.Add(comp);
		}

		public Comp23 Head => Comps.Last();
		public Trail23 Step(Comp23 comp)
		{
			var rv = new Trail23();
			rv.Comps.AddRange(Comps);			
			rv.Comps.Add(comp);
			return rv;
		}
		public override string ToString()
		{
			return $"{string.Join(',', Comps.Select(s=>s.Name))}";
		}
	}
	public class Comp23
	{
		public Comp23(string name)
		{
			Name = name;
		}
		public string Name { get; }
		public List<Comp23> Connections { get; } = [];

		public string BiggestGroup()
		{
			var trails = new List<Trail23>();
			trails.Add(new Trail23(this));
			var rv = string.Empty;
			var maxCount = 0;
			while(trails.Any())
			{
				var newTrails = new List<Trail23>();
				foreach(var trail in trails)
				{
					foreach (var conn in trail.Head.Connections)
					{
						if (trail.Comps.Any(t => t.Name == conn.Name))// already evaluates
							continue;
						var failed = false;
						foreach (var comp in trail.Comps)
						{
							if (!comp.ConnectedTo(conn.Name))
							{
								failed = true;
								break;
							}
						}
						if (!failed)
						{
							var newTrail = trail.Step(conn);
							newTrails.Add(newTrail);
							if (newTrail.Comps.Count > maxCount)
							{
								maxCount = newTrail.Comps.Count;
								rv = newTrail.ToString();
							}
						}
					}
				}
				trails = newTrails;
			}
			return rv;

		}
		public bool ConnectedTo(string name)
		{
			return Connections.Any(c => c.Name == name);
		}
		public List<string> Threes()
		{
			var rv = new List<string>();
			for (int i = 0; i < Connections.Count - 1; i++)
			{
				var conn1 = Connections[i];
				for (int j = i + 1; j < Connections.Count; j++)
				{
					var conn2 = Connections[j];
					var matches = conn1.Connections.Where(c1 => c1.Name == conn2.Name);
					if (matches.Any())
					{
						var n = matches.Count();
						rv.Add(Order(Name, conn1.Name, conn2.Name));
					}
				}
			}
			return rv;
		}
		string Order(string sz1, string sz2, string sz3)
		{
			var list = new List<string>() { sz1, sz2, sz3 };
			return string.Join(",",list.OrderBy(c => c));
		}
		public override string ToString()
		{
			return $"{Name} {String.Join(',', Connections.Select(c => c.Name))}";
		}
	}
	public class Set23
	{
		public Set23(string line)
		{
			var parts = line.Split('-');
			C1 = parts[0];
			C2 = parts[1];
		}

		public string C1 { get; set; }
		public string C2 { get; set; }
	}
	string Order(string str)
	{
		return string.Join(",", str.Split(',').OrderBy(c => c));
	}

	public object? Star2()
	{
		var key = new StarCheckKey(StarEnum.Star2, IsReal, null);
		StarCheck check;
		if (!IsReal)
			check = new StarCheck(key, "co,de,ka,ta");
		else
			check = new StarCheck(key, 0L);

		var lines = Program.GetLines(check.Key);
		//var text = Program.GetText(check.Key);
		var rv = string.Empty;
		// magic
		var sets = lines.Select(l => new Set23(l));

		var comps = new List<Comp23>();
		foreach (var set in sets)
		{
			var found1 = comps.FirstOrDefault(s => s.Name == set.C1);
			if (found1 == null)
			{
				found1 = new Comp23(set.C1);
				comps.Add(found1);
			}
			var found2 = comps.FirstOrDefault(s => s.Name == set.C2);
			if (found2 == null)
			{
				found2 = new Comp23(set.C2);
				comps.Add(found2);
			}
			found1.Connections.Add(found2);
			found2.Connections.Add(found1);
		}
		
		foreach(var comp in comps)
		{
			var group = comp.BiggestGroup();
			if (group.Length > rv.Length)
				rv = Order(group);
		}

		check.Compare(rv);
		return rv;
	}
}

