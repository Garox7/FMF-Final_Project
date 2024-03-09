using UserManager.Models;

namespace UserManager.Services;
public interface IUsersService
{
  List<User> GetAll();
  User? GetUserById(string id);
  User? GetUserByUsername(string username);
  User? GetUserByToken(string token);
  UserDto CreateUser(UserDto userDto);
  UserDto UpdateUser(UserDto userDto);
  bool DeleteUser(string id);
  List<UserDto> ToUserDto(List<User> UsersList);
}