namespace UserManager.Models;

public class UserDto
{
    public string Id { get; }
    public string Username { get; }
    public string Name { get; }
    public string Surname { get; }
    public string? Token { get; }

    public UserDto(string id, string username, string name, string surname, string? token)
    {
        Id = id;
        Username = username;
        Name = name;
        Surname = surname;
        Token = token;
    }
    
     public static UserDto FromUser(User user)
    {
        return new UserDto(
            user.Id,
            user.Username,
            user.Name,
            user.Surname,
            user.Token
        );
    }
}