using Advent22.Days;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Schema;
using static Advent22.Day17;

namespace Advent22
{
    internal class Day17
    {
        static public void Run()
        {
            Day1();
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
            public BaseShape(BaseShape other)
            {
                Width = other.Width;
                Height = other.Height;
                ShapeType = other.ShapeType;
            }
            public BaseShape(int width, int height, ShapeTypeEnum shapeType)
            {
                Width = width;
                Height = height;
                ShapeType = shapeType;
            }
            public BaseShape(ShapeTypeEnum shapeType)
            {
                Width = 3;
                Height = 3;
                ShapeType = shapeType;
            }

            public ShapeTypeEnum ShapeType { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            static public BaseShape Bar()
            {
                return new BaseShape(4, 1, ShapeTypeEnum.Bar);
            }
            static public BaseShape Plus()
            {
                return new BaseShape(ShapeTypeEnum.Plus);
            }
            static public BaseShape Ell()
            {
                return new BaseShape(ShapeTypeEnum.Ell);
            }
            static public BaseShape Line()
            {
                return new BaseShape(1, 4, ShapeTypeEnum.Line);
            }
            static public BaseShape Box()
            {
                return new BaseShape(2, 2, ShapeTypeEnum.Box);
            }

            public bool Full
            {
                get
                {
                    return (int)ShapeType < 3;
                }
            }
            public override string ToString()
            {
                return ShapeType.ToString();
            }
        }
        public class Shape : BaseShape
        {
            public Shape(BaseShape other, BasePoint pt) : base(other)
            {
                ULCorner = pt;
            }

            internal BasePoint ULCorner { get; set; } = new BasePoint(2, 0);

            public override string ToString()
            {
                return $"{base.ToString()}@{ULCorner}";
            }
            internal Shape IfBlow(char c)
            {
                var x = ULCorner.X;
                if (c == '<')
                    x--;
                else if (c == '>')
                    x++;
                return new Shape(this, new BasePoint(x, ULCorner.Y));
            }

            internal Shape IfFall()
            {
                return new Shape(this, new BasePoint(ULCorner.X, ULCorner.Y + 1));
            }
            internal bool CollidesWith(HashSet<(int x, int y)> restingPts)
            {
                return GetSimplePoints().Overlaps(restingPts);
            }

            internal bool CollidesWith(Shape other)
            {
                //if (!Full && other.Full)
                //    return other.CollidesWith(this);    // switch so we only have to check one way
                if (Math.Abs(ULCorner.Y - other.ULCorner.Y) > 4) 
                    return false;
                
                //var overlapX = (other.Left <= Right && Right <= other.Right) || (other.Left >= Left && Left <= other.Left);
                //var overlapY = (other.Bottom <= Bottom && Bottom <= other.Top) || (other.Bottom >= Top && Top >= other.Top);
                //var overlap = overlapX && overlapY;
                //if (!overlap)
                //    return false;

                //if (Full && other.Full)
                //    return overlap;
                //else
                {
                    var otherPts = other.GetPoints();
                    var pts = GetPoints();
                    foreach (var pt in pts)
                        if (otherPts.Any(p => p.Same(pt)))
                            return true;
                }
                return false;
            }
            internal List<BasePoint> GetPoints()
            {
                //Debug.Assert(!Full, "Shouldn't be here with type " + ShapeType);
                var rv = new List<BasePoint>();
                if (ShapeType == ShapeTypeEnum.Plus)
                {
                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            if (x == 1 || y == 1)
                                rv.Add(new BasePoint(ULCorner.X + x, ULCorner.Y + y));
                }
                else if (ShapeType == ShapeTypeEnum.Ell)
                {
                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            if (x == 2 || y == 2)
                                rv.Add(new BasePoint(ULCorner.X + x, ULCorner.Y + y));
                }
                else 
                {
                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            rv.Add(new BasePoint(ULCorner.X + x, ULCorner.Y + y));
                }
                return rv;
            }
            internal bool CollidesWith(int ptX, int ptY)
            {
                // we already know they overlap
                var x = ptX - ULCorner.X;
                var y = ptY - ULCorner.Y;
                var overlap = x >= 0 && x < Width && y >= 0 && y < Height;
                if (!overlap)
                    return false;

                if (Full)
                    return overlap;
                if (ShapeType == ShapeTypeEnum.Plus)
                    return (x == 1 || y == 1);
                else if (ShapeType == ShapeTypeEnum.Ell)
                    return (x == 2 || y == 2);
                return true;
            }

            public int Left
            {
                get
                {
                    return ULCorner.X;
                }
            }
            public int Right
            {
                get
                {
                    return ULCorner.X + Width - 1;
                }
            }
            public int Top
            {
                get
                {
                    return ULCorner.Y;
                }
            }
            public int Bottom
            {
                get
                {
                    return ULCorner.Y + Height - 1;
                }
            }

            internal HashSet<(int x, int y)> GetSimplePoints()
            {
                var pts = GetPoints();
                var rv = new HashSet<(int, int)>();
                foreach (var pt in pts) 
                    rv.Add((pt.X, pt.Y));
                return rv;
            }
        }
        internal class Board
        {
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
                return new Shape(baseShape, new BasePoint(2, _ceiling));
            }

            internal void Blow(Shape fallingShape, char c)
            {
                var newShape = fallingShape.IfBlow(c);
                if (OnBoard(newShape))
                    fallingShape.ULCorner = newShape.ULCorner;
            }
            internal bool Fall(Shape fallingShape)
            {
                var newShape = fallingShape.IfFall();
                if (OnBoard(newShape))
                {
                    fallingShape.ULCorner = newShape.ULCorner;
                    return true;
                }
                return false;
            }
            internal bool OnBoard(Shape shape)
            {
                if (shape.Left < 0 || shape.Right > 6)
                    return false;
                if (shape.Bottom >= _floor)
                    return false;

                if (shape.CollidesWith(RestingShape))
                    return false;
                //foreach (var restingShape in RestingShapes)
                //{
                //   if (shape.CollidesWith(restingShape))
                //        return false;
                //}
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
                //RestingShapes.Add(shape);
                var pts = shape.GetSimplePoints();
                foreach(var pt in pts)
                    RestingShape.Add((pt.x, pt.y));
            }
        }
        

