namespace Advent22
{
    internal class Day3
    {
        static public void Run()
        {
            var lines = File.ReadAllLines("Day3-input.txt");
            //lines = new string[] { "vJrwpWtwJgWrhcsFMMfFFhFp", "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", "PmmdzqPrVvPwwTWBwg", "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", "ttgJtRGJQctTZtZT", "CrZsJsPPZsGzwwsLwLmpwMDw" };
            var score2 = 0;
            for (int i = 0; i < lines.Length; i+=3)
            {
                var l1 = lines[i];
                var l2 = lines[i+1];
                var l3 = lines[i+2];
                foreach (var c in l1)
                {
                    if (l2.Contains(c))
                    {
                        if (l3.Contains(c))
                        {
                            score2 += Score(c);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine("score2 = " + score2);
        }
        static public void Run1()
        {
            var lines = File.ReadAllLines("Day3-input.txt");
            lines = new string[] { "vJrwpWtwJgWrhcsFMMfFFhFp", "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL", "PmmdzqPrVvPwwTWBwg", "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn", "ttgJtRGJQctTZtZT", "CrZsJsPPZsGzwwsLwLmpwMDw"};
            var score1 = 0;
            var both = new List<char>();
            foreach (var line in lines)
            {
                var left = line.Substring(0, line.Length / 2);
                var right = line.Substring(line.Length / 2);
                foreach(var c in left)
                {
                    if (right.Contains(c))
                    {
                        score1 += Score(c);
                        if (!both.Contains(c))
                            both.Add(c);
                        break;
                    }
                }
            }
            Console.WriteLine("score1 = " + score1);
        }
        static int Score(char c)
        {
            if (c >= 'A' && c <= 'Z')
                return c - 'A' + 27;
            else
                return c - 'a' + 1;
        }
    }
}
