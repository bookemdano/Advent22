using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace Advent22
{
    internal class Day8
    {
        static public void Run()
        {
            var input = File.ReadAllLines("Day8-input.txt");
            /*input = new string[] {
                "30373",
                "25512",
                "65332",
                "33549",
                "35390"
            };*/
            var size = input.Length;
            var score1 = 0;
            var score2 = 0;
            var s1 = ScenicScore(input, 2, 1);
            var s2 = ScenicScore(input, 2, 3);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    var blocked = true;
                    if (!BlockedLeft(input, x, y))
                        blocked = false;
                    if (!BlockedRight(input, x, y))
                        blocked = false;
                    if (!BlockedUp(input, x, y))
                        blocked = false;
                    if (!BlockedDown(input, x, y))
                        blocked = false;
                    if (blocked == false)
                    {
                        score1++;
                        var scenicScore = ScenicScore(input, x, y);
                        if (scenicScore > score2)
                            score2 = scenicScore;
                    }
                }
            }
            Console.WriteLine("Score1 = " + score1);
            Console.WriteLine("Score2 = " + score2);
        }
        static int ScenicScore(string[] input, int x, int y)
        {
            return ScoreLeft(input, x, y) *
                ScoreRight(input, x, y) *
                ScoreUp(input, x, y) *
                ScoreDown(input, x, y);
        }
        static int ScoreLeft(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (x == 0)
                return 0;
            var score = 0;
            for (int iX = x-1; iX >= 0; iX--)
            {
                score++;
                if (Get(input, iX, y) >= h)
                    break;
            }
            return score;
        }
        static int ScoreRight(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (x == input.Length - 1)
                return 0;
            var score = 0;
            for (int iX = x + 1; iX < input.Length; iX++)
            {
                score++;
                if (Get(input, iX, y) >= h)
                    break;
            }
            return score;
        }
        static int ScoreUp(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (y == 0)
                return 0;
            var score = 0;
            for (int iY = y-1; iY >= 0; iY--)
            {
                score++;
                if (Get(input, x, iY) >= h)
                    break;
            }
            return score;
        }
        static int ScoreDown(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (y == input.Length - 1)
                return 0;
            var score = 0;
            for (int iY = y + 1; iY < input.Length; iY++)
            {
                score++;
                if (Get(input, x, iY) >= h)
                    break;
            }
            return score;
        }

        static bool BlockedLeft(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (x == 0)
                return false;
            for (int iX = 0; iX < x; iX++)
            {
                if (Get(input, iX, y) >= h)
                    return true;
            }
            return false;
        }
        static bool BlockedRight(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (x == input.Length - 1)
                return false;
            for (int iX = x + 1; iX < input.Length; iX++)
            {
                if (Get(input, iX, y) >= h)
                    return true;
            }
            return false;
        }
        static bool BlockedUp(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (y == 0)
                return false;
            for (int iY = 0; iY < y; iY++)
            {
                if (Get(input, x, iY) >= h)
                    return true;
            }
            return false;
        }
        static bool BlockedDown(string[] input, int x, int y)
        {
            var h = Get(input, x, y);
            if (y == input.Length - 1)
                return false;
            for (int iY = y + 1; iY < input.Length; iY++)
            {
                if (Get(input, x, iY) >= h)
                    return true;
            }
            return false;
        }
        static int Get(string[] input, int x, int y)
        {
            return input[y][x] - '0';
        }
    }

}