        static void Day1()
        {
            var input = File.ReadAllText("DayFake17.txt");
            var iShape = 0;
            Shape fallingShape = null;
            var board = new Board();
            var ichar = 0;
            var baseShapes = 5;
            var target = 2022;
            
            while (board.RestingCount < target)
            {
                if (fallingShape == null)
                {
                    fallingShape = board.NewShape(iShape++ % baseShapes);
                    //board.Draw(fallingShape);
                }
                else
                {
                    //Helper.Log($"Move {fallingShape} {input[ichar % input.Length]}({ichar})");
                    board.Blow(fallingShape, input[ichar++ % input.Length]);
                    if (!board.Fall(fallingShape))
                    {
                        board.AddToResting(new Shape(fallingShape, fallingShape.ULCorner));
                        fallingShape = null;
                    }
                }
                //board.Draw(fallingShape);
            }
            board.Draw(null);
            Helper.Log("Star Score: " + board.TowerHeight());
        }

        static void Day2()
        {
            var input = File.ReadAllText("DayFake17.txt");
            var iShape = 0;
            Shape fallingShape = null;
            var board = new Board();
            var ichar = 0;
            var baseShapes = 5;
            var target = 1E12;
            var loop = baseShapes * input.Length;
            
            while (board.RestingCount < target)
            {
                if (fallingShape == null)
                {
                    fallingShape = board.NewShape(iShape++ % baseShapes);
                    //board.Draw(fallingShape);
                }
                else
                {
                    //Helper.Log($"Move {fallingShape} {input[ichar % input.Length]}({ichar})");
                    board.Blow(fallingShape, input[ichar++ % input.Length]);
                    if (!board.Fall(fallingShape))
                    {
                        board.AddToResting(new Shape(fallingShape, fallingShape.ULCorner));
                        fallingShape = null;
                    }
                }
                //board.Draw(fallingShape);
            }
            board.Draw(null);
            Helper.Log("Star Score: " + board.TowerHeight());
        }
    }
}

