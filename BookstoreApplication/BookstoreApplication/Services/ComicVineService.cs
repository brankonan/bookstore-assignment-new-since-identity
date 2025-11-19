using BookstoreApplication.DTOs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

public class ComicVineService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public ComicVineService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["ComicVine:ApiKey"];
        _baseUrl = config["ComicVine:BaseUrl"] ?? "https://comicvine.gamespot.com/api";

        if (_http.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("BookstoreApp/1.0 (+https://example.com)");
        }
    }

    public async Task<List<ComicVolumeDto>> SearchVolumesAsync(string name)
    {
        var url =
            $"{_baseUrl}/volumes/?" +
            $"api_key={_apiKey}&format=json&filter=name:{Uri.EscapeDataString(name)}";

        var json = await SendAndGetContentAsync(url);

        var obj = JObject.Parse(json);
        var results = obj["results"];
        if (results == null) return new List<ComicVolumeDto>();

        return results
            .Select(v => new ComicVolumeDto
            {
                Id = (int)v["id"],
                Name = (string)v["name"],
                Publisher = (string)v["publisher"]?["name"],
                StartYear = (string)v["start_year"],
                ImageUrl = (string)v["image"]?["icon_url"]
            })
            .ToList();
    }

    public async Task<List<ComicIssueDto>> GetIssuesForVolumeAsync(int volumeId)
    {
        var url =
            $"{_baseUrl}/issues/?" +
            $"api_key={_apiKey}&format=json&filter=volume:{volumeId}";

        var json = await SendAndGetContentAsync(url);

        var obj = JObject.Parse(json);
        var results = obj["results"];
        if (results == null) return new List<ComicIssueDto>();

        return results
            .Select(i => new ComicIssueDto
            {
                Id = (int)i["id"],
                Name = (string)i["name"],
                CoverUrl = (string)i["image"]?["icon_url"],
                IssueNumber = (string)i["issue_number"],
                Description = (string)i["description"],
                ReleaseDate = (string)i["cover_date"],
                PageCount = (int?)i["page_count"] ?? 0
            })
            .ToList();
    }

    public async Task<ComicIssueDto?> GetSingleIssueAsync(int issueId)
    {
        var url =
            $"{_baseUrl}/issue/4000-{issueId}/?" +
            $"api_key={_apiKey}&format=json";

        var json = await SendAndGetContentAsync(url);

        var obj = JObject.Parse(json);
        var i = obj["results"];
        if (i == null) return null;

        return new ComicIssueDto
        {
            Id = (int)i["id"],
            Name = (string)i["name"],
            CoverUrl = (string)i["image"]?["icon_url"],
            IssueNumber = (string)i["issue_number"],
            Description = (string)i["description"],
            ReleaseDate = (string)i["cover_date"],
            PageCount = (int?)i["page_count"] ?? 0
        };
    }

    private async Task<string> SendAndGetContentAsync(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        var response = await _http.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"ComicVine API error: {(int)response.StatusCode} {response.ReasonPhrase}. Content: {content}");
        }

        return content;
    }
}
