using System.Runtime.CompilerServices;

namespace Advent22
{
    internal class Day10
    {
        static char _light = '█';
        static char _dark = ' ';
        static public void Run()
        {
            Star1();
            Star2();
        }
        static public void Star2()
        {
            var input = File.ReadAllLines("Day10.txt");
            var sprite = 1;
            var outline = new string(_dark, 240);
            var pixel = 0;
            var nextRow = 39;
            foreach (var line in input)
            {
                //Draw(outline, sprite, pixel);
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

                    //Draw(outline, sprite, pixel);
                    //cycle 2
                    outline = MaybeLight(outline, sprite, pixel);
                    pixel++;
                    sprite += add;
                }
            }

            Draw(outline, -1, -1); 
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
            if (line[sprite] == _light)
                replaceC = 'S';
            else //if (line[sprite] == _dark)
                replaceC = 's';
            line = ReplaceChar(line, sprite-1, replaceC);
            line = ReplaceChar(line, sprite, replaceC);
            line = ReplaceChar(line, sprite+1, replaceC);
            */
            if (pixel >= 0 && pixel < line.Length)
            {
                if (line[pixel] == _light || line[pixel] == 'S')
                    line = ReplaceChar(line, pixel, 'P');
                else
                    line = ReplaceChar(line, pixel, 'p');
            }

            Helper.Log($"Sprite:{sprite} Pixel:{pixel}");
            for (int i = 0; i < 6; i++) 
            {
                Helper.Log(line.Substring(i * 40, 40));
            }

        }
        static string LightChar(string str, int i)
        {
            return ReplaceChar(str, i, _light);
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
            var nextBreak = 20;
            var score = 0;
            foreach (var line in input)
            {
                var parts = line.Split(' ');
                for (int i = 0; i < parts.Count(); i++)
                {
                    if (++cycle >= nextBreak)
                    {
                        score += nextBreak * reg;
                        nextBreak += 40;
                    }
                }
                if (parts.Count() > 1)
                    reg += int.Parse(parts[1]);
            }
            Helper.Log("Score: " + score);
        }
    }
}

