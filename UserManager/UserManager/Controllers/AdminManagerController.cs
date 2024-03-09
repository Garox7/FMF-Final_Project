using Microsoft.AspNetCore.Mvc;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Controllers;
[ApiController]
[Route("api/user-manager/admin")]
public class AdminManagerController : Controller
{
    private readonly UsersService _userService;

    public AdminManagerController(UsersService usersService)
    {
        _userService = usersService;
    }

    [HttpGet]
    [Route("users")]
    public ActionResult<List<UserDto>> GetUsers([FromQuery] string token)
    {
        User? isAuthenticatedUser = _userService.GetUserByToken(token);

        if (isAuthenticatedUser is not null)
        {
            return Ok(_userService.ToUserDto(_userService.GetAll()));
        }
        return Unauthorized();
    }

    [HttpGet]
    [Route("users/{id}")]
    public ActionResult<UserDto> GetUser(string id, [FromQuery] string token)
    {
        User? isAuthenticatedUser = _userService.GetUserByToken(token);

        if (isAuthenticatedUser is not null)
        {
            User userToFind = _userService.GetUserById(id);

            return Ok(UserDto.FromUser(userToFind));   
        }
        return NotFound("User not found or invalid token");
    }

    [HttpPost]
    [Route("users/create")]
    public ActionResult<UserDto> CreateUser([FromQuery] string token, [FromBody] UserDto userDto)
    {
        User? isAuthenticatedUser = _userService.GetUserByToken(token);

        if(isAuthenticatedUser is not null)
        {
            UserDto? userToCreate = _userService.CreateUser(userDto);
            return Ok(userToCreate);
        }
        return BadRequest("Invalid Token");
    }

    [HttpPatch]
    [Route("users/update/{id}")]
    public ActionResult<UserDto> UpdateUser([FromQuery] string token, [FromBody] UserDto userDto)
    {
        User? isAuthenticatedUser = _userService.GetUserByToken(token);

        if (isAuthenticatedUser is not null)
        {
            UserDto? userToUpdate = _userService.UpdateUser(userDto);
            return Ok(userToUpdate);
        }
        return BadRequest("Invalid Token");
    }

    [HttpDelete]
    [Route("users/delete/{id}")]
    public ActionResult DeleteUser(string id, [FromQuery] string token)
    {
        User? isAuthenticatedUser = _userService.GetUserByToken(token);
        
        if (isAuthenticatedUser is not null)
        {
            return _userService.DeleteUser(id) ? Ok("User deleted") : BadRequest("User is already deleted");
        }
        return BadRequest("Invalid Token");
    }
}