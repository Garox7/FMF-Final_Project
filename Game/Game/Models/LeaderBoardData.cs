using Game.Models;

namespace Game.Model;
public class LeaderBoardData
{
    public string PlayerUsername { get; set; }
    public int TotalMatches { get; set; }
    public int WonMatches { get; set; }
    public int LostMatches { get; set; }
    public double WinRate { get; set; }
}