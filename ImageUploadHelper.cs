using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;
namespace WebApiNew
{
    public static class ImageUploadHelper
    {
        public static async Task<string?> UploadToImgBB(string base64Image)
        {
            string apiKey = "fd3b6586733a864caf4f8e9c9b5fba07";
            string url = $"https://api.imgbb.com/1/upload?key={apiKey}";

            using var client = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("image", base64Image)
        });

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result?.data?.url;
        }
    }
}