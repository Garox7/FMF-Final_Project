using Newtonsoft.Json;

namespace UserManager.Models;


public class User
{
    public string Id { get; }
    [JsonProperty]
    public string Username { get; set; }
    [JsonProperty]
    private string _password;
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Token { get; private set;}
    [JsonProperty]
    private DateTime _tokenExpirationDate {get; set;}
    [JsonProperty]
    private bool _isBlocked;
    private string? _challenge;

    [JsonConstructor]
    public User(string id, string username, string password, string name, string surname, string token, DateTime tokenExpirationDate)
    {
        Id = id;
        Username = username;
        _password = password;
        Name = name;
        Surname = surname;
        Token = token;
        _tokenExpirationDate = tokenExpirationDate;
        _isBlocked = false;
    }

    public User(string id, string username, string name, string surname)
    {
        Id = id;
        Username = username;
        _password = "pass";
        Name = name;
        Surname = surname;
        Token = "";
        _isBlocked = false;
    }

    internal bool ChangeState()
    {
        if (_isBlocked is true)
        {
            return _isBlocked = false;
        }
        else return _isBlocked = true;
    }

    internal string GenerateChallenge()
    {
        // Generate challenge
        _challenge = "VR";

        return _challenge;
    }

    private string? ResolveChallenge(string challenge)
    {
        if (challenge == _password + _challenge)
            return $"{_password}{_challenge}";
        return null;
    }

    internal bool VerifyChallenge(string challenge)
    {
        if (_challenge is null)
        {
            return false;
        }
        return challenge == ResolveChallenge(challenge);
    }

    internal string GenerateToken()
    {
        
        Token = Guid.NewGuid().ToString();
        _tokenExpirationDate = DateTime.UtcNow.AddHours(6);

        return Token;
    }

    internal bool VerifyToken(string token)
    {
        if (_tokenExpirationDate > DateTime.UtcNow) {
            return token == Token;
        }
        else throw new Exception("The Token has expired or not valid");
    }

    internal void UpdateUser(UserDto userDto)
    {
        Username = userDto.Username;
        Name = userDto.Name;
        Surname = userDto.Surname;
    }
}