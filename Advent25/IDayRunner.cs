using AoCLibrary;

namespace Advent25;

public class RunnerResult
{
    public object? StarValue { get; set; }
    public bool? StarSuccess { get; set; }
    public TimeSpan Ts { get; set; }
    public override string ToString()
    {
        return $"StarValue=>{StarValue} StarSuccess=> {StarSuccess} TimeSpan=> {ElfHelper.SmallString(Ts)}";
    }
}

interface IDayRunner
{
    RunnerResult Star1(bool isReal);
    RunnerResult Star2(bool isReal);

}
