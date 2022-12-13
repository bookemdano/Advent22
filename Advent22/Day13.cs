using Advent22;

namespace Advent22
{
    internal class Day13
    {
        static public void Run()
        {
            var input = File.ReadAllLines("Day13.txt");
            var pairs = new List<Pair>();
            int score = 0;
            var iPair = 1;
            for (int i = 0; i < input.Count(); i += 3)
            {
                var pair = new Pair();
                pair.Left = new ItemOrList(input[i]);
                pair.Right = new ItemOrList(input[i + 1]);
                pairs.Add(pair);
                var c = pair.Compare();
                if (c == true)
                    score += iPair;
                Helper.Log($"{iPair} {score}");
                //Helper.Log($"{pair.Left}, {pair.Right} is {c} {score}");
                iPair++;
            }
            Helper.Log("Star1 Score: " + score); // 5910 is too high, 0 is different than -1

            var masterList = new List<ItemOrList>();
            masterList.AddRange(pairs.Select(p => p.Left));
            masterList.AddRange(pairs.Select(p => p.Right));
            var startItem = new ItemOrList("[[2]]");
            var endItem = new ItemOrList("[[6]]");

            var startI = 1;
            var endI = 1;
            foreach (var item in masterList)
            {
                if (item.Compare(endItem) == CompareEnum.Less)
                {
                    endI++;
                    if (item.Compare(startItem) == CompareEnum.Less)
                        startI++;
                }
            }
            Helper.Log("Star2 Score: " + (startI * (endI + 1)));  // 23868 too low, off by one
        }
    }
    public enum CompareEnum
    {
        NA,
        Less,
        More,
        Same
    }
    public class Pair
    {
        public bool Compare()
        {
            var leftItems = Left.Items;
            if (!Left.HasItems)
                leftItems = new ItemOrList[] { Left };
            var rightItems = Right.Items;
            if (!Right.HasItems)
                rightItems = new ItemOrList[] { Right };

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
        public ItemOrList Left { get; set; }
        public ItemOrList Right { get; set; }
    }
    public class ItemOrList
    {
        public bool HasItems
        {
            get
            {
                return (Items != null);
            }
        }
        public bool HasVal
        {
            get
            {
                return !HasItems;
            }
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

        public ItemOrList(string str)
        {
            if (str == "[]")
                return;
            if (str.StartsWith('['))
            {
                var parts = Split(str.Substring(1, str.Length - 2));
                Items = parts.Select(p => new ItemOrList(p)).ToArray();
            }
            else
                Val = int.Parse(str);
        }
        public ItemOrList[] Items { get; set; }
        public int Val { get; set; } = -1;

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

