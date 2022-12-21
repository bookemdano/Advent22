using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using static Advent22.Days.Day17;

namespace Advent22.Days
{
    internal class Day17
    {

        static public void Run()
        {
            //Day1();
            Day2();
        }
        public enum ShapeTypeEnum
        {
            Bar,
            Line,
            Box,
            Plus,
            Ell,
        }
        public class BaseShape
        {
            public BaseShape()
            {

            }
            public BaseShape(BaseShape other)
            {
                Points = other.Points;
            }
            public BaseShape(HashSet<(int x, int y)> points)
            {
                Points = points;
            }
            public override string ToString()
            {
                return $"{Points.Count}";
            }

            public HashSet<(int x, int y)> Points;

            public int Height
            {
                get
                {
                    return Points.Max(p => p.y) - Points.Min(p => p.y) + 1;
                }
            }

            static public BaseShape Bar()
            {
                return new BaseShape(new HashSet<(int, int)> { (0, 0), (1, 0), (2, 0), (3, 0) });
            }
            static public BaseShape Plus()
            {
                return new BaseShape(new HashSet<(int, int)> { (1, 0), (1, 1), (1, 2), (0, 1), (2, 1) });
            }
            static public BaseShape Ell()
            {
                return new BaseShape(new HashSet<(int, int)> { (2, 0), (2, 1), (2, 2), (0, 2), (1, 2), (2, 2) });
            }
            static public BaseShape Line()
            {
                return new BaseShape(new HashSet<(int, int)> { (0, 0), (0, 1), (0, 2), (0, 3) });
            }
            static public BaseShape Box()
            {
                return new BaseShape(new HashSet<(int, int)> { (0, 0), (0, 1), (1, 0), (1, 1) });
            }
        }
        public class Shape : BaseShape
        {
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }

            public Shape(BaseShape baseShape, int x, int y) : base(baseShape)
            {
                OffsetX = x;
                OffsetY = y;
            }

            public override string ToString()
            {
                return $"{base.ToString()} {OffsetX},{OffsetY}";
            }
            public HashSet<(int x, int y)> AbsPoints(int offsetX = 0, int offsetY = 0)
            {
                return Points.Select(p => (p.x + OffsetX + offsetX, p.y + OffsetY + offsetY)).ToHashSet();
            }
            internal bool CollidesWith(HashSet<(int x, int y)> restingPts, int offsetX, int offsetY)
            {
                return AbsPoints(offsetX, offsetY).Overlaps(restingPts);
            }
            internal bool CollidesWith(int ptX, int ptY)
            {
                return Points.Contains((ptX - OffsetX, ptY - OffsetY));
            }

            public int Left
            {
                get
                {
                    return Points.Min(p => p.x) + OffsetX;
                }
            }
            public int Right
            {
                get
                {
                    return Points.Max(p => p.x) + OffsetX;
                }
            }
            public int Top
            {
                get
                {
                    return Points.Min(p => p.y) + OffsetY;
                }
            }
            public int Bottom
            {
                get
                {
                    return Points.Max(p => p.y) + OffsetY;
                }
            }
        }
        internal class Board
        {
            int _maxLook = 14;

            public Board(int maxLook)
            {
                _maxLook = maxLook;
            }
            static BaseShape[] _shapes = new BaseShape[] { BaseShape.Bar(), BaseShape.Plus(), BaseShape.Ell(), BaseShape.Line(), BaseShape.Box() };
            //public List<Shape> RestingShapes { get; set; } = new List<Shape>();
            public HashSet<(int x, int y)> RestingShape { get; set; } = new HashSet<(int, int)>();
            public int RestingCount { get; set; }

            int _floor = 4;
            int _ceiling = 0;
            internal int TopRow()
            {
                //if (!RestingShapes.Any())
                // return _floor;
                //return RestingShapes.Min(s => s.ULCorner.Y);
                if (!RestingShape.Any())
                    return _floor;
                return RestingShape.Min(s => s.y);
            }
            internal Shape NewShape(int iShape)
            {
                var baseShape = _shapes[iShape];
                _ceiling = TopRow() - 4 - baseShape.Height + 1;
                return new Shape(baseShape, 2, _ceiling);
            }

