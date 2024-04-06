using CsvToMongo.Models;
using Hangfire;
using MongoDB.Driver;

namespace CsvToMongo.Services;

public class HumanService
{
    private readonly HttpClient _httpClient;
    public HumanService()
    {
        _httpClient = new HttpClient();
        RecurringJob.AddOrUpdate(() => DownloadCsv(), Cron.Daily);

    }
    public async Task DownloadCsv()
    {
        var respons = await _httpClient.GetAsync("https://people.sc.fsu.edu/~jburkardt/data/csv/addresses.csv");
        var csvData = await respons.Content.ReadAsStringAsync();
        var humans = ParseData(csvData);

        string connectionString = "mongodb://localhost:27017";
        MongoClient client = new MongoClient(connectionString);
        IMongoDatabase database = client.GetDatabase("test");
        IMongoCollection<Human> collection = database.GetCollection<Human>("humans");

        collection.InsertMany(humans);
    }
    static List<Human> ParseData(string data)
    {
        List<Human> humans = new List<Human>();
        string[] lines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            string[] fields = line.Split(',');
            Human human = new Human();

            // Assuming the data always follows the given format
            human.Name = fields[0].Trim();
            human.SecondName = fields[1].Trim();
            human.Street = fields[2].Trim();
            human.Idk = fields[3].Trim();
            human.Ab = fields[4].Trim();
            human.NumberXd = fields[5].Trim();

            humans.Add(human);
        }

        return humans;
    }
}
