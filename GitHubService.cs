// Sets information for GitHub API to specific repo

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WinFormsApp1
{
    public class GitHubService
    {
        private readonly HttpClient _client;
        private readonly string _owner;
        private readonly string _repo;
        private readonly string _path;
        private string? _currentFileSha; // Changed to nullable string to fix warning

        // Hardcoded path based on your request, but PAT is passed in
        public GitHubService(string personalAccessToken)
        {
            _owner = "kyofyufufufufufufufu";
            _repo = "test_database1";
            _path = "jsonTest.json";

            _client = new HttpClient();

            // GitHub API requires a User-Agent header for PAT authorization
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("WinFormsApp", "1.0"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", personalAccessToken);
        }

        public async Task<QuestionSet> GetDatabaseAsync()
        {
            string url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}";

            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(content);

            // Tells GitHub which version of the file we are overwriting later
            // Used null-forgiving operator (!) as 'sha' is expected from GitHub API
            _currentFileSha = json["sha"]?.ToString()!; 

            // Cleaner look
            // Used null-forgiving operator (!) as 'content' is expected from GitHub API
            string base64Content = json["content"]?.ToString()!; 
            base64Content = base64Content.Replace("\n", "");

            byte[] data = Convert.FromBase64String(base64Content);
            string decodedString = Encoding.UTF8.GetString(data);

            // Used null-forgiving operator (!) as we expect a valid QuestionSet from the JSON
            return JsonConvert.DeserializeObject<QuestionSet>(decodedString)!;
        }

        public async Task SaveDatabaseAsync(QuestionSet data)
        {
            // Explicitly check for null SHA before using it
            if (_currentFileSha == null)
            {
                throw new InvalidOperationException("Cannot save database: File SHA must be loaded by calling GetDatabaseAsync first.");
            }
            
            string url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{_path}";

            string jsonContent = JsonConvert.SerializeObject(data, Formatting.Indented);

            string base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonContent));

            // Creates Payload
            var body = new
            {
                message = "Update via WinForms App",
                content = base64Content,
                sha = _currentFileSha
            };

            var httpContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PutAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"GitHub API Error: {response.StatusCode} - {error}");
            }

            // Save again without reloading
            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseJson = JObject.Parse(responseString);
            // Used null-forgiving operator (!) as 'sha' is expected from GitHub API response
            _currentFileSha = responseJson["content"]?["sha"]?.ToString()!;
        }
    }
}