            internal void Blow(Shape fallingShape, char c)
            {
                if (OnBoard(fallingShape, CharToX(c), 0))
                    fallingShape.OffsetX += CharToX(c);
            }
            int _maxLooked = 0;
            internal bool Fall(Shape fallingShape)
            {
                if (OnBoard(fallingShape, 0, 1))
                {
                    fallingShape.OffsetY++;
                    if (fallingShape.OffsetY - _ceiling > _maxLooked)
                        _maxLooked = fallingShape.OffsetY - _ceiling;
                    if (fallingShape.OffsetY > _ceiling + _maxLook)
                        Helper.Log("Too deep");
                    return true;
                }
                return false;
            }
            internal bool OnBoard(Shape shape, int offsetX, int offsetY)
            {
                if (shape.Left + offsetX < 0 || shape.Right + offsetX > 6)
                    return false;
                if (shape.Bottom + offsetY >= _floor)
                    return false;

                if (shape.CollidesWith(RestingShape, offsetX, offsetY))
                    return false;
                return true;
            }
            public void Draw(Shape fallingShape)
            {
                for (int y = _ceiling; y < _floor; y++)
                {
                    var line = Math.Abs(y % 100).ToString("00") + "|";
                    for (int x = 0; x < 7; x++)
                    {
                        if (fallingShape?.CollidesWith(x, y) == true)
                            line += "@";
                        else if (RestingShape.Contains((x, y)))
                            line += "#";
                        else
                            line += '.';
                    }
                    line += "|";
                    Helper.Log(line);
                }
                Helper.Log("  +-------+");
            }

            internal int TowerHeight()
            {
                return Math.Abs(TopRow() - _floor);
            }
            internal void AddToResting(Shape shape)
            {
                RestingCount++;
                RestingShape.UnionWith(shape.AbsPoints());
                if (RestingCount % _maxLook == 0)
                {
                    var newHash = new HashSet<(int x, int y)>();
                    foreach (var pt in RestingShape)
                    {
                        if (pt.y < _ceiling + _maxLook)
                            newHash.Add((pt.x, pt.y));
                    }
                    RestingShape = newHash;
                }
            }
            static int CharToX(char c)
            {
                if (c == '<')
                    return -1;
                else //if (c == '>')
                    return 1;
            }
            internal void MegaBlow(Shape shape, string nextChars)
            {
                int offsetX = 0;
                foreach (var c in nextChars)
                {
                    var newOffsetX = offsetX + CharToX(c);
                    if (shape.Left + newOffsetX >= 0 && shape.Right + newOffsetX <= 6)
                        offsetX = newOffsetX;
                }

                shape.OffsetX += offsetX;
                shape.OffsetY += nextChars.Length;
            }

            internal void LookForPatterns()
            {
                // TODO could use ulong instead of string
                var hashSet = new HashSet<string>();
                int windowSize = 100;
                int i = 0;
                for (int y = _floor; y >= _ceiling - windowSize; y--)
                {
                    var str = "";
                    for (int j = 0; j < windowSize; j++)
                    {
                        if (y - j < _ceiling)
                        {
                            str = y.ToString() + str;
                            break;
                        }
                        for (int x = 0; x < 7; x++)
                        {
                            if (RestingShape.Contains((x, y - j)))
                                str += '#';
                            else
                                str += '.';
                        }
                    }
                    if (hashSet.Contains(str))
                    {
                        var j = 0;
                        foreach (var s in hashSet)
                        {
                            if (s.CompareTo(str) == 0)
                                Helper.Log("Dup!!! " + i + " of " + j + " " + (i - j));   // + " " + str);
                            j++;
                        }
                    }
                    i++;
                    hashSet.Add(str);
                }
            }

            internal void IsFamiliar(Shape fallingShape)
            {
                throw new NotImplementedException();
            }

