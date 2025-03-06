using System.Text.Json;
using Json.Components.Entities;


namespace Json.Components.Service.Data
{
    public class JsonFileService
    {
        private readonly IWebHostEnvironment _env;

        public JsonFileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<List<Skill>?> ReadJsonFileAsync(string fileName)
        {
            var filePath = Path.Combine(_env.WebRootPath, "json-files", fileName);

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"❌ File not found: {filePath}");
                return null;
            }

            var jsonContent = await File.ReadAllTextAsync(filePath);
            Console.WriteLine($"📖 Reading file: {filePath}");

            try
            {
                return JsonSerializer.Deserialize<List<Skill>>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"❌ JSON Parsing Error: {ex.Message}");
                return null;
            }
        }


        public List<string> GetJsonFileNames()
        {
            var folderPath = Path.Combine(_env.WebRootPath, "json-files");
            if (!Directory.Exists(folderPath)) return new List<string>();

            return Directory.GetFiles(folderPath, "*.json")
                            .Select(file => Path.GetFileName(file)!)
                            .ToList();
        }
    }
}