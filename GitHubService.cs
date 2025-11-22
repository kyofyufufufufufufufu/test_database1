// Sets information for GitHub API to specific repo
// Includes image uploading and direct path to image URL
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WinFormsApp1
{
    public class GitHubService
    {
        private readonly HttpClient _client;
        private readonly string _owner;
        private readonly string _repo;
        private readonly string _path;
        private readonly string _imagesPath = "images";
        private string? _currentFileSha;

        // Hardcoded path for test database, will change when necessary
        public GitHubService(string personalAccessToken)
        {
            _owner = "kyofyufufufufufufufu";
            _repo = "test_database1";
            _path = "jsonTest.json";

            _client = new HttpClient();

            // GitHub API PAT authorization
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

            _currentFileSha = json["sha"]?.ToString()!;

            string base64Content = json["content"]?.ToString()!;
            base64Content = base64Content.Replace("\n", "");

            byte[] data = Convert.FromBase64String(base64Content);
            string decodedString = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject<QuestionSet>(decodedString)!;
        }

        // Uploading image to GitHub images folder
        public async Task<string> UploadImageAsync(string localFilePath)
        {
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException($"Image file not found at: {localFilePath}");
            }

            byte[] fileBytes = File.ReadAllBytes(localFilePath);
            string base64Content = Convert.ToBase64String(fileBytes);

            string fileName = Path.GetFileName(localFilePath);
            string targetPath = $"{_imagesPath}/{fileName}";

            string? sha = null;
            string checkUrl = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{targetPath}";
            try
            {
                // GET request
                HttpResponseMessage checkResponse = await _client.GetAsync(checkUrl);
                if (checkResponse.IsSuccessStatusCode)
                {
                    string content = await checkResponse.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(content);
                    sha = json["sha"]?.ToString();
                }
            }
            catch (Exception)
            {
                // If fails assume the file doesn't exist
            }

            // Upload payload
            var body = new
            {
                message = $"Add/Update image: {fileName} via WinForms App",
                content = base64Content,
                sha = sha
            };

            string url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{targetPath}";
            var httpContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            // PUT request
            HttpResponseMessage response = await _client.PutAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                throw new Exception($"GitHub API Error uploading image: {response.StatusCode} - {error}");
            }

            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseJson = JObject.Parse(responseString);

            // Permanent, public link for file
            return responseJson["content"]?["download_url"]?.ToString()!;
        }

        public async Task SaveDatabaseAsync(QuestionSet data)
        {
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
            _currentFileSha = responseJson["content"]?["sha"]?.ToString()!;
        }
    }
}