            static internal string HashToString(HashSet<(int x, int y)> hashSet)
            {
                var rv = "";
                var minX = hashSet.Min(p => p.x);
                var minY = hashSet.Min(p => p.y);
                var maxX = hashSet.Max(p => p.x);
                var maxY = hashSet.Max(p => p.y);
                for (int y = minY; y <= maxY; y++)
                    for (int x = minX; x <= maxX; x++)
                    {
                        if (hashSet.Contains((x, y)))
                            rv += "#";
                        else
                            rv += ".";
                    }
                return rv;
            }

            internal void Update(HashSet<(int x, int y)> endRestingShape, int rowDelta, int shapeDelta)
            {
                RestingShape = new HashSet<(int x, int y)>();
                foreach (var pt in endRestingShape)
                    RestingShape.Add((pt.x, pt.y + rowDelta));
                RestingCount += shapeDelta;
            }
        }


        static void Day1()
        {
            var fake = false;
            var filename = "Day17.txt";
            var target = 2022;
            var maxLook = 100;
            if (fake == true)
            {
                filename = "DayFake17.txt";
                maxLook = 14;
            }

            var input = File.ReadAllText(filename);
            var iShape = 0;
            Shape fallingShape = null;
            var board = new Board(maxLook);
            var ichar = 0;
            var baseShapes = 5;
            var sw = Stopwatch.StartNew();
            bool drawAlot = false;
            bool drawSome = false;
            int ceilingClearance = 3;
            while (board.RestingCount < target)
            {
                if (fallingShape == null)
                {
                    fallingShape = board.NewShape(iShape++ % baseShapes);
                    if (drawSome)
                        board.Draw(fallingShape);
                    if (ichar % input.Length + ceilingClearance < input.Length)
                    {
                        // if we are close to the end of the input then skip this
                        var next = input.Substring(ichar % input.Length, ceilingClearance);
                        board.MegaBlow(fallingShape, next);
                        ichar += ceilingClearance;
                        if (drawSome)
                            board.Draw(fallingShape);
                    }
                }
                else
                {
                    //Helper.Log($"Move {fallingShape} {input[ichar % input.Length]}({ichar})");
                    board.Blow(fallingShape, input[ichar++ % input.Length]);
                    if (!board.Fall(fallingShape))
                    {
                        board.AddToResting(fallingShape);
                        fallingShape = null;
                    }
                }
                if (drawAlot)
                    board.Draw(fallingShape);
            }
            Helper.Log(sw.Elapsed);
            board.Draw(null);
            Helper.Log("Star1 Score: " + board.TowerHeight());
        }
        public class Result
        {
            // key
            public string NextChars { get; set; }
            public string RelativeRestingShape { get; set; }
            public BaseShape Shape { get; set; }

            // needed for relative
            public int CurrentChar { get; set; }
            public int CurrentTopRow { get; internal set; }

            // target state
            public int CharsDelta { get; set; }
            public HashSet<(int x, int y)> EndRestingShape { get; internal set; }

            public int Shapes { get; set; } = 1;

