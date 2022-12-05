using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent22
{
    internal class Day1
    {
        static public void Run()
        {
            // Day 1- lower memory
            var lines = File.ReadAllLines("Day1-input.txt");
            var counts = new int[3];
            var min = 0;
            var minI = 0;
            var sum = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    if (sum > min)
                    {
                        counts[minI] = sum;
                        min = sum;
                        for (int i = 0; i < 3; i++)
                        {
                            if (counts[i] < min)
                            {
                                min = counts[i];
                                minI = i;
                            }
                        }
                    }
                    sum = 0;
                }
                else
                    sum += int.Parse(line);
            }
            if (sum > min)
                counts[minI] = sum;

            Console.WriteLine("Top = " + counts.Max());
            Console.WriteLine("Top3 = " + counts.Sum());
            //foreach (var count in counts.OrderDescending())
            //    Console.WriteLine(count);

        }

        static public void RunQuickBrute()
        {
            // Day 1- quick
            var lines = File.ReadAllLines("Day1-input.txt");
            var counts = new List<int>();
            var sum = 0;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    counts.Add(sum);
                    sum = 0;
                }
                else
                    sum += int.Parse(line);
            }

            counts.Add(sum);

            Console.WriteLine("Top = " + counts.OrderDescending().First());
            Console.WriteLine("Top3 = " + counts.OrderDescending().Take(3).Sum());
            //foreach (var count in counts.OrderDescending())
            //    Console.WriteLine(count);

        }
    }
}
