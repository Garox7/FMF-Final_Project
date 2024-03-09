using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Game.Models;
using Game.Services;

namespace Game.Controllers;

[ApiController]
[Route("api/login")]
public class LoginController : Controller
{
    private readonly HttpClient _httpclient;
    private readonly GameService _gameService;

    public LoginController(GameService gameService)
    {
        _httpclient = new HttpClient
        {
            BaseAddress = new Uri(System.Configuration.ConfigurationManager.AppSettings["AuthEndPoint"] ?? "")
        };

        _gameService = gameService;
    }

    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetAutentication(string username)
    {
        try
        {
            var response = await _httpclient.GetAsync($"api/authentication/{username}");

            if (response.IsSuccessStatusCode)
            {
                string challenge = await response.Content.ReadAsStringAsync();
                return Ok(JsonConvert.SerializeObject(challenge));
            }
            return Unauthorized("Invalid credentials");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("{playerEmail}/{challenge}")]
    public async Task<IActionResult> Authentication(string playerEmail, string challenge)
    {
        try {
            var response = await _httpclient.GetAsync($"api/authentication/{playerEmail}/{challenge}");

            if (response.IsSuccessStatusCode)
            {
                PlayerDto? playerDto = await response.Content.ReadFromJsonAsync<PlayerDto>();

                return Ok(JsonConvert.SerializeObject(playerDto));
            }
            return Unauthorized("Invalid credentials");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}