            internal bool Same(Result other)
            {
                if (RelativeRestingShape != other.RelativeRestingShape)
                    return false;
                if (NextChars != other.NextChars)
                    return false;
                if (Shape.Points != other.Shape.Points)
                    return false;
                return true;
            }
        }
        // like substring but loops around
        static string NextChars(string source, int ichar, int len)
        {

            if (ichar % source.Length + len <= source.Length)
                return source.Substring(ichar % source.Length, len);
            var str = source.Substring(ichar % source.Length);
            str += source.Substring(0, len - str.Length);
            return str;
        }
        static void Day2()
        {
            var fake = true;
            var filename = "Day17.txt";
            var target = 1E12;
            var maxLook = 47;
            if (fake == true)
            {
                filename = "DayFake17.txt";
                //target = 2022;
                //loop = 159;
                //offset = 26;
                maxLook = 14;
                //remainder = (target - offset) / target;
            }
            var input = File.ReadAllText(filename);
            Shape fallingShape = null;
            var board = new Board(maxLook);
            var ichar = 0;
            var baseShapes = 5;
            var sw = Stopwatch.StartNew();
            bool drawAlot = false;
            bool drawSome = false;
            int ceilingClearance = 3;
            Result lastResult = null;
            var results = new List<Result>();
            while (board.RestingCount < target)
            {
                if (fallingShape == null)
                {
                    if (lastResult != null)
                    {
                        lastResult.CharsDelta = ichar - lastResult.CurrentChar;
                        lastResult.EndRestingShape = new HashSet<(int, int)>(board.RestingShape);
                        results.Add(lastResult);
                    }
                    fallingShape = board.NewShape(board.RestingCount % baseShapes);
                    if (board.RestingCount % 100000 == 0)
                        Helper.Log("RestingCount " + board.RestingCount + " " + board.TowerHeight());


                    if (board.RestingCount > maxLook)
                    {
                        lastResult = new Result();
                        lastResult.NextChars = NextChars(input, ichar, maxLook);
                        lastResult.Shape = new BaseShape(fallingShape);
                        lastResult.CurrentChar = ichar;
                        lastResult.CurrentTopRow = board.TopRow();
                        lastResult.RelativeRestingShape = Board.HashToString(board.RestingShape);
                        var found = results.FirstOrDefault(r => r.Same(lastResult));
                        if (found != null)
                        {
                            if (found.Shapes == 1)
                            {
                                var jump = new Result();
                                jump.NextChars = found.NextChars;
                                jump.Shape = found.Shape;
                                jump.RelativeRestingShape = found.RelativeRestingShape;
                                jump.CurrentChar = found.CurrentChar;
                                jump.CurrentTopRow = found.CurrentTopRow;

                                var last = results.Last();
                                jump.CharsDelta = last.CurrentChar - jump.CurrentChar;
                                jump.EndRestingShape = new HashSet<(int, int)>(last.EndRestingShape);
                                results.Insert(0, jump);
                                jump.Shapes = results.Count() - results.IndexOf(found);
                                found = jump;
                            }

                            ichar += found.CharsDelta;
                            board.Update(found.EndRestingShape, board.TopRow() - found.CurrentTopRow, found.Shapes);

                            /*
                            // assume we are on a roll
                            var start = results.IndexOf(found);
                            for(int iResult = start + 1; iResult < results.Count(); iResult++)
                            {
                                ichar += results[iResult].CharsDelta;
                                board.Update(results[iResult].EndRestingShape, board.TopRow() - results[iResult].CurrentTopRow);
                                if (board.RestingCount % 1000000 == 0)
                                    Helper.Log("RestingCount " + board.RestingCount + " " + board.TowerHeight());
                            }
                            */
                            lastResult = null;
                            fallingShape = null;
                            // we are all done with this shape, go to the next one
                            continue;
                        }
                    }
                    else
                        lastResult = null;

                    if (drawSome)
                        board.Draw(fallingShape);

                    var next = NextChars(input, ichar, ceilingClearance);
                    board.MegaBlow(fallingShape, next);
                    ichar += ceilingClearance;
                    if (drawSome)
                        board.Draw(fallingShape);
                }
                else
                {
                    //Helper.Log($"Move {fallingShape} {input[ichar % input.Length]}({ichar})");
                    board.Blow(fallingShape, input[ichar++ % input.Length]);
                    if (!board.Fall(fallingShape))
                    {
                        if (drawSome)
                            Helper.Log("TopRow " + board.RestingCount + " " + board.TowerHeight());
                        board.AddToResting(fallingShape);
                        fallingShape = null;
                        //if (board.RestingCount % 100 == 0)
                        //    board.LookForPatterns();
                    }
                }
                if (drawAlot)
                    board.Draw(fallingShape);
            }
            Helper.Log(sw.Elapsed);
            board.Draw(null);
            Helper.Log("Star2 Score: " + board.TowerHeight());
        }

    }
}

