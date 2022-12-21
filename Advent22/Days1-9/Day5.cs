using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day5
    {
        static public void Run()
        {
            var lines = File.ReadAllLines("Day5-input.txt");
            //lines = new string[] { "    [D]    ","[N] [C]    ","[Z] [M] [P]"," 1   2   3 ","","move 1 from 2 to 1","move 3 from 1 to 3","move 2 from 2 to 1","move 1 from 1 to 2"};
            var instacks = true; 
            var stacks = new Dictionary<int, string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith(" 1"))
                {
                    instacks = false;
                    continue;
                }

                if (instacks)
                {
                    for(int i = 1; i < line.Length; i+=4) 
                    {
                        var s = ((i - 1) / 4) + 1;
                        if (!stacks.ContainsKey(s))
                            stacks[s] = "";
                        if (line[i] != ' ')
                            stacks[s] = line[i] + stacks[s];
                    }
                }
                else
                {
                    var parts = line.Split(' ');
                    var counts = int.Parse(parts[1]);
                    var from = int.Parse(parts[3]);
                    var to = int.Parse(parts[5]);
                    var moveds = stacks[from].Substring(stacks[from].Length - counts, counts);
                    stacks[from] = stacks[from].Substring(0, stacks[from].Length - counts);
                    stacks[to] = stacks[to] + moveds; //Reverse(moveds);    // reverse for part 1

                }
            }
            var score1 = "";
            foreach(var stack in stacks)
            {
                score1 += stack.Value.Last();
            }
            Console.WriteLine("score1 = " + score1);
        }
        static string Reverse(string str)
        {
            var rv = "";
            foreach (var c in str.Reverse())
                rv += c;
            return rv;
        }
    }
}

