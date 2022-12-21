using System.Numerics;

namespace Advent22
{
    internal class Day11
    {
        static public void Run()
        {
            Star1();
            Star2();
        }
        static public void Star1()
        {
            var input = File.ReadAllLines("DayFake11.txt");
            var monkeys = new List<Monkey>();
            for(int i = 0; i < input.Count(); i += 7)
                monkeys.Add(new Monkey(input[i + 1], input[i + 2], input[i + 3], input[i + 4], input[i + 5]));

            for(int round = 0; round < 20; round++)
                for(int i = 0; i < monkeys.Count(); i++)
                {
                    var monkey = monkeys[i];
                    foreach (var item in monkey.Items)
                    {
                        monkey.OperateOn(item);
                        monkey.Relax(item);
                        if (monkey.Test(item))
                            monkey.ThrowTo(monkeys[monkey.IfTrue], item);
                        else
                            monkey.ThrowTo(monkeys[monkey.IfFalse], item);
                    }
                    monkey.Items.Clear();
                }

            var ordered = monkeys.OrderByDescending(m => m.InspectionCount).Take(2).ToArray();
            var score = ordered[0].InspectionCount * ordered[1].InspectionCount;
            Helper.Log("Star1 Score: " + score);

        }
        static public void Star2()
        {
            var input = File.ReadAllLines("Day11.txt");
            var monkeys = new List<Monkey>();
            for (int i = 0; i < input.Count(); i += 7)
                monkeys.Add(new Monkey(input[i + 1], input[i + 2], input[i + 3], input[i + 4], input[i + 5]));

            for (int round = 0; round < 10000; round++)
            {
                Helper.Log("Round " + round);
                for (int i = 0; i < monkeys.Count(); i++)
                {
                    var monkey = monkeys[i];
                    foreach (var item in monkey.Items)
                    {
                        monkey.OperateOn(item);
                        if (monkey.Test(item))
                            monkey.ThrowTo(monkeys[monkey.IfTrue], item);
                        else
                            monkey.ThrowTo(monkeys[monkey.IfFalse], item);
                    }
                    monkey.Items.Clear();
                }
            }

            var ordered = monkeys.OrderByDescending(m => m.InspectionCount).Take(2).ToArray();
            var score = ordered[0].InspectionCount * ordered[1].InspectionCount;
            Helper.Log("Star2 Score: " + score);
        }
    }
    class Item // bi slide
    {
        public Item(long worryLevel)
        {
            _bi = new BigInteger(worryLevel);
            Slide();
        }
        public override string ToString()
        {
            return _bi.ToString();
        }

        void Slide()
        {
            
            var lcm = 9699690;
            if (_bi > lcm)
            {
                var raw = _bi / lcm;
                _bi = _bi - (raw * lcm);
            }
        }
        private BigInteger _bi;

        internal bool IsDivisible(int denominator)
        {
            return _bi % denominator == 0;
        }
        internal void Square()
        {
            _bi = _bi * _bi;
            Slide();
        }
        internal void Multiply(int prime)
        {
            _bi = _bi * prime;
            Slide();
        }
        internal void DivideBy3()
        {
            _bi = _bi / 3;
        }
        internal void Add(int n)
        {
            _bi = _bi + n;
        }
    }
    class ItemBi
    {
        public ItemBi(long worryLevel)
        {
            _bi = new BigInteger(worryLevel);
        }
        public override string ToString()
        {
            return _bi.ToString();
        }

        private BigInteger _bi;

        internal bool IsDivisible(int denominator)
        {
            return _bi % denominator == 0;
        }
        internal void Square()
        {
            _bi = _bi * _bi;
        }
        internal void Multiply(int prime)
        {
            _bi = _bi * prime;
        }
        internal void DivideBy3()
        {
            _bi = _bi / 3;
        }
        internal void Add(int n)
        {
            _bi = _bi + n;
        }
    }

    class ItemPrime
    {
        //private BigInteger _bi;
        static List<int> _primes = new List<int>();
        static ItemPrime()
        {
            int next = 2;
            while(_primes.Count() < 1000)
            {
                bool comp = false;
                foreach (var prime in _primes)
                {
                    if (next % prime == 0)
                    {
                        comp = true;
                        break;
                    }
                }
                if (!comp)
                    _primes.Add(next);
                next++;
            }
        }

