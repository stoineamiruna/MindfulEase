using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MindfulEase.Services
{

    namespace MindfulEase.Services
    {
        public class SentimentAnalysisService
        {
            private readonly HttpClient _httpClient;

            public SentimentAnalysisService(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public async Task<List<dynamic>> AnalyzeEmotionAsync(string text)
            {
                var content = new StringContent(JsonConvert.SerializeObject(new { text = text }), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5000/analyze-emotion", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    // Deserializăm rezultatul intr-o lista de tip dynamic
                    return JsonConvert.DeserializeObject<List<dynamic>>(result);
                }

                return null;  // In caz de eroare, returnam null
            }


        }
    }

}
