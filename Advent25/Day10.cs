using AoCLibrary;
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
        int _goal;
        public int _val; // for expediency
        public Light10(bool goal)
        {
            _goal = goal ? 1 : 0;
        }
        public Light10(int goal, int val)
        {
            _goal = goal;
            _val = val;
        }
        public Light10(Light10 other)
        {
            _goal = other._goal;
            _val = other._val;
        }
        override public string ToString()
        {
            return $"{_val}[{_goal}]";
        }
        public bool IsMatch()
        {
            return _val == _goal;
        }

        internal bool IsJOver()
        {
            return _val > _goal;
        }

        internal bool Increment()
        {
            _val++;
            return (_val <= _goal);
        }

        internal void Toggle()
        {
            if (_val == 0)
                _val = 1;
            else
                _val = 0;
        }
    }
   
    class Light10s
    {
        Light10[] _lights;
        StarEnum _star;
        public Light10s(StarEnum star, string str)
        {
            _star = star;
            if (star == StarEnum.Star1)
                _lights = str.Select(c => new Light10(c == '#')).ToArray();
            else //if (star == StarEnum.Star2)
            {
                var parts = str.Split(',');
                _lights = parts.Select(p => new Light10(int.Parse(p), 0)).ToArray();

            }
        }
        public Light10s(Light10s other)
        {
            _star = other._star;
            _lights = other._lights.Select(l => new Light10(l)).ToArray();
        }
        public bool IsMatch()
        {
            foreach (var light in _lights)
            {
                if (!light.IsMatch())
                    return false;
            }
            return true;
        }
        public bool IsJOver()
        {
            foreach (var light in _lights)
            {
                if (light.IsJOver())
                    return true;
            }
            return false;
        }
        public override string ToString()
        {
            return string.Join("", _lights);
        }
        internal bool Act(Button10 button)
        {
            foreach (var light in button.LightIds)
            {
                if (StarEnum.Star1 == _star)
                    _lights[light].Toggle();
                else
                {
                    if (!_lights[light].Increment())
                        return false;
                }
            }
            return true;
        }
        internal void SwitchAll(Button10 button)
        {
            foreach (var light in button.LightIds)
            {
                _lights[light].Toggle();
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
            return string.Join(',', _lights.Select(l => l._val));
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
            return rv;
        }
     
        override public string ToString()
        {
            return $"l:{Lights} c:{Count}";
        }
        public string CompareString { get; private set; }
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
        public int JMatch()
        {
            if (_lights.IsMatch())
                return 0;
            ElfHelper.DayLogPlus($"New Machine- {_lights}");

            var buttonSets = new List<ButtonSet10>();
            _buttons = _buttons.OrderByDescending(b => b.LightIds.Length).ToList();
            foreach (var button in _buttons)
            {
                var newSet = new ButtonSet10(button, _lights);
                if (newSet.Lights.IsMatch())
                    return newSet.Count;
                buttonSets.Add(newSet);
            }
            var next = DateTime.Now;
            var compareStrings = new HashSet<string>();
            var i = 0L;
            while (true)
            {
                var newSets = new List<ButtonSet10>();
                foreach (var set in buttonSets)
                {
                    foreach (var button in _buttons)
                    {
                        var newSet = new ButtonSet10(set, button);
                        if (newSet.Lights.IsJOver())
                            continue;
                        if (newSet.Lights.IsMatch())
                            return newSet.Count;

                        if (!compareStrings.Contains(newSet.CompareString))
                        {
                            newSets.Add(newSet);
                            compareStrings.Add(newSet.CompareString);
                        }
                    }
                }
                if (DateTime.Now > next)
                {
                    ElfHelper.DayLogPlus($"{i} Sets {buttonSets.Count} => {newSets.Count}");
                    next = DateTime.Now.AddSeconds(5);
                }
                i++;
                buttonSets = newSets;
            }
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

