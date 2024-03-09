using Newtonsoft.Json;

namespace FileManager;
public class DataManager<T>
{
    private readonly string dataFilePath;

    public DataManager(string dataFilePath)
    {
        this.dataFilePath = dataFilePath;
    }

    public async Task<T> LoadData<T>()
    {
        if (File.Exists(dataFilePath))
        {
            using (StreamReader reader = new StreamReader(dataFilePath))
            {
                string jsonData = await File.ReadAllTextAsync(dataFilePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
        }
        throw new Exception("file not found") ;
    }

    public async Task SaveData<T>(T data)
    {
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        await File.WriteAllTextAsync(dataFilePath, jsonData);
    }
}