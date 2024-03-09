using Microsoft.AspNetCore.Mvc;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : Controller
{
    private UsersService _usersService;

    public AuthenticationController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    [Route("{username}")]
    public ActionResult<string> GetAuthenticate(string username)
    {
        try
        {
            User? user =  _usersService.GetUserByUsername(username);
            if (user is null)
            {
                return NotFound("User not found");
            }
            return Ok(user.GenerateChallenge());
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("{username}/{challenge}")]
    public async Task<ActionResult<UserDto>> Authenticate(string username, string challenge)
    {
        try
        {
            User? user = _usersService.GetUserByUsername(username);

            if (user is null)
            {
                return NotFound("User not found");
            }

            if (user.VerifyChallenge(challenge))
            {
                user.GenerateToken();
                
                await _usersService.SaveUsers();

                return Ok(UserDto.FromUser(user));
            }
            return BadRequest("invalid credentials");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Route("check-auth/{token}")]
    public ActionResult<bool> CheckAuthentication(string token)
    {
        try
        {
            User? user = _usersService.GetUserByToken(token);

            if (user is null)
            {
                return BadRequest("Invalid Token");
            }
            else return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}