using UserManager.Models;
using FileManager;

namespace UserManager.Services;
public class UsersService : IUsersService
{
    private List<User> _users = new();
    private readonly string _filePath = "./Data/Users.json";
    private readonly DataManager<List<User>> _dataManager;

    public UsersService()
    {
        _dataManager = new DataManager<List<User>>(_filePath);
        LoadUsers().Wait();
    }

    public async Task LoadUsers()
    {
        try
        {
            _users = await _dataManager.LoadData<List<User>>();
        }
        catch (Exception e)
        {
            throw new Exception($"Errore durante il caricamento dei dati: {e}");
        }
    }

    public async Task SaveUsers()
    {
        try
        {
            await _dataManager.SaveData(_users);
        }
        catch (Exception e)
        {
            throw new Exception($"Errore durante il salvataggio dei dati: {e}");
        }
    }

    public List<User> GetAll()
    {
        if (_users is not null)
        {
            return _users;
        }
        else throw new Exception("Users not found");
    }

    public User GetUserById(string id)
    {
        User user = _users.First(u => u.Id == id);

        if (user is not null)
        {
            return user;
        }
        else throw new Exception("User not Found");
    }

    public User? GetUserByUsername(string username)
    {
        User? user = _users.FirstOrDefault(u => u.Username == username);
        if (user is not null)
        {
            return user;
        }
        else return null;
    }

    public User? GetUserByToken(string token)
    {
        User? user = _users.Find(u => u.Token == token);
        if (user is not null && user.VerifyToken(token))
        {
            return user;
        }
        else throw new Exception("User not Found");
    }

    public UserDto CreateUser(UserDto userDto)
    {
        User newUser = new User(
            userDto.Id,
            userDto.Username,
            userDto.Name,
            userDto.Surname
        );

        _users.Add(newUser);
        SaveUsers().Wait();

        return UserDto.FromUser(newUser);
    }

    public UserDto UpdateUser(UserDto userDto)
    {
        User? userToUpdate = GetUserById(userDto.Id);

        userToUpdate.UpdateUser(userDto);

        SaveUsers().Wait();

        return UserDto.FromUser(userToUpdate);
        
    }

    public bool DeleteUser(string id)
    {
        User? userToDelete = GetUserById(id);

        if (userToDelete is not null)
        {
            if (userToDelete.ChangeState())
            {
                SaveUsers().Wait();
                return true;
            }
            return false;
        }
        else throw new Exception("User Not found");
    }

    public List<UserDto> ToUserDto(List<User> UsersList)
    {
        var ToListDto = new List<UserDto>();

        foreach (var user in _users)
        {
            UserDto userDto = UserDto.FromUser(user);
            ToListDto.Add(userDto);
        }
        return ToListDto;
    }
}