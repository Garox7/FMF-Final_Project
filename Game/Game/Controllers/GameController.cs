using Microsoft.AspNetCore.Mvc;
using Game.Services;
using Game.Models;
using Game.Model;

namespace Game.Controllers;

[ApiController]
[Route("api/game")]
public class GameController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly GameService _gameService;

    public GameController(GameService gameService)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["AuthEndPoint"] ?? "")
        };

        _gameService = gameService;
    }

    [HttpGet]
    [Route("match/{playerEmail}/{id}")]
    public async Task<IActionResult> GetMatch(string playerEmail, int id,[FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");
            
            if (authResponse.IsSuccessStatusCode)
            {
                Match existingMatch = _gameService.GetMatch(playerEmail, id);
                return Ok(existingMatch);
            }
            return Unauthorized();
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("match/list-match/{playerEmail}")]
    public async Task<IActionResult> GetMatches(string playerEmail, [FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");
            
            if (authResponse.IsSuccessStatusCode)
            {
                List<Match> matches = _gameService.GetAllMatches(playerEmail);
                return Ok(matches);
            }
            return Unauthorized();
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("match/new-match/{playerEmail}")]
    public async Task<IActionResult> CreateMatch(string playerEmail, [FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");

            if (authResponse.IsSuccessStatusCode)
            {
                Match newMatch = _gameService.CreateMatch(playerEmail);
                return Ok(newMatch);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Route("match/play/{playerEmail}/{matchId}")]
    public async Task<IActionResult> PlayTurnMatch(string playerEmail, int matchId, [FromBody] MatchDto matchDto, [FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");

            if (authResponse.IsSuccessStatusCode)
            {
                Match turnResponse = _gameService.PlayTurnMatch(matchDto.DiscardCards, matchDto.DiscardPileId, playerEmail, matchId);
                return Ok(turnResponse);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("match/draw-card/{playerEmail}/{matchId}")]
    public async Task<IActionResult> GetCard(string playerEmail, int matchId, [FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");

            if (authResponse.IsSuccessStatusCode)
            {
                Card cardDrawn = _gameService.DrawCardInCurrentMatch(playerEmail, matchId);
                return Ok(cardDrawn);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("match/delete/{playerEmail}/{matchId}")]
    public async Task<IActionResult> DeleteMatch(string playerEmail, int matchId, [FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");

            if (authResponse.IsSuccessStatusCode)
            {
                List<Match> newListMatches = _gameService.DeleteMatch(playerEmail, matchId);
                return Ok(newListMatches);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("match/leaderboard")]
    public async Task<IActionResult> GetLeaderboard([FromQuery] string token)
    {
        try
        {
            var authResponse = await _httpClient.GetAsync($"api/authentication/check-auth/{token}");

            if (authResponse.IsSuccessStatusCode)
            {
                List<LeaderBoardData> newListMatches = _gameService.GetLeaderboardData();
                return Ok(newListMatches);
            }
            return Unauthorized();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}