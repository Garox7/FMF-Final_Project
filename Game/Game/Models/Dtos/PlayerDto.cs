namespace Game.Models;
public class PlayerDto
{
    public string Id { get; }
    public string Username { get; }
    public string Name { get; }
    public string Surname { get; }
    public string Token { get; }

    public PlayerDto(string id, string username, string name, string surname, string token)
    {
        Id = id;
        Username = username;
        Name = name;
        Surname = surname;
        Token = token;
    }
}