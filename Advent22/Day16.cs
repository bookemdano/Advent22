using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using static Advent22.Days.Day14;
using static System.Net.Mime.MediaTypeNames;

namespace Advent22
{
    internal class Day16
    {
        static public void Run()
        {
            Day1();
            Day2();
        }
        static void Day1()
        {
            var input = File.ReadAllLines("DayFake15.txt");
            var score = 0;
            Helper.Log("Star1 Score: " + score); 
        }
        static void Day2()
        {
            var input = File.ReadAllLines("DayFake15.txt");
            var score = 0;
            Helper.Log("Star2 Score: " + score);
        }
    }
}

