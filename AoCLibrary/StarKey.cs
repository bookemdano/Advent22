namespace AoCLibrary;

public enum StarEnum
{
	NA,
	Star1,
	Star2
}
public class StarKey
{
	public StarKey(string day, string star)
	{
		DayIndex = int.Parse(day) - 1;
		if (star == "1")
			Star = StarEnum.Star1;
		else if (star == "2")
			Star = StarEnum.Star2;
	}
	public StarKey(int dayIndex, StarEnum star)
	{
		DayIndex = dayIndex;
		Star = star;
	}
	public override int GetHashCode()
	{
		return DayIndex * 100 + (int) Star;
	}
	public override bool Equals(object? obj)
	{
		if (obj is StarKey other)
			return DayIndex == other.DayIndex && Star == other.Star;
		return false;
	}
	public int DayIndex { get; set; }
	public StarEnum Star { get; set; }
	public override string ToString()
	{
		return $"{(DayIndex + 1):00}-{Star}";
	}
}
