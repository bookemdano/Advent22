using System.Runtime.CompilerServices;

namespace Advent22
{
    internal class Day10
    {
        static public void Run()
        {
            Star1();
            Star2();
        }
        static public void Star2()
        {
            var input = File.ReadAllLines("Day10.txt");
            var sprite = 1;
            var outline = new string('.', 240);
            var pixel = 0;
            var nextRow = 39;
            foreach (var line in input)
            {
                Draw(outline, sprite, pixel);
                if (pixel > nextRow)
                {
                    nextRow += 40;
                    sprite += 40;
                }
                if (line == "noop")
                {
                    outline = MaybeLight(outline, sprite, pixel);
                    pixel++;
                }
                else
                {
                    var parts = line.Split(' ');
                    var add = int.Parse(parts[1]);
                    // cycle 1
                    outline = MaybeLight(outline, sprite, pixel);
                    pixel++;

                    if (pixel > nextRow)
                    {
                        nextRow += 40;
                        sprite += 40;
                    }

                    Draw(outline, sprite, pixel);
                    //cycle 2
                    outline = MaybeLight(outline, sprite, pixel);
                    pixel++;
                    sprite += add;
                }
            }

            Draw(outline, sprite, pixel); // not BACEKLHF
        }
        static string MaybeLight(string line, int sprite, int pixel)
        {
            if (pixel >= sprite - 1 && pixel <= sprite + 1)
                return LightChar(line, pixel);
            else
                return line;

        }
        static void Draw(string line, int sprite, int pixel)
        {
            /*char replaceC;
            if (line[sprite] == '#')
                replaceC = 'S';
            else //if (line[sprite] == '.')
                replaceC = 's';
            line = ReplaceChar(line, sprite-1, replaceC);
            line = ReplaceChar(line, sprite, replaceC);
            line = ReplaceChar(line, sprite+1, replaceC);
            */
            if (pixel >= 0 && pixel < line.Length)
            {
                if (line[pixel] == '#' || line[pixel] == 'S')
                    line = ReplaceChar(line, pixel, 'P');
                else
                    line = ReplaceChar(line, pixel, 'p');
            }

            Console.WriteLine($"Sprite:{sprite} Pixel:{pixel}");
            for (int i = 0; i < 6; i++) 
            {
                Console.WriteLine(line.Substring(i * 40, 40));
            }

        }
        static string LightChar(string str, int i)
        {
            return ReplaceChar(str, i, '#');
        }
        static string ReplaceChar(string str, int i, char c)
        {
            if (i < 0)
                return str;
            var chars = str.ToCharArray();
            chars[i] = c;
            return new string(chars);
        }
        static public void Star1()
        {
            var input = File.ReadAllLines("Day10.txt");
            var reg = 1;
            var cycle = 0;
            var breakAt = 20;
            var score = 0;
            foreach (var line in input)
            {
                if (line == "noop")
                {
                    cycle++;
                    if (cycle >= breakAt)
                    {
                        score += breakAt * reg;
                        breakAt += 40;
                    }
                }
                else
                {
                    var parts = line.Split(' ');
                    var add = int.Parse(parts[1]);
                    // cycle 1
                    cycle++;
                    if (cycle >= breakAt)
                    {
                        score += breakAt * reg;
                        breakAt += 40;
                    }
                    cycle++;
                    if (cycle >= breakAt)
                    {
                        score += breakAt * reg;
                        breakAt += 40;
                    }
                    reg += add;
                }
            }
            //score += breakAt * reg;
            Console.WriteLine("Score: " + score);
        }
    }
}

