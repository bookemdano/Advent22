using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day4
    {
        static public void Run()
        {
            var lines = File.ReadAllLines("Day4-input.txt");
            //lines = new string[] { "2-4,6-8", "2-3,4-5", "5-7,7-9", "2-8,3-7", "6-6,4-6", "2-6,4-8" };
            var score1 = 0;
            var score2 = 0;
            foreach (var line in lines)
            {
                var groups = line.Split(',');
                var first = groups[0].Split('-');
                var second = groups[1].Split('-');
                var firstParts = new int[] { int.Parse(first[0]), int.Parse(first[1]) };
                var secondParts = new int[] { int.Parse(second[0]), int.Parse(second[1]) };

                if (firstParts[0] <= secondParts[0] && firstParts[1] >= secondParts[1])
                {
                    score1++;
                    score2++;
                }
                else if (firstParts[0] >= secondParts[0] && firstParts[1] <= secondParts[1])
                {
                    score1++;
                    score2++;
                }
                else if (firstParts[1] >= secondParts[0] && firstParts[1] <= secondParts[1])
                    score2++;
                else if (firstParts[0] >= secondParts[0] && firstParts[0] <= secondParts[1])
                    score2++;
            }
            Console.WriteLine("score1 = " + score1);
            Console.WriteLine("score2 = " + score2);
        }
    }
}