        public ItemPrime(long worryLevel)
        {
            _dict = Factor(worryLevel);
            //_bi = worryLevel;
        }
        static Dictionary<int, int> Factor(BigInteger level)
        {
            var rv = new Dictionary<int, int>();
            if (level == 1)
            {
                rv.Add(1, 1);
                return rv;
            }
            foreach(var prime in _primes)
            {
                bool containsKey = rv.ContainsKey(prime);
                while (level % prime == 0)
                {
                    if (!containsKey)
                    {
                        rv.Add(prime, 0);
                        containsKey = true;
                    }
                    rv[prime]++;
                    level = level / prime;
                }
                if (level == 1)
                    break;
            }
            if (level != 1)
            {
                // now we have to use the remainer
                _remainder = (long)level;
                if (_remainder < 0)
                    Helper.Log("Bad News");
            }
            return rv;
        }
        public override string ToString()
        {
            return string.Join(",", _dict.Select(kvp => $"{kvp.Key}^{kvp.Value}"));
        }

        Dictionary<int, int> _dict = new Dictionary<int, int>();
        private static long _remainder = 1;

        internal bool IsDivisible(int denominator)
        {
            //Debug.Assert((_dict.ContainsKey(denominator)) == (_bi % denominator == 0));
            return (_dict.ContainsKey(denominator));
        }
        internal void Square()
        {
            foreach(var k in _dict.Keys)
                _dict[k] = _dict[k] * 2;
            //_bi = _bi * _bi;
            //Debug.Assert(ToBi() == _bi);
        }
        internal void Multiply(int prime)
        {
            if (!_dict.ContainsKey(prime))
                _dict.Add(prime, 1);
            else
                _dict[prime] = _dict[prime] * 2;

            //_bi = _bi * prime;
            //Debug.Assert(ToBi() == _bi);
        }
        internal void DivideBy3()
        {
            if (_dict.ContainsKey(3))
            {
                _dict[3]--;
                if (_dict[3] == 0)
                    _dict.Remove(3);
            }
            else
            {
                var bi = ToBi();
                _dict = Factor(bi / 3);
            }

            //_bi = _bi / 3;
            //Debug.Assert(ToBi() == _bi);
        }
        BigInteger ToBi()
        {
            BigInteger bi = 1;
            foreach (var kvp in _dict)
                bi = bi * new BigInteger(Math.Pow(kvp.Key, kvp.Value));
            return bi;
        }
        internal void Add(int n)
        {
            var bi = ToBi();
            bi += n;
            _dict = Factor(bi);

            //_bi = _bi + n;
            //Debug.Assert(ToBi() == _bi);
        }
    }

    class Monkey
    {
        public List<Item> Items { get; set; } = new List<Item>();
        public string Operation { get; }
        public int Denominator{ get; }
        public int IfTrue { get; }
        public int IfFalse { get; }
        public long InspectionCount { get; private set; }

        public Monkey(string line1, string line2, string line3, string line4, string line5)
        {
            var parts = line1.Replace("Starting items:", "").Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
                Items.Add(new Item(int.Parse(part)));
            Operation = line2.Replace("Operation: new = old ", "").Trim();
            Denominator = int.Parse(line3.Replace("Test: divisible by ", "").Trim());
            IfTrue = int.Parse(line4.Replace("If true: throw to monkey ", "").Trim());
            IfFalse = int.Parse(line5.Replace("If false: throw to monkey ", "").Trim());
        }
        internal bool Test(Item item)
        {
            return item.IsDivisible(Denominator);
        }
        internal void OperateOn(Item item)
        {
            InspectionCount++;
            if (Operation == "* old")
                item.Square();
            else
            {
                var parts = Operation.Split(" ");
                var val = int.Parse(parts[1]);
                if (parts[0] == "+")
                    item.Add(val);
                if (parts[0] == "*")
                    item.Multiply(val);
            }
            //if (item.WorryLevel < 0)
            //    Helper.Log("Bad news!");
        }
        internal void Relax(Item item)
        {
            item.DivideBy3();
            //return item.WorryLevel;
        }

        internal void ThrowTo(Monkey monkey, Item item)
        {
            monkey.Items.Add(item);
        }
        public override string ToString()
        {
            return string.Join(",", Items) + " (" + InspectionCount + ")";
        }

    }
}

