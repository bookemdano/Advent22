namespace Advent25;

public class RunnerResult
{
	public object? Star1 { get; set; }
	public object? Star2 { get; set; }
	public override string ToString()
	{
		return "Star1=> " + Star1 + " Star2=> " + Star2;
	}
}
interface IDayRunner
{
	bool IsReal { get; }
	object? Star1();
	object? Star2();

}
