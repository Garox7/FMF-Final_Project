using System.Collections.Concurrent;
using FileManager;
using Game.Models;
using Game.Exceptions;
using Game.Model;

namespace Game.Services;
public class GameService
{
    private ConcurrentBag<Match> _matches;
    private readonly string _matchPath = "./Data/matches.json";
    private readonly DataManager<ConcurrentBag<Match>> _matchesDataManager;

    public GameService() 
    {
        _matches = new ConcurrentBag<Match>();
        _matchesDataManager = new DataManager<ConcurrentBag<Match>>(_matchPath);
        LoadAsyncData().Wait();
    }

    public async Task LoadAsyncData()
    {
        try
        {
            _matches = await _matchesDataManager.LoadData<ConcurrentBag<Match>>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error loading data: {ex}");
        }
    }

    private async Task SaveAsyncData()
    {
        try
        {
            await _matchesDataManager.SaveData(_matches);
        }
        catch (Exception ex)
        {
            throw new Exception($"error while saving: {ex}");
        }
    }

    public List<Match> GetAllMatches(string playerEmail) 
    {
        List<Match> matches = _matches.Where(m => m.Player.Email == playerEmail && m.IsDeleted == false).ToList();

        if (matches is not null)
        {
            return matches;   
        }
        else throw new Exception("No matches found");
    }

    public Match GetMatch(string playerEmail, int id)
    {
        try {
            Match matchFound = _matches.First(m => m.Player.Email == playerEmail && m.Id == id);
            return matchFound;
        }
        catch
        {
            throw new MatchNotFoundException("match not found");
        }
    }

    public Match CreateMatch(string playerEmail)
    {
        int newId = _matches.Count + 1;
        Match newMatch = new(newId, playerEmail);
        _matches.Add(newMatch);
        _ = SaveAsyncData();
        return newMatch;
    }

    public Match PlayTurnMatch(List<Card> discardCards, int discardPileId, string playerEmail, int matchId)
    {
        Match currentMatch = _matches.First(m => m.Player.Email == playerEmail && m.Id == matchId);
        Match playCurrentMatch = currentMatch.PlayTurn(discardCards, discardPileId);
        _ = SaveAsyncData();
        return playCurrentMatch;
    }

    public Card DrawCardInCurrentMatch(string playerEmail, int matchId)
    {
        Match currentMatch = _matches.First(m => m.Player.Email == playerEmail && m.Id == matchId);
        Card cardDrawn = currentMatch.DrawFromDeck();
        return cardDrawn;
    }

    public List<Match> DeleteMatch(string playerEmail, int matchId)
    {
        Match? matchToDelete = _matches.FirstOrDefault(m => m.Id == matchId);

        if (matchToDelete is not null) {
            matchToDelete.IsDeleted = true;
            List<Match> newListMtches = GetAllMatches(playerEmail);
            _ = SaveAsyncData();
            return newListMtches;
        }
        else throw new Exception("The match you want to delete was not found");
    }

    public List<LeaderBoardData> GetLeaderboardData()
    {
        List<LeaderBoardData> leaderboardData = _matches
            .Where(match => match.Status == MatchStatus.Won || match.Status == MatchStatus.Lost)
            .GroupBy(match => match.Player.Email)
            .Select(group => new LeaderBoardData
            {
                PlayerUsername = group.First().Player.Email,
                TotalMatches = group.Count(),
                WonMatches = group.Count(match => match.Status == MatchStatus.Won),
                LostMatches = group.Count(match => match.Status == MatchStatus.Lost),
                WinRate = CalculateWinRate(group.Count(), group.Count(match => match.Status == MatchStatus.Won))
            }).ToList();
    
        leaderboardData = leaderboardData.OrderByDescending(s => s.WinRate).ToList();

        return leaderboardData;
    }

    private double CalculateWinRate(int totalMatches, int wonMatches)
    {
        if (totalMatches == 0)
        {
            return 0.0;
        }

        return (double)wonMatches / totalMatches;
    }
}