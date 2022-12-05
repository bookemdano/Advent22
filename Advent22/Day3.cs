using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day3
    {
        public enum ShapeEnum
        {
            None,
            Rock,
            Paper,
            Scissors
        }
        public enum ResultEnum
        {
            None,
            Loss,
            Draw,
            Win
        }
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
        static ResultEnum Slow(ShapeEnum them, ShapeEnum you)
        {
            if (you == ShapeEnum.Rock && them == ShapeEnum.Rock)
                return ResultEnum.Draw;
            if (you == ShapeEnum.Paper && them == ShapeEnum.Rock)
                return ResultEnum.Win;
            if (you == ShapeEnum.Scissors && them == ShapeEnum.Rock)
                return ResultEnum.Loss;

            if (you == ShapeEnum.Rock && them == ShapeEnum.Paper)
                return ResultEnum.Loss;
            if (you == ShapeEnum.Paper && them == ShapeEnum.Paper)
                return ResultEnum.Draw;
            if (you == ShapeEnum.Scissors && them == ShapeEnum.Paper)
                return ResultEnum.Win;

            if (you == ShapeEnum.Rock && them == ShapeEnum.Scissors)
                return ResultEnum.Win;
            if (you == ShapeEnum.Paper && them == ShapeEnum.Scissors)
                return ResultEnum.Loss;
            if (you == ShapeEnum.Scissors && them == ShapeEnum.Scissors)
                return ResultEnum.Draw;

            return ResultEnum.None;
        }
    }
}
