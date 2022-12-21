namespace Advent22
{
    internal class Day20
    {
        static bool _logAll = false;
        static bool _fake = false;
        static string[] _fakeRights = new string[]
        {
                "2, 1, -3, 3, -2, 0, 4",    //1
                "1, -3, 2, 3, -2, 0, 4",    //2
                "1, 2, 3, -2, -3, 0, 4",    //-3
                "1, 2, -2, -3, 0, 3, 4",    //3
                "1, 2, -3, 0, 3, 4, -2",    //-2
                "1, 2, -3, 0, 3, 4, -2",    //0
                "1, 2, -3, 4, 0, 3, -2",    //4

        };
        static public void Run()
        {
            //Day1();
            Day2();
        }
        public class BaseCode
        {
            public BaseCode(int originalIndex, long val)
            {
                OriginalIndex = originalIndex;
                Value = val;
            }

            public int OriginalIndex { get; set; }
            public long Value { get; set; }
            public override string ToString()
            {
                return $"{OriginalIndex}: {Value}";
            }
        }
        public class Code : BaseCode
        {
            public Code(int originalIndex, long val) : base(originalIndex, val)
            {
            }

            public Code Left { get; set; }
            public Code Right { get; set; }
            public bool Head { get; set; }
            public override string ToString()
            {
                var link = "";
                if (Left == null && Right == null)
                    link = "N";
                else if (Left == null)
                    link = "R";
                else if (Right == null)
                    link = "L";
                else
                    link = "D";
                return $"{base.ToString()} {link}";
            }
            internal Code FindLeft(int val)
            {
                if (val == 0)
                    return this;
                if (Left.Head)
                    val++;
                return Left.FindLeft(val - 1);
            }

            internal Code FindRight(int val)
            {
                if (val == 0)
                    return this;
                if (Right.Head)
                    val++;
                return Right.FindRight(val - 1);
            }

            internal void MoveLeft(int val)
            {
                if (val == 0)
                    return;

                // source fix
                var oldLeft = Left;
                var oldRight = Right;
                var newRight = FindLeft(val);
                var newLeft = newRight.Left;
                if (newLeft.Head)
                {
                    newLeft = newLeft.Left;
                    newRight = newLeft.Right;   // this will be the 0
                }

                oldLeft.Right = oldRight;
                oldRight.Left = oldLeft;

                // destination fix
                newLeft.Right = this;
                newRight.Left = this;

                // fix me
                Left = newLeft;
                Right = newRight;
            }
            internal void MoveRight(int val)
            {
                if (val == 0)
                    return;

                // source fix
                var oldLeft = Left;
                var oldRight = Right;

                var newLeft = FindRight(val);
                var newRight = newLeft.Right;
                /*if (newRight.OriginalIndex == -1)
                {
                    newRight = newRight.Right;
                    newLeft = newRight.Left;   // this will be the 0
                }*/

                oldLeft.Right = oldRight;
                oldRight.Left = oldLeft;

                // destination fix
                newLeft.Right = this;
                newRight.Left = this;

                // fix me
                Left = newLeft;
                Right = newRight;
            }

            internal static Code FakeHead()
            {
                var code = new Code(-1, 1);
                code.Head = true;
                return code;
            }
        }

        static void Day1()
        {
            var input = File.ReadAllLines("Day20.txt");
            if (_fake == true)
                input = File.ReadAllLines("DayFake20.txt");

            var n = input.Count();
            var codes = new List<BaseCode>();
            for (int i = 0; i < n; i++)
            {
                var code = new Code(i, long.Parse(input[i]));
                codes.Add(code);
            }

            if (_logAll)
                Helper.Log("Start " + string.Join(",", codes.Select(c => c.Value)));
            for (int i = 0; i < n; i++)
            {
                var code = codes.First(c => c.OriginalIndex == i);
                var val = code.Value;
                var from = codes.IndexOf(code);
                var to = (int)((from + val) % (codes.Count - 1));
                if (to < 0 && from + val != 0)
                    to += codes.Count - 1;
                codes.Remove(code);
                codes.Insert(to, code);
                if (_logAll)
                    Helper.Log($"Moves '{val}' between {from} and {to}");
                if (_logAll)
                {
                    //Helper.Log(string.Join(", ", newInts));
                    if (_fake && _fakeRights[i] != string.Join(", ", codes.Select(c => c.Value)))
                        Helper.Log("Bummer");

                }
            }
            if (_logAll)
                Helper.Log("Star Final: " + string.Join(",", codes.Select(c => c.Value)));

            File.WriteAllLines("newIntsOld.txt", codes.Select(c => c.Value.ToString()));
            var zeroCode = codes.First(c => c.Value == 0);
            var i0 = codes.IndexOf(zeroCode);
            var a2 = NextI(codes, i0 + 1000);
            var b2 = NextI(codes, i0 + 2000);
            var c2 = NextI(codes, i0 + 3000);

            var score = a2.Value + b2.Value + c2.Value;

            Helper.Log("Star Score: " + score); // not 11237
        }
        static void Day2()
        {
            var input = File.ReadAllLines("Day20.txt");
            if (_fake == true)
                input = File.ReadAllLines("DayFake20.txt");

            var n = input.Count();
            var codes = new List<BaseCode>();
            for (int i = 0; i < n; i++)
            {
                var code = new Code(i, long.Parse(input[i]) * 811589153);
                codes.Add(code);
            }

            if (_logAll)
                Helper.Log("Start " + string.Join(",", codes.Select(c => c.Value)));
            for (int j = 0; j < 10; j++)
                for (int i = 0; i < n; i++)
                {
                    var code = codes.First(c => c.OriginalIndex == i);
                    var val = code.Value;
                    var from = codes.IndexOf(code);
                    var to = (int)((from + val) % (codes.Count - 1));
                    if (to < 0 && from + val != 0)
                        to += codes.Count - 1;
                    codes.Remove(code);
                    codes.Insert(to, code);
                    if (_logAll)
                        Helper.Log($"Moves '{val}' between {from} and {to}");
                    if (_logAll)
                    {
                        //Helper.Log(string.Join(", ", newInts));
                        if (_fake && _fakeRights[i] != string.Join(", ", codes.Select(c => c.Value)))
                            Helper.Log("Bummer");

                    }
                }
            if (_logAll)
                Helper.Log("Star Final: " + string.Join(",", codes.Select(c => c.Value)));

            File.WriteAllLines("newIntsOld.txt", codes.Select(c => c.Value.ToString()));
            var zeroCode = codes.First(c => c.Value == 0);
            var i0 = codes.IndexOf(zeroCode);
            var a2 = NextI(codes, i0 + 1000);
            var b2 = NextI(codes, i0 + 2000);
            var c2 = NextI(codes, i0 + 3000);

            var score = a2.Value + b2.Value + c2.Value;

            Helper.Log("Star Score: " + score); // not 11237
        }

        static void Day1Link()  //LinkedList()
        {
            var input = File.ReadAllLines("Day20.txt");
            if (_fake == true)
                input = File.ReadAllLines("DayFake20.txt");

            var origInts = input.Select(s => int.Parse(s)).ToList();

            var n = origInts.Count;
            var codes = new List<Code>();
            var previous = Code.FakeHead();
            codes.Add(previous);
            for (int i = 0; i < n; i++)
            {
                var code = new Code(i, origInts[i]);
                if (previous != null)
                    previous.Right = code;
                code.Left = previous;
                codes.Add(code);
                previous = code;
            }
            codes.Last().Right = codes.First();
            codes.First().Left = codes.Last();
            Valid(codes);
            var temp = GetList(codes);
            if (_logAll)
                Helper.Log("Start " + string.Join(",", codes));
            for (int i = 0; i < n; i++)
            {
                var code = codes.First(c => c.OriginalIndex == i);
                var val = code.Value;
                if (val < 0)    // move left
                    code.MoveLeft((int)((0 - val) % n));
                else if (val > 0)    // move left
                    code.MoveRight((int)(val % n));
                if (_logAll)
                {
                    Valid(codes);
                    temp = GetList(codes);
                }
                if (_fake && _fakeRights[i] != string.Join(", ", temp))
                    Helper.Log("Bummer");
            }

            if (_logAll)
                Helper.Log("Star Final: " + string.Join(",", codes));

            var current = codes.First(c => c.Value == 0);
            var a1 = current.FindRight(1000);
            var b1 = current.FindRight(2000);
            var c1 = current.FindRight(3000);

            temp = GetList(codes);
            var i0 = temp.IndexOf(0);
            var a2 = NextI(temp, i0 + 1000);
            var b2 = NextI(temp, i0 + 2000);
            var c2 = NextI(temp, i0 + 3000);

            var score = a2 + b2 + c2;

            Helper.Log("Star Score: " + score); // not 11237
            File.WriteAllLines("newInts.txt", temp.Select(i => i.ToString()));
        }
        static public List<long> GetList(List<Code> linkedList)
        {
            var rv = new List<long>();
            var current = linkedList.First(c => c.Head).Right;
            while (rv.Count() < linkedList.Count() - 1)
            {
                rv.Add(current.Value);
                current = current.Right;
            }
            return rv;
        }
        static public bool Valid(List<Code> intList)
        {
            var intLefts = new List<Code>();
            var intRights = new List<Code>();
            var intCurrents = new List<Code>();
            var c = intList[0];
            while (intCurrents.Count() < intList.Count())
            {
                if (!intLefts.Contains(c.Left))
                    intLefts.Add(c.Left);
                else
                    Helper.Log("Bad lefts");

                if (!intCurrents.Contains(c))
                    intCurrents.Add(c);
                else
                    Helper.Log("Bad intCurrents");

                if (!intRights.Contains(c.Right))
                    intRights.Add(c.Right);
                else
                    Helper.Log("Bad rights");

                c = c.Right;
            }
            if (!(intRights.Count == intLefts.Count && intLefts.Count == intCurrents.Count && intCurrents.Count == intList.Count))
            {
                Helper.Log("Bad counts");
                return false;
            }
            return true;
        }

        static void InsertBetweenValues(List<BaseCode> list, BaseCode val, BaseCode right)
        {
            list.Remove(val);
            var iRight = list.IndexOf(right);
            if (iRight == 0)
                list.Add(val);
            else
                list.Insert(iRight, val);
        }
        static int Roll(int n, int max)
        {
            if (n > max)
                return Roll(n - max, max);
            else if (n < 0)
                return Roll(n + max, max);
            else
                return n;
        }
        static T NextI<T>(List<T> list, int i)
        {
            while (i < 0)
                i += list.Count();

            if (i % list.Count() < list.Count())
                return list[i % list.Count()];
            return list[i % list.Count()];
        }

        static void InsertBetween(List<int> list, int from, int to)
        {
            if (from == to)
                return;
            var val = list[from];
            if (to == 0)
            {
                if (_logAll)
                    Helper.Log("Add to end");
                list.Remove(val);
                list.Add(val);
                return;
            }
            list.Remove(val);
            if (from < to)  // we moved 'to' to the left so subtract 1
            {
                if (_logAll)
                    Helper.Log($"From after");
                to--;
            }
            list.Insert(to + 1, val); // +1 because we want to insert after to
        }
    }
}
