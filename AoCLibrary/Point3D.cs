using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace AoCLibrary;

public class Point3D(long x, long y, long z) : IEquatable<Point3D>
{
    static public Point3D Parse(string str)
    {
        var parts = Utils.SplitLongs(',', str.Trim('(').Trim(')'));
        if (parts.Length == 2)
            return new Point3D(parts[0], parts[1], 0);

        return new Point3D(parts[0], parts[1], parts[2]);
    }

    public Point3D(Point3D other) : this(other.X, other.Y, other.Z)
    {
        
    }
    public long X { get; set; } = x;
    public long Y { get; set; } = y;
    public long Z { get; set; } = z;
    public bool Equals(Point3D? other)
    {
        return other?.X == X && other?.Y == Y && other?.Z == Z;
    }
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
        //return (int)((X * 1E6) + (Y * 1E3) + Z);
    }
    public override bool Equals(object? obj)
    {
        if (obj is not Point3D other)
            return false;

        return Equals(other);
    }
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public double Distance(Point3D other)
	{         
		var dx = other.X - X;
		var dy = other.Y - Y;
		var dz = other.Z - Z;
		return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }
    internal decimal Slope2D(Point3D pt2)
    {
        if ((pt2.X - X) == 0)
            return decimal.MaxValue;
        return (decimal)(pt2.Y - Y) / (pt2.X - X);
    }

    internal bool IsOnLine2D(decimal slope, decimal intercept)
    {
        return (Y == slope * X + intercept);
    }

    public Point3D Offset(Point3D other)
    {
        var dx = other.X - X;
        var dy = other.Y - Y;
        var dz = other.Z - Z;
        return new Point3D(dx, dy, dz);
    }

    public Point3D Add(Point3D offset)
    {
		return new Point3D(X + offset.X, Y + offset.Y, Z + offset.Z);
    }

    public Point3D Subtract(Point3D offset)
    {
        return new Point3D(X - offset.X, Y - offset.Y, Z - offset.Z);
    }

    public long Sub(int i)
    {
        if (i == 0)
            return X;
        else if (i == 1)
            return Y;
        else if (i == 2)
            return Z;
        return 0;
    }

    public long ManhattanDistance(Point3D other)
    {
        return Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
    }
    /*
public bool FuzzyMatch(Point3D other)
{
var ints = GetInts().Select(i => Math.Abs(i));
var others = other.GetInts().Select(i => Math.Abs(i)).ToList();
foreach (var i in ints)
  if (!others.Remove(i))
      return false;

return (others.Any() == false);
}*/
}

public class Rotation3D
{
    int[,] _matrix = new int[3, 3];
    static List<Rotation3D>? _allRotations;
    static public List<Rotation3D> AllRotations()
    {
        if (_allRotations == null)
        {
            var perms = new[]
            {
                new Point3D(0, 1, 2),
                new Point3D(0, 2, 1),
                new Point3D(1, 0, 2),
                new Point3D(1, 2, 0),
                new Point3D(2, 0, 1),
                new Point3D(2, 1, 0),
            };
            var signs = new List<Point3D>();
            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                        signs.Add(new Point3D(x, y, z));
            var rv = new List<Rotation3D>();
            foreach (var perm in perms)
                foreach (var sign in signs)
                {
                    var rot = new Rotation3D();
                    for (int i = 0; i < 3; i++)
                        rot.Set(perm.Sub(i), i, sign.Sub(i));
                    if (rot.Determinate() == 1)
                        rv.Add(rot);
                }
            _allRotations = rv;
        }
        return _allRotations;
    }
    private int Determinate()
    {
        return
            _matrix[0, 0] * (_matrix[1, 1] * _matrix[2, 2] - _matrix[1, 2] * _matrix[2, 1]) -
            _matrix[0, 1] * (_matrix[1, 0] * _matrix[2, 2] - _matrix[1, 2] * _matrix[2, 0]) +
            _matrix[0, 2] * (_matrix[1, 0] * _matrix[2, 1] - _matrix[1, 1] * _matrix[2, 0]);
    }

    private void Set(long x, int y, long value)
    {
        _matrix[x, y] = (int) value;
    }

    public Point3D Apply(Point3D pt)
    {
        var x = _matrix[0, 0] * pt.X + _matrix[0, 1] * pt.Y + _matrix[0, 2] * pt.Z;
        var y = _matrix[1, 0] * pt.X + _matrix[1, 1] * pt.Y + _matrix[1, 2] * pt.Z;
        var z = _matrix[2, 0] * pt.X + _matrix[2, 1] * pt.Y + _matrix[2, 2] * pt.Z;
        return new Point3D(x, y, z);
    }
}