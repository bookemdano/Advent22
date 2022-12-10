using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day6
    {
        static public void Run()//2
        {
            var input = File.ReadAllText("Day6-input.txt");
            //input = "mjqjpqmgbljsphdztnvjfqwrcgsmlb";   //19
            //input = "bvwbjplbgvbhsrlpgdmjqwftvncz";   //23
            //input = "nppdvjthqldpwncqszvftbrmjlhg";   //6
            //input = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg";   //10
            //input = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw";   //11
            var score1 = 0;
            Console.WriteLine("score1 = " + score1);
            var window = "";
            var n = 14;
            for (int i = 0; i < input.Length; i++)
            {
                if (window.Length < n)
                    window += input[i];
                if (window.Length == n)
                {
                    var dup = false;
                    for (int j = 0; j < window.Length - 1; j++)
                    {
                        for (int k = j + 1; k < window.Length; k++)
                        {
                            if (window[k] == window[j])
                            {
                                dup = true;
                                break;
                            }
                        }
                        if (dup == true)
                            break;
                    }
                    if (dup == false)
                    {
                        score1 = i + 1;
                        break;
                    }
                    window = window.Substring(1, n-1);
                }
            }
            Console.WriteLine("Score = " + score1);
        }
        static public void Run1()
        {
            var input = File.ReadAllText("Day6-input.txt");
            //input = "mjqjpqmgbljsphdztnvjfqwrcgsmlb";   //7
            //input = "bvwbjplbgvbhsrlpgdmjqwftvncz";   //5
            //input = "nppdvjthqldpwncqszvftbrmjlhg";   //6
            //input = "nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg";   //10
            //input = "zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw";   //11
            var score1 = 0;
            Console.WriteLine("score1 = " + score1);
            var window = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (window.Length < 4)
                    window += input[i];
                if (window.Length == 4)
                {
                    var dup = false;
                    for (int j = 0; j < window.Length - 1; j++)
                    {
                        for (int k = j + 1; k < window.Length; k++)
                        {
                            if (window[k] == window[j])
                            {
                                dup = true;
                                break;
                            }
                        }
                        if (dup == true)
                            break;
                    }
                    if (dup == false)
                    {
                        score1 = i + 1;
                        break;
                    }
                    window = window.Substring(1, 3);
                }
            }
            Console.WriteLine("Score = " + score1);
        }
    }
}

