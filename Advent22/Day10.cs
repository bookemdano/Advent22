using System.Runtime.CompilerServices;

namespace Advent22
{
    internal class Day10
    {
        static public void Run()
        {
            var input = File.ReadAllLines("Day10fake.txt");
            var score1 = 0;
            var score2 = 0;
            var cycle = 1;
            var x = 1;
            var spriteStart = 0;
            var spriteEnd = 2;
            var stopAt = 20;
            var endRow =20;
            var outlines = new List<string>();
            for (int i = 0; i < 6; i++)
                outlines.Add(new string('.', 40));
            var drawPixel = 0;
            int iLine = 0;
            foreach (var line in input)
            {
                if (drawPixel > endRow)
                {
                    endRow += 40;
                    //drawPixel = endRow;
                    iLine++;
                    drawPixel -= 40;
                    //x += endRow;
                    //spriteStart = x - 1;
                    //spriteEnd = x + 1;

                }

                if (line == "noop")
                {
                    if (cycle >= spriteStart && cycle <= spriteEnd)
                        outlines[iLine] = LightChar(outlines[iLine], drawPixel);
                    cycle++;
                    drawPixel++;
                }
                else
                {
                    var parts = line.Split(' ');
                    var add = int.Parse(parts[1]);
                    // cycle 1
                    cycle++;
                    if (drawPixel >= spriteStart && drawPixel <= spriteEnd)
                        outlines[iLine] = LightChar(outlines[iLine], drawPixel);
                    drawPixel++;

                    //cycle 2
                    cycle++;
                    if (drawPixel >= spriteStart && drawPixel <= spriteEnd)
                        outlines[iLine] = LightChar(outlines[iLine], drawPixel);
                    drawPixel++;

                    x += add;
                    spriteStart = x - 1;
                    spriteEnd = x + 1;
                }

            }

            foreach(var outline in outlines)
                Console.WriteLine(outline);

            Console.WriteLine("Score1 = " + score1);
            Console.WriteLine("Score2 = " + score2);
        }
        static string LightChar(string str, int i)
        {
            var chars = str.ToCharArray();
            chars[i] = '#';
            return new string(chars);
        }
    }
}

