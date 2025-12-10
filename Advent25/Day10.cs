using AoCLibrary;
using System.ComponentModel.DataAnnotations;
namespace Advent25;

internal class Day10 : IDayRunner
{
	// Day https://adventofcode.com/2025/day/10
	// Input https://adventofcode.com/2025/day/10/input
    public RunnerResult Star1(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star1, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 7L);
		else
			res.Check = new StarCheck(key, 404L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);
		var rv = 0L;
		// magic
	    foreach (var line in lines)
            rv += new Machine10(line, key.Star).Match();
	 
        res.CheckGuess(rv);
        return res;
    }
    class Light10
    {
        public int Goal { get; }
        public int _val; // for expediency
        public Light10(bool goal)
        {
            Goal = goal ? 1 : 0;
        }
        public Light10(int goal, int val)
        {
            Goal = goal;
            _val = val;
        }
        public Light10(Light10 other)
        {
            Goal = other.Goal;
            _val = other._val;
        }
        override public string ToString()
        {
            return $"{_val}[{Goal}]";
        }
        public bool IsMatch()
        {
            return _val == Goal;
        }

        internal bool IsJOver()
        {
            return _val > Goal;
        }

        internal bool Increment()
        {
            _val++;
            return (_val <= Goal);
        }

        internal void Toggle()
        {
            if (_val == 0)
                _val = 1;
            else
                _val = 0;
        }

        internal long Delta()
        {
            return Math.Abs(_val - Goal);
        }
    }
   
    class Light10s
    {
        public Light10[] Lights { get; }
        StarEnum _star;
        public Light10s(StarEnum star, string str)
        {
            _star = star;
            if (star == StarEnum.Star1)
                Lights = str.Select(c => new Light10(c == '#')).ToArray();
            else //if (star == StarEnum.Star2)
            {
                var parts = str.Split(',');
                Lights = parts.Select(p => new Light10(int.Parse(p), 0)).ToArray();

            }
        }
        public Light10s(Light10s other)
        {
            _star = other._star;
            Lights = other.Lights.Select(l => new Light10(l)).ToArray();
        }
        public bool IsMatch()
        {
            foreach (var light in Lights)
            {
                if (!light.IsMatch())
                    return false;
            }
            return true;
        }
        public bool IsJOver()
        {
            foreach (var light in Lights)
            {
                if (light.IsJOver())
                    return true;
            }
            return false;
        }
        public override string ToString()
        {
            return string.Join("", Lights);
        }
        internal bool Act(Button10 button)
        {
            foreach (var light in button.LightIds)
            {
                if (StarEnum.Star1 == _star)
                    Lights[light].Toggle();
                else
                {
                    if (!Lights[light].Increment())
                        return false;
                }
            }
            return true;
        }
        internal void SwitchAll(Button10 button)
        {
            foreach (var light in button.LightIds)
            {
                Lights[light].Toggle();
            }
        }
        internal bool IncrementAll(Button10 button)
        {
            foreach (var light in button.LightIds)
            {
            }
            return true;
        }

        internal string CompareString()
        {
            return string.Join(',', Lights.Select(l => l._val));
        }
        internal long Delta()
        {
            return Lights.Sum(l => l.Delta());
        }
    }
    class Button10
    {
        public int[] LightIds;
        public Button10(string button)
        {
            var parts = button.Split(',');
            LightIds = parts.Select(p => int.Parse(p)).ToArray();
        }
        override public string ToString()
        {
            return $"b:{string.Join(",", LightIds)}";
        }
    }
    class ButtonSet10
    {
        public Light10s Lights { get; }
        public int Count { get; private set; } = 0;
        //List<Button10> _history = [];
        public ButtonSet10(Button10 button, Light10s lights)
        {
            Lights = new Light10s(lights);
            Add(button);
        }
        public ButtonSet10(ButtonSet10 set, Button10 button)
        {
            Lights = new Light10s(set.Lights);
            Count = set.Count;
            //_history.AddRange(set._history);
            Add(button);
        }
        public bool Add(Button10 button)
        {
            //_history.Add(button);
            var rv = Lights.Act(button);
            CompareString = Lights.CompareString();
            Count++;
            Delta = Lights.Delta();
            return rv;
        }
     
        override public string ToString()
        {
            return $"l:{Lights} c:{Count}";
        }
        public string CompareString { get; private set; }
        public long Delta { get; private set; }
    }
    class Machine10
    {
        public int Match()
        {
            if (_lights.IsMatch())
                return 0;

            var buttonSets = new List<ButtonSet10>();
            foreach (var button in _buttons)
            {
                var newSet = new ButtonSet10(button, _lights);
                if (newSet.Lights.IsMatch())
                    return newSet.Count;
                buttonSets.Add(newSet);
            }
            var compareStrings = new HashSet<string>();

            while (true)
            {
                var newSets = new List<ButtonSet10>();
                foreach (var set in buttonSets)
                {
                    foreach (var button in _buttons)
                    {
                        var newSet = new ButtonSet10(set, button);
                        if (newSet.Lights.IsMatch())
                            return newSet.Count;
                        if (!compareStrings.Contains(newSet.CompareString))
                        {
                            newSets.Add(newSet);
                            compareStrings.Add(newSet.CompareString);
                        }
                    }
                }
                buttonSets = newSets;
            }
        }
        public long JMatch()
        {
            if (_lights.IsMatch())
                return 0L;
            ElfHelper.DayLogPlus($"New Machine- {_lights}");

            using var ctx = new Microsoft.Z3.Context();
            using var opt = ctx.MkOptimize();
            var presses = Enumerable.Range(0, _buttons.Count())
                .Select(i => ctx.MkIntConst($"b{i}"))
                .ToArray();
            foreach(var press in presses)
                opt.Add(ctx.MkGe(press, ctx.MkInt(0)));

            for (var i = 0; i < _lights.Lights.Count(); i++)
            {
                var affecting = presses.Where((_, j) => _buttons[j].LightIds.Contains(i)).ToArray();
                if (affecting.Length > 0)
                {
                    Microsoft.Z3.Expr sum;
                    if (affecting.Length == 1)
                        sum = affecting[0];
                    else
                        sum = ctx.MkAdd(affecting);

                    opt.Add(ctx.MkEq(sum, ctx.MkInt(_lights.Lights[i].Goal)));
                }
            }

            if (presses.Length == 1)
                opt.MkMinimize(presses[0]);
            else
                opt.MkMinimize(ctx.MkAdd(presses));
            opt.Check();

            var model = opt.Model;
            return presses.Sum(p => ((Microsoft.Z3.IntNum)model.Evaluate(p, true)).Int64);
        }

        Light10s _lights;
        List<Button10> _buttons = [];
        public Machine10(string line, StarEnum star)
        {
            //[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
            var parts = line.Split(' ');
            foreach (var part in parts)
            {
                if (part.StartsWith("["))
                {
                    if (star == StarEnum.Star1)
                        _lights = new Light10s(star, part.Substring(1, part.Length - 2));
                }
                else if (part.StartsWith("("))
                {
                    _buttons.Add(new Button10(part.Substring(1, part.Length - 2)));
                }
                else if (part.StartsWith("{"))
                {
                    if (star == StarEnum.Star2)
                        _lights = new Light10s(star, part.Substring(1, part.Length - 2));
                }
            }         
        }
        static string StringBetween(string str, char a, char b)
        {
            var start = str.IndexOf(a);
            var end = str.IndexOf(b,start+1);
            if (start < 0 || end < 0 || end <= start)
                return "";
            return str.Substring(start + 1, end - start - 1);
        }
    }

    public RunnerResult Star2(bool isReal)
    {
        var key = new StarCheckKey(StarEnum.Star2, isReal, null);
        var res = new RunnerResult();
        if (!isReal)
			res.Check = new StarCheck(key, 33L);
		else
			res.Check = new StarCheck(key, 0L);

		var lines = Program.GetLines(key);
		//var text = Program.GetText(key);

		var rv = 0L;
        // magic
        foreach (var line in lines)
        {
            rv += new Machine10(line, key.Star).JMatch();
            ElfHelper.DayLogPlus("Machine done " + rv);
        }

        res.CheckGuess(rv);
        return res;
	}
}

