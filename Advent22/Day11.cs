using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace Advent22
{
    internal class Day11
    {
        static public void Run()
        {
            //Star1();
            Star2();
        }
        static public void Star1()
        {
            var input = File.ReadAllLines("Day11.txt");
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
            var input = File.ReadAllLines("DayFake11.txt");
            var monkeys = new List<Monkey>();
            for (int i = 0; i < input.Count(); i += 7)
                monkeys.Add(new Monkey(input[i + 1], input[i + 2], input[i + 3], input[i + 4], input[i + 5]));

            for (int round = 0; round < 1000; round++)
            {
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
    class Item
    {
        static int[] _primes = new int[] { 2, 3, 5, 7, 11, 13, 17, 19 };
        public Item(long worryLevel)
        {
            if (worryLevel == 1)
            {
                _dict.Add(1, 1);
                return;
            }
            for (int i = 2; i < 1000; i++)
            {
                while (worryLevel % i == 0)
                {
                    if (!_dict.ContainsKey(i))
                        _dict.Add(i, 0);
                    _dict[i]++;
                    worryLevel = worryLevel / i;
                }
                if (worryLevel == 1)
                    break;
            }
        }
        public override string ToString()
        {
            return string.Join(",", _dict.Select(kvp => $"{kvp.Key}^{kvp.Value}"));
        }

        public long WorryLevel { get; set; }

        Dictionary<int, int> _dict = new Dictionary<int, int>();
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
            return (item.WorryLevel % Denominator == 0);
        }
        internal double OperateOn(Item item)
        {
            InspectionCount++;
            if (Operation == "* old")
                item.WorryLevel = item.WorryLevel * item.WorryLevel;
            else
            {
                var parts = Operation.Split(" ");
                var val = int.Parse(parts[1]);
                if (parts[0] == "+")
                    item.WorryLevel = item.WorryLevel + val;
                if (parts[0] == "*")
                    item.WorryLevel = item.WorryLevel * val;
            }
            if (item.WorryLevel < 0)
                Helper.Log("Bad news!");
            return item.WorryLevel;
        }
        internal double Relax(Item item)
        {
            item.WorryLevel = (long)((double)item.WorryLevel / 3.0);
            return item.WorryLevel;
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

