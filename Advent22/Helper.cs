using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent22
{
    internal class Helper
    {
        static public void Log(string str)
        {
            Console.WriteLine(str);
            File.AppendAllText($"endless{DateTime.Today.ToString("yyyyMMdd")}.log", $"{DateTime.Now} {str}\n");
        }
    }
}
