using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day7
    {
        static public void Run()
        {
            var input = File.ReadAllLines("Day7-input.txt");
            //input = new string[] { "$ cd /", "$ ls", "dir a", "14848514 b.txt", "8504156 c.dat", "dir d", "$ cd a", "$ ls", "dir e", "29116 f", "2557 g", "62596 h.lst", "$ cd e", "$ ls", "584 i", "$ cd ..", "$ cd ..", "$ cd d", "$ ls", "4060174 j", "8033020 d.log", "5626152 d.ext", "7214296 k"};

            var sizes = new Dictionary<string, int>();
            var dir = new Stack<string>();
            var score = 0;
            foreach(var line in input)
            {
                if (line.StartsWith("$"))
                {
                    if (line.StartsWith("$ cd"))
                    {
                        var moveTo = line.Substring(5, line.Length - 5);
                        if (moveTo == "..")
                            dir.Pop();
                        else
                            dir.Push(moveTo);
                    }
                    continue;
                }
                if (char.IsDigit(line[0]))
                {
                    var parts = line.Split(' ');
                    var size = int.Parse(parts[0]);
                    var key = string.Join(',', dir.ToArray().Reverse());
                    while (key != null)
                    {
                        if (!sizes.ContainsKey(key))
                            sizes.Add(key, 0);
                        sizes[key] += size;
                        key = GetParent(key);
                    }
                }
            }
            foreach (var size in sizes.Values)
            {
                if (size <= 100000)
                    score += size;
            }
            var total = sizes.First().Value;
            var free = 7E7 - total;
            var needed = 30000000 - free;
            var vals = sizes.Values.Where(v => v > needed).ToArray();
            var min = vals.Min();

            Console.WriteLine("Score = " + score);
            Console.WriteLine("Score2 = " + min); // not 42536714
        }
        static string GetParent(string str)
        {
            var parts = str.Split(',');
            if (parts.Length == 1)
                return null;
            return string.Join(',', parts, 0, parts.Length - 1);
        }
    }

}

