using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent23
{
	public class Point
	{
		public Point(int row, int col)
		{
			Row = row;
			Col = col;
		}
		public override int GetHashCode()
		{
			return Row * 100000 + Col;
		}
		public override bool Equals(object? obj)
		{
			if (obj is not Point other)
				return false;
			return (other.Row == Row && other.Col == Col);
		}
		public override string ToString()
		{
			return $"({Row}, {Col})";
		}
		public int Row { get; }
		public int Col { get; }
	}
	internal class ElfUtils
	{
	}
}
