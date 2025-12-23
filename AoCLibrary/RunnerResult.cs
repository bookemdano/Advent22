namespace AoCLibrary;

public class RunnerResult
{
    public TimeSpan Ts { get; set; }
    public StarCheck Check { get; set; }
    public bool StarSuccess { get; private set; }
    public long? GuessL { get; set; }
    public string? GuessSz { get; set; }

    public void CheckGuess(long l)
    {
        GuessL = l;
        StarSuccess = Check.Compare(l);
    }
    public void CheckGuess(string sz)
    {
        GuessSz = sz;
        StarSuccess = Check.Compare(sz);
    }
    public string Guess()
    {
        if (GuessL != null)
            return GuessL.ToString()!;
        else
            return GuessSz!;
    }

    public override string ToString()
    {
        return $"Guess=>{Guess()} StarSuccess=>{StarSuccess} Check=>{Check} TimeSpan=>{ElfHelper.SmallString(Ts)}";
    }
}
