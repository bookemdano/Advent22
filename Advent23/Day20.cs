using AoCLibrary;
using System.Globalization;
namespace Advent23
{
	internal class Day20 : IDayRunner
	{
		public bool IsReal => true;

		// Day https://adventofcode.com/2023/day/20
		// Input https://adventofcode.com/2023/day/20/input
		public object? Star1()
		{
			var checks = new List<StarCheck>();
			if (!IsReal)
			{
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal, 0), 32000000L));
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal, 1), 11687500L));
			}
			else
				checks.Add(new StarCheck(new StarCheckKey(StarEnum.Star1, IsReal), 808146535L));

			var rv = 0L;
			foreach (var check in checks)
			{
				var lines = Program.GetLines(check.Key);
				// magic
				rv = 0L;
				var modules = new Modules20(lines);

				var n = 1000;

				for(int i = 0; i < n; i++)
				{
					modules.PushButton(stopAtReset: false);
					modules.Score();
				}
				rv = modules.Score();
				check.Compare(rv);
				// 15845130	too low
				//808146535
			}
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
			// magic
			long rv = 0L;
			var modules = new Modules20(lines);
			{
				var inputs = modules["rg"].Inputs.Keys.ToList();
				while(inputs.Count() < modules.Count())
				{
					var newInputs = new List<string>();
					foreach (var input in inputs)
						newInputs.AddRange(modules[input].Inputs.Keys);
					inputs.AddRange(newInputs);
					inputs = inputs.Distinct().ToList();
				}
			}
			int i = 0;
			while(true)
			{
				i++;
				modules.PushButton(stopAtReset: true);
				if (modules.IsReset)
					break;
			}
			rv = i;

			check.Compare(rv);
			return rv;
		}
	}
	public class Modules20 : Dictionary<string, Module20>
	{
		public Modules20(string[] lines)
		{
			foreach (var line in lines)
			{
				var mod = new Module20(line);
				Add(mod.Name, mod);
			}
			foreach(var mod in this.Values)
			{
				foreach(var outKey in mod.Outputs)
				{
					if (!this.ContainsKey(outKey))
						continue;
					this[outKey].AddInput(mod.Name);
				}
			}
		}

		int _highs;
		int _lows;
		internal long Score()
		{
			ElfHelper.DayLog($"Score() {_highs} {_lows}");
			return (long) _highs * _lows;
		}
		internal void PushButton(bool stopAtReset)
		{
			var mods = new List<Module20>();
			AddScore(Pulse20Enum.Lo);
			mods.Add(this["broadcaster"]);
			while (mods.Any())
			{
				var newMods = new List<Module20>();
				foreach(var mod in mods)
				{
					foreach (var output in mod.Outputs)
					{
						AddScore(mod.Pulse);
						if (stopAtReset && mod.Pulse == Pulse20Enum.Lo && output == "rx")
						{
							IsReset = true;
							return;
						}
						if (!ContainsKey(output))
							continue;
						var newMod = this[output];
						if (newMod.Fire(mod.Name, mod.Pulse))
						{
							newMods.Add(newMod);
						}
					}
				}
				mods = newMods;
			}
		}

		private void AddScore(Pulse20Enum pulse)
		{
			if (pulse == Pulse20Enum.Hi)
				_highs++;
			else if (pulse == Pulse20Enum.Lo)
				_lows++;
		}

		internal bool IsReset { get; private set; }
	}
	public enum Pulse20Enum
	{
		Lo,
		Hi
	}
	public class Module20
	{
		public Module20(string line)
		{
			var parts = line.Split("->".ToCharArray(), StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			Name = parts[0];
			if (parts[0].StartsWith('%') || parts[0].StartsWith('&'))
			{
				_modType = parts[0][0];
				Name = Name.Substring(1);
			}
			Outputs = Utils.Split(',', parts[1]);
			Pulse = Pulse20Enum.Lo;
		}
		public override string ToString()
		{
			return GetString(false);
		}
		public string GetString(bool shortString)
		{
			if (shortString)
				return $"{_modType}{Name} {Pulse}";
			var inputString = string.Empty;
			if (IsConj)
			{
				inputString = string.Join(',', Inputs.Select(kvp => $"{kvp.Key}-{kvp.Value}"));
				if (!string.IsNullOrEmpty(inputString))
					inputString = $" i:({inputString})";
			}
			return $"{_modType}{Name} {Pulse} o:({string.Join(',', Outputs)}){inputString}";
		}

		static Pulse20Enum Flip(Pulse20Enum pulse)
		{
			if (pulse == Pulse20Enum.Hi)
				return Pulse20Enum.Lo;
			else
				return Pulse20Enum.Hi;
		}
		internal bool Fire(string from, Pulse20Enum pulse)
		{
			bool rv = true;
			var origPulse = Pulse;
			if (IsFlip)
			{
				if (pulse == Pulse20Enum.Lo)
					Pulse = Flip(Pulse);
				else
					rv = false;
			}
			else if (IsConj)
			{
				Inputs[from] = pulse;
				if (Inputs.Values.All(b => b == Pulse20Enum.Hi))
					Pulse = Pulse20Enum.Lo;
				else
					Pulse = Pulse20Enum.Hi;
			}
			//ElfHelper.DayLog($"{_modType}{Name} Fire(from:{from},{pulse}) from {origPulse} to {Pulse} forward:{rv}");
			return rv;
		}

		internal void AddInput(string name)
		{
			if (!Inputs.ContainsKey(name))
				Inputs.Add(name, Pulse20Enum.Lo);
		}
		char _modType = '-';
		public string Name { get; }
		public string[] Outputs { get; }
		public Dictionary<string, Pulse20Enum> Inputs { get; } = [];
		public bool IsFlip => _modType == '%';
		public bool IsConj => _modType == '&';
		public Pulse20Enum Pulse { get; private set; }
	}
}
