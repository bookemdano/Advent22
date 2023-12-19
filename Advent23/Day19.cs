using AoCLibrary;
using System.Data;
using System.Security.AccessControl;

namespace Advent23
{
	internal class Day19 : IDayRunner
	{
		public bool IsReal => false;

		// Day https://adventofcode.com/2023/day/19
		// Input https://adventofcode.com/2023/day/19/input
		public object? Star1()
		{
			var key = new StarCheckKey(StarEnum.Star1, IsReal);
			StarCheck check;
			if (!IsReal)
				check = new StarCheck(key, 19114);
			else
				check = new StarCheck(key, 495298);

			var lines = Program.GetLines(check.Key, true);
			var rv = 0L;
			// magic
			var workflows = GetWorkflows(lines);
			var parts = GetParts(lines);
			foreach (var part in parts)
			{
				var output = CheckWorkflows(workflows, part);
				if (Rule19.IsAccept(output))
					rv += part.Sum();
			}
			check.Compare(rv);
			// 495298
			return rv;
		}
		Dictionary<string, Workflow19> GetWorkflows(IEnumerable<string> lines)
		{
			var workflows = new Dictionary<string, Workflow19>();
			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line))
					break;
				var workflow = new Workflow19(line);
				workflows.Add(workflow.Name, workflow);
			}
			return workflows;
		}
		List<Part19> GetParts(IEnumerable<string> lines)
		{
			bool inRules = true;
			var parts = new List<Part19>();
			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line))
				{
					inRules = false;
					continue;
				}

				if (!inRules)
					parts.Add(new Part19(line));
			}
			return parts;
		}
		string CheckWorkflows(Dictionary<string, Workflow19> workflows, Part19 part)
		{
			var output = "in";
			var path = new List<string>();
			while (!Rule19.IsFinal(output))
			{
				path.Add(output);
				output = workflows[output].Evaluate(part);
			}
			ElfHelper.DayLog($"{part} in {string.Join("=>", path)} => {output}");
			return output;
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
			var workflows = GetWorkflows(lines);
			var queue = new List<Workflow19>();
			var limitSets = new List<LimitSet19>();
			limitSets.Add(new LimitSet19());
			var stack = new Stack<Workflow19>();
			stack.Push(workflows["in"]);
			while(stack.Any())
			{
				var newLims = new List<LimitSet19>();
				foreach (var limitSet in limitSets)
				{
					var workflow = stack.Pop();
					foreach (var rule in workflow.Rules)
					{
						var lims = limitSet.Overlaps(rule);
						if (!lims.Any())
						{
							limitSet.SetOutput(rule.Output);
							continue;
						}
						// break apart
						var newLimitSet = limitSet.Split(rule);
						if (newLimitSet != null)
						{
							newLims.Add(newLimitSet);
							stack.Push(workflows[rule.Output]);
						}
					}
				}
				limitSets.AddRange(newLims);
			}
			check.Compare(rv);
			return rv;
		}
	}
	public class LimitSet19
	{
		public LimitSet19(LimitSet19 other)
		{
			var ratings = "xmas".ToCharArray();
			foreach(var rating in ratings)
			{
				_limits.Add(rating, new List<Limit19>());
				foreach (var limit in other.GetLimits(rating))
					_limits[rating].Add(new Limit19(limit));
			}
		}
		List<Limit19> GetLimits(char rating)
		{
			return _limits[rating];
		}
		public LimitSet19()
		{
			_limits.Add('x', new List<Limit19>() { new Limit19(0, 4000) });
			_limits.Add('m', new List<Limit19>() { new Limit19(0, 4000) });
			_limits.Add('a', new List<Limit19>() { new Limit19(0, 4000) });
			_limits.Add('s', new List<Limit19>() { new Limit19(0, 4000) });
		}
		Dictionary<char, List<Limit19>> _limits = [];

		internal LimitSet19? Split(Rule19 rule)
		{
			if (rule.Condition == null)
				return null;
			var rv = new LimitSet19(this);
			if (FutherLimitInverse(rule))
			{
				rv.FutherLimit(rule);
				return rv;
			}
			return null;
		}
		public override string ToString()
		{
			var parts = new List<string>();
			foreach (var kvp in _limits)
				parts.Add($"{kvp.Key} ({string.Join("|", kvp.Value)})");
			return string.Join(",", parts);
		}
		private bool FutherLimitInverse(Rule19 rule)
		{
			Utils.Assert(rule.Condition != null, "blank condition");
			var condition = rule.Condition!;
			foreach (var limit in _limits[condition.Rating])
			{
				if (limit.Contains(condition.Value))
				{
					if (condition.Relation == ">")
						limit.End = condition.Value;
					else if (condition.Relation == "<")
						limit.Start = condition.Value;
					return true;
				}
			}
			return false;
		}
		private bool FutherLimit(Rule19 rule)
		{
			Utils.Assert(rule.Condition != null, "blank condition");
			var condition = rule.Condition!;
			foreach (var limit in _limits[condition.Rating])
			{
				if (limit.Contains(condition.Value))
				{
					if (condition.Relation == ">")
						limit.Start = condition.Value + 1;
					else if (condition.Relation == "<")
						limit.End = condition.Value - 1;
					limit.Output = rule.Output;
					return true;
				}
			}
			return false;
		}

		internal List<Limit19> Overlaps(Rule19 rule)
		{
			var rv = new List<Limit19>();
			if (rule.Condition == null)
				return rv;
			var rating = rule.Condition.Rating;
			var limits = _limits[rating];
			foreach(var limit in limits)
			{
				if (rule.Condition.Evaluate(limit.Start) || rule.Condition.Evaluate(limit.End))
					rv.Add(limit);
			}
			return rv;
		}

		internal void SetOutput(string output)
		{
			foreach(var kvp in _limits)
			{
				foreach(var limit in kvp.Value)
				{
					limit.Output = output;
				}
			}
		}
	}
	public class Limit19
	{
		public Limit19(Limit19 other)
		{
			Start = other.Start;
			End = other.End;
			Output = other.Output;
		}
		public bool Contains(int value)
		{
			return value >= Start && value <= End;
		}
		public Limit19(int start, int end)
		{
			Start = start;
			End = end;
			Output = "";
		}
		public override string ToString()
		{
			return $"s:{Start}-e:{End} o:{Output}";
		}
		public int Start { get; set; }
		public int End { get; set; }
		public string Output { get; set; }
	}
	public class Workflow19
	{
		//ex{x>10:one,m<20:two,a>30:R,A}
		public Workflow19(string line)
		{
			var parts = Utils.Split('{', line.Replace("}",""));
			Name = parts[0];
			var ruleStrings = Utils.Split(',', parts[1]);
			foreach (var ruleString in ruleStrings)
			{
				Rules.Add(new Rule19(ruleString));
			}
		}

		public string Name { get; }
		public List<Rule19> Rules { get; } = [];

		public string Evaluate(Part19 part)
		{
			foreach(var rule in Rules)
			{
				var o = rule.Evaluate(part);
				if (o != null)
					return o;
			}
			return null;
		}

	}

	public class Condition19
	{
		public Condition19(char rating, string relation, int value)
		{
			Rating = rating;
			Relation = relation;
			Value = value;
		}
		public Condition19(string line)
		{
			if (line.Contains('>'))
				Relation = ">";
			else if (line.Contains("<"))
				Relation = "<";
			var parts = line.Split("><".ToCharArray());
			Rating = parts[0].First();
			Value = int.Parse(parts[1]);
		}
		public bool Evaluate(Part19 part)
		{
			return Evaluate(part.GetValue(Rating));
		}
		public bool Evaluate(int compareTo)
		{
			if (Relation == ">")
				return (compareTo > Value);
			else if (Relation == "<")
				return (compareTo < Value);
			return false;
		}

		public string Relation { get; }
		public char Rating { get; }
		public int Value { get; }
		public override string ToString()
		{
			return $"{Rating}{Relation}{Value}";
		}
	}
	public class Rule19
	{
		//x>10:one
		public Rule19(string line)
		{
			var parts = Utils.Split(':', line);
			Output = parts.Last();
			if (parts.Length > 1)
			{
				Condition = new Condition19(parts[0]);
			}
		}
		static public bool IsFinal(string output)
		{
			return IsAccept(output) || IsReject(output);
		}
		static public bool IsAccept(string output)
		{
			return (output == "A");
		}
		static public bool IsReject(string output)
		{
			return (output == "R");
		}
		public string Output { get; }
		public Condition19? Condition { get; }
		public string? Evaluate(Part19 part)
		{
			if (Condition == null || Condition.Evaluate(part))
				return Output;
			return null;
		}
		public override string ToString()
		{
			return $"c:{Condition} => o:{Output}";
		}
	}
	public class Part19
	{
		Dictionary<char, int> _dict = [];
		public Part19(string line)
		{
			var parts = Utils.Split(',', line.Replace("{", "").Replace("}", ""));
			foreach(var part in parts)
			{
				var subs = Utils.Split('=', part);
				_dict.Add(subs[0].First(), int.Parse(subs[1]));
			}
		}
		public int Sum()
		{
			return _dict.Sum(d => d.Value);
		}
		public override string ToString()
		{
			return string.Join(',', _dict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
		}

		internal int GetValue(char rating)
		{
			return _dict[rating];
		}
	}
}
