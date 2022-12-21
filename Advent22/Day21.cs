namespace Advent22
{
    internal class Day21
    {
        static bool _logAll = false;
        static bool _fake = false;
        static public void Run()
        {
            //Day1();
            Day2();
        }
        public class Monkey
        {
            public Monkey(string line)
            {
                var halves = line.Split(':');
                Name = halves[0];
                var parts = halves[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                    Number = int.Parse(parts[0]);
                else
                {
                    _dependNames = new string[2];
                    _dependNames[0] = parts[0];
                    Operation = parts[1][0];
                    _dependNames[1] = parts[2];
                }
            }
            public string Name { get; set; }
            public int Number { get; set; }
            public char Operation { get; set; }
            public Monkey Left { get; set; }
            public Monkey Right { get; set; }
            string[] _dependNames { get; set; }
            public bool HasNumber
            {
                get
                {
                    return _dependNames == null;
                }
            }
            public override string ToString()
            {
                if (HasNumber)
                    return $"{Name}: {Number}";
                else
                    return $"{Name}: {_dependNames[0]} {Operation} {_dependNames[1]}";
            }
            internal bool Contains(Monkey monkey)
            {
                if (monkey == this)
                    return true;
                if (HasNumber)
                    return false;

                return Left.Contains(monkey) || Right.Contains(monkey);
            }
            long? _answer = null;
            internal long Answer()
            {
                if (_logAll)
                    Helper.Log("Find Answer: " + this);
                long rv = Number;
                if (!HasNumber)
                {
                    if (_answer == null)
                    {
                        rv = Operate(Left.Answer(), Right.Answer(), Operation);
                        _answer = rv;
                    }
                    rv = _answer.Value;
                }
                if (_logAll)
                    Helper.Log("Found Answer: " + this + " = " + rv);
                return rv;
            }
            static public char Opposite(char operation)
            {
                if (operation == '+')
                    return '-';
                else if (operation == '-')
                    return '+';
                else if (operation == '*')
                    return '/';
                else //if (Operation == '/')
                    return '*';
            }
            static public long Operate(long left, long right, char operation)
            {
                if (operation == '+')
                    return left + right;
                else if (operation == '-')
                    return left - right;
                else if (operation == '*')
                    return left * right;
                else
                {
                    var d = (double)left / right;
                    var diff = Math.Abs(d - (left / right));
                    if (diff > .1)
                        Helper.Log("Funky");
                    //if (Operation == '/')
                    return left / right;
                }
            }

            internal void Fill(Monkey[] monks)
            {
                if (HasNumber)
                    return;
                Left = monks.First(m => m.Name == _dependNames[0]);
                Right = monks.First(m => m.Name == _dependNames[1]);
            }

            internal void GetChainTo(Monkey monkey, List<Monkey> chain)
            {
                chain.Add(this);
                if (monkey == this)
                    return;
                if (Left.Contains(monkey))
                    Left.GetChainTo(monkey, chain);
                else
                    Right.GetChainTo(monkey, chain);
            }

        }
        private static void Day1()
        {
            var monks = File.ReadAllLines("day21.txt").Select(l => new Monkey(l)).ToArray();
            foreach (var monk in monks)
                monk.Fill(monks);

            var root = monks.First(m => m.Name == "root");
            Helper.Log("Final Answer = " + root.Answer());  //1141925498 too low
        }
        private static void Day2()
        {
            var monks = File.ReadAllLines("day21.txt").Select(l => new Monkey(l)).ToArray();
            foreach (var monk in monks)
                monk.Fill(monks);
            var root = monks.First(m => m.Name == "root");
            root.Answer();  // fills in all the answers
            var human = monks.First(m => m.Name == "humn");
            var leftHuman = root.Left.Contains(human);
            var humanChain = new List<Monkey>();
            long target;
            if (leftHuman)
            {
                root.Left.GetChainTo(human, humanChain);
                target = root.Right.Answer();
            }
            else
            {
                root.Right.GetChainTo(human, humanChain);
                target = root.Left.Answer();
            }

            Helper.Log("Target " + target);
            Helper.Log("Old human total was " + root.Right.Answer());

            foreach (var monk in humanChain)
            {
                Helper.Log("Chain " + monk);
                if (monk == human)
                    break;
                if (monk.Left.Contains(human))
                {
                    var sub = monk.Right.Answer();// put sub in left spot because we are going backwards
                    Helper.Log("Sub " + monk.Right.Name + " = " + sub);
                    target = Monkey.Operate(target, sub, Monkey.Opposite(monk.Operation));
                }
                else
                {
                    var sub = monk.Left.Answer();// put sub in other spot because we are going backwards
                    Helper.Log("Sub " + monk.Left.Name + " = " + sub);
                    if (monk.Operation == '-')
                        target = Monkey.Operate(sub, target, monk.Operation);
                    else if (monk.Operation == '/')
                        target = Monkey.Operate(sub, target, monk.Operation);
                    else
                        target = Monkey.Operate(target, sub, Monkey.Opposite(monk.Operation));
                }
            }

            Helper.Log("Final Answer = " + target); // 7172286853922 too high, 611 too low
        }
    }
}
