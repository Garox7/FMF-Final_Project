using Microsoft.AspNetCore.Mvc;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Controllers;
[ApiController]
[Route("api/users")]
public class UsersController : Controller
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpGet]
    [Route("{id}")]
    public ActionResult<UserDto> GetUser(string id, [FromQuery] string token)
    {
        User isUthorizedUser = _usersService.GetUserById(id);

        if (isUthorizedUser.VerifyToken(token))
        {
            var user = _usersService.GetUserById(id);

            if (user is not null && isUthorizedUser.Id == id)
            {
                return Ok(UserDto.FromUser(user));
            }
            else BadRequest("User not Found");
        }
        return Unauthorized("Token expired");
    }

    [HttpPatch]
    [Route("update-user/{id}")]
    public ActionResult<UserDto> UpdateUser(string id, [FromQuery] string token, [FromBody] UserDto userUpdate)
    {
        User? isUthorizedUser = _usersService.GetUserById(id);

        if (isUthorizedUser.VerifyToken(token))
        {
            var userToUpdated = _usersService.GetUserById(id);
            if (userToUpdated is not null && isUthorizedUser.Id == id)
            {
                _usersService.UpdateUser(userUpdate);
                return Ok("User successfully updated");
            }
            else BadRequest("Cannot update user");
        }
        // non è più valido allora ritorno Unhautorized.
        return Unauthorized("Invalid token");
    }
}

