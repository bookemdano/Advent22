namespace Advent22
{
    internal class Day12
    {
        static public void Run()
        {
            //Star1();
            Star2();
        }
        static public void Star1()
        {
            var input = File.ReadAllLines("Day12.txt");
            //var e = grid.Find('E');

            var grids = new List<Grid>();
            grids.Add(new Grid(input));
            var score = 0;
            bool winner = false;
            var currents = new List<RC>();
            while (winner == false)
            {
                score++;
                var removes = new List<Grid>();
                var adds = new List<Grid>();
                Helper.Log("No winners " + score + " checking " + grids.Count() + " grids");
                foreach (var grid in grids)
                {
                    var deltas = grid.Deltas(ignoreA: false);
                    if (deltas.Count() == 0 || currents.Any(c => c.Same(grid.Current)))
                        // deadend
                        removes.Add(grid);
                    else
                    {
                        var first = true;
                        var cur = grid.Current;
                        grid.Set(cur, 'X');
                        if (!currents.Any(c => c.Same(cur)))
                            currents.Add(cur);
                        foreach (var delta in deltas)
                        {
                            if (grid.GetChar(delta.Key) == 'E')
                            {
                                Helper.Log("WINNER!");
                                Helper.Log(grid.ToString());
                                //removes.Add(grid);
                                winner = true;
                                break;
                            }
                            if (first)
                            {
                                grid.Current = delta.Key;
                                //Helper.Log(grid.ToString());
                                first = false;
                            }
                            else
                            {
                                var add = new Grid(grid, delta.Key);
                                //Helper.Log(add.ToString());
                                adds.Add(add);
                            }
                        }
                        if (winner)
                            break;
                    }
                }
                // go through the grids and get only allow lowest current
                if (winner)
                    break;
                foreach (var remove in removes)
                    grids.Remove(remove);
                var group = grids.GroupBy(g => g.Current).Take(100);
                foreach (var add in adds)
                    grids.Add(add);
                //var uniques = currents.DistinctBy(c => c.ToString()).ToArray();
                //foreach (var current in uniques)
                //    foreach (var grid in grids)
                //        grid.Set(current, 'X');
            }
            Helper.Log("Star1 Score: " + score);

        }
        static public void Star2()
        {
            var input = File.ReadAllLines("Day12.txt");
            // create a set of grids for each starting point, all the 'a's and 'S'
            var grids = new Dictionary<RC, List<Grid>>();
            var grid0 = new Grid(input);
            grids.Add(grid0.Current, new List<Grid>() { new Grid(input) });
            var all = grid0.FindAll('a');
            foreach (var a in all)
                grids.Add(a, new List<Grid>() { new Grid(grid0, a) });
      
            // add all the starting points to list of ones we have tried already
            var currents = new Dictionary<RC, List<RC>>();
            foreach (var grid in grids)
                currents.Add(grid.Key, new List<RC>());

            var score = 0;
            bool winner = false;
            while (winner == false)
            {
                score++;
                Helper.Log("No winner " + score + " checking " + grids.Sum(g => g.Value.Count()) + " grids");
                foreach (var kvp in grids)
                {
                    var removes = new List<Grid>();
                    var adds = new List<Grid>();
                    var starting = kvp.Key;
                    foreach (var grid in kvp.Value)
                    {
                        var deltas = grid.Deltas(true);
                        if (deltas.Count() == 0 || currents[starting].Any(c => c.Same(grid.Current)))
                            // deadend or loop
                            removes.Add(grid);
                        else
                        {
                            var first = true;
                            var cur = grid.Current;
                            // set the current one as X and add to list so we ignore it going forward to avoid loops
                            grid.Set(cur, 'X');
                            if (!currents[starting].Any(c => c.Same(cur)))
                                currents[starting].Add(cur);
                            foreach (var delta in deltas)
                            {
                                if (grid.GetChar(delta.Key) == 'a')
                                {
                                    // we are already checking this path
                                    continue;
                                }
                                if (grid.GetChar(delta.Key) == 'E')
                                {
                                    Helper.Log("WINNER!");
                                    Helper.Log(grid.ToString());
                                    //removes.Add(grid);
                                    winner = true;
                                    break;
                                }
                                if (first)
                                {
                                    // extend current grid
                                    grid.Current = delta.Key;
                                    //Helper.Log(grid.ToString());
                                    first = false;
                                }
                                else
                                {
                                    // make a new grid
                                    var add = new Grid(grid, delta.Key);
                                    //Helper.Log(add.ToString());
                                    adds.Add(add);
                                }
                            }
                            if (winner)
                                break;
                        }
                    }
                    if (winner)
                        break;
                    foreach (var remove in removes)
                        grids[starting].Remove(remove);
                    foreach (var add in adds)
                        grids[starting].Add(add);
                }
                // go through the grids and get only allow lowest current
                if (winner)
                    break;
            }
            Helper.Log("Star2Score: " + score);
        }
    }
    class Grid
    {
        List<char[]> _lines;
        //char[,] _lines;

        int _rows;
        int _cols;
        public RC Current { get; set; }

        public Grid(Grid other, RC current)
        {
            _lines = new List<char[]>();
            foreach (var line in other._lines)
                _lines.Add(line.ToArray());
            _rows = other._rows;
            _cols = other._cols;
            Current = current;
        }
        public Grid(string[] lines)
        {
            _rows = lines.Count();
            _cols = lines[1].Length;
            _lines = new List<char[]>();
            foreach(var line in lines)
                _lines.Add(line.ToCharArray());
            Current = CalcCurrent();

        }
        public RC CalcCurrent()
        {
            return Find('S');

        }
        public int Score()
        {
            return 1;
        }
        public Dictionary<RC, int> Deltas(bool ignoreA)
        {
            var val = Get(Current);
            var deltas = new Dictionary<RC, int>();
            var options = Options(Current);
            foreach (var kvp in options.OrderByDescending(k => k.Value))
            {
                if (kvp.Value < 0)
                    break;
                if (ignoreA && kvp.Value == 0)
                    break;
                if (kvp.Value - val > 1)
                    continue;
                deltas.Add(kvp.Key, kvp.Value - val);
            }
            return deltas;
        }
        public Dictionary<RC, int> Options(RC rc)
        {
            var rv = new Dictionary<RC, int>();
            rv.Add(new RC(rc.Row - 1, rc.Col), Get(new RC(rc.Row - 1, rc.Col)));
            rv.Add(new RC(rc.Row + 1, rc.Col), Get(new RC(rc.Row + 1, rc.Col)));
            rv.Add(new RC(rc.Row, rc.Col - 1), Get(new RC(rc.Row, rc.Col - 1)));
            rv.Add(new RC(rc.Row, rc.Col + 1), Get(new RC(rc.Row, rc.Col + 1)));
            return rv;
        }
        public void Set(RC rc, char c)
        {
            _lines[rc.Row][rc.Col] = c;
        }
        string ReplaceCharX(string str, int i, char c)
        {
            if (str[i] == c)
                return str;
            if (i < 0)
                return str;
            var chars = str.ToCharArray();
            chars[i] = c;
            return new string(chars);
        }

        public char GetChar(RC rc)
        {
            if (rc.Row >= _rows || rc.Col >= _cols || rc.Row < 0 || rc.Col < 0)
                return ' ';

            return _lines[rc.Row][rc.Col];
        }
        int Get(RC rc)
        {
            var c = GetChar(rc);
            if (c == ' ')
                return int.MinValue;

            if (c == 'X')
                return int.MinValue;
            else if (c == 'S')
                return 0;
            else if (c == 'E')
                return 25;
            return c - 'a';
        }
        public RC Find(char c)
        {
            for (int iRow = 0; iRow < _rows; iRow++)
            {
                for (int iCol = 0; iCol < _cols; iCol++)
                    if (_lines[iRow][iCol] == c)
                        return new RC(iRow, iCol);

            }
            return null;
        }
        public List<RC> FindAll(char c)
        {
            var rv = new List<RC>();
            for (int iRow = 0; iRow < _rows; iRow++)
            {
                for (int iCol = 0; iCol < _cols; iCol++)
                    if (_lines[iRow][iCol] == c)
                        rv.Add(new RC(iRow, iCol));

            }
            return rv;
        }
        public override string ToString()
        {
            return Current + Environment.NewLine + string.Join(Environment.NewLine, _lines.Select(l => new string(l)));
        }

    }
    class RC
    {
        public int Row { get;}
        public int Col { get;}
        public RC(RC other)
        {
            Row = other.Row;
            Col = other.Col;
        }
        public RC(int row, int col)
        {
            Row = row;
            Col = col;
        }
        public bool Same(RC rc)
        {
            return rc.Col == Col && rc.Row == Row;
        }
        public override string ToString()
        {
            return Row + "," + Col;
        }
    }
}

