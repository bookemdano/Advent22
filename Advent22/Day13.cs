using Advent22;

namespace Advent22
{
    internal class Day13
    {
        static public void Run()
        {
            var input = File.ReadAllLines("Day13.txt");
            int score = 0;
            var iPair = 1;
            var startItem = new ItemOrList("[[2]]");
            var endItem = new ItemOrList("[[6]]");
            var startI = 1;
            var endI = 1;
            for (int i = 0; i < input.Count(); i += 3)
            {
                var left = new ItemOrList(input[i]);
                var right = new ItemOrList(input[i + 1]);
                var c = Compare(left, right);
                if (c == true)
                    score += iPair;
                Helper.Log($"{iPair} {score}");
                //Helper.Log($"{pair.Left}, {pair.Right} is {c} {score}");
                iPair++;
    
                // part 2
                if (left.Compare(endItem) == CompareEnum.Less)
                {
                    endI++;
                    if (left.Compare(startItem) == CompareEnum.Less)
                        startI++;
                }
                if (right.Compare(endItem) == CompareEnum.Less)
                {
                    endI++;
                    if (right.Compare(startItem) == CompareEnum.Less)
                        startI++;
                }
            }
            Helper.Log("Star1 Score: " + score); // 5910 is too high, 0 is different than []
            Helper.Log("Star2 Score: " + (startI * (endI + 1)));  // 23868 too low, off by one
        }
        static bool Compare(ItemOrList left, ItemOrList right)
        {
            var leftItems = left.Items;
            if (!left.HasItems)
                leftItems = new ItemOrList[] { left };
            var rightItems = right.Items;
            if (!right.HasItems)
                rightItems = new ItemOrList[] { right };

            for (int i = 0; i < leftItems.Length; i++)
            {
                if (rightItems.Length > i)
                {
                    var comp = leftItems[i].Compare(rightItems[i]);
                    if (comp != CompareEnum.Same)
                        return (comp == CompareEnum.Less);
                }
                else
                    return false;
            }
            if (leftItems.Length < rightItems.Length)
                return true;
            return false;
        }
    }
    public enum CompareEnum
    {
        NA,
        Less,
        More,
        Same
    }

    public class ItemOrList
    {
        public ItemOrList(string str)
        {
            if (str == "[]")
            {
                Val = -1;   // we want this to be less than [0]
                return; 
            }
            if (str.StartsWith('['))
            {
                var parts = Split(str.Substring(1, str.Length - 2));
                Items = parts.Select(p => new ItemOrList(p)).ToArray();
            }
            else
                Val = int.Parse(str);
        }

        public CompareEnum Compare(ItemOrList right)
        {
            if (HasVal && right.HasVal)
                return IntCompare(Val, right.Val);

            var leftItems = Items;
            if (!HasItems)
                leftItems = new ItemOrList[] { this };
            var rightItems = right.Items;
            if (!right.HasItems)
                rightItems = new ItemOrList[] { right };
            for (int i = 0; i < leftItems.Length; i++)
            {
                if (rightItems.Length > i)
                {
                    var comp = leftItems[i].Compare(rightItems[i]);
                    if (comp != CompareEnum.Same)
                        return comp;
                }
                else
                    return CompareEnum.More;    // ran out of left items
            }
            if (leftItems.Length < rightItems.Length) // ran out of right item
                return CompareEnum.Less;
            return CompareEnum.Same;
        }
        CompareEnum IntCompare(int left, int right)
        {
            if (left < right)
                return CompareEnum.Less;
            else if (left > right)
                return CompareEnum.More;
            else
                return CompareEnum.Same;
        }

        public ItemOrList[] Items { get; set; }
        public int Val { get; set; }

        public bool HasItems => (Items != null);
        public bool HasVal => !HasItems;

        public override string ToString()
        {
            if (HasVal)
                return "v:" + Val.ToString();
            else
                return $"n:[{string.Join(',', Items.Select(s => s.ToString()))}]";
        }
        public string[] Split(string str)
        {
            var rv = new List<string>();
            var depth = 0;
            var start = 0;
            for(int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (c == '[')
                    depth++;
                else if (c == ']')
                    depth--;
                else if (depth == 0 && c == ',')
                {
                    rv.Add(str.Substring(start, i - start));
                    start = i + 1;
                }
            }
            rv.Add(str.Substring(start, str.Length - start));
            return rv.ToArray();
        }
    }
}

