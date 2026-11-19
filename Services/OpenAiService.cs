using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FitForge.Api.Services;

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OpenAiService(HttpClient httpclient, IConfiguration configuration)
    {
        _httpClient = httpclient;
        _configuration = configuration;
    }

    public async Task<string> GenerateWorkoutPlanTextAsync(string fitnessGoal, string experienceLevel,
        string equipmentAvailable)
    {
        var apiKey = _configuration["OpenAi:ApiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Brak klucza.");
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        
        var prompt = $"Jesteś profesjonalnym trenerem personalnym. Stwórz spersonalizowany plan treningowy dla użytkownika o następujących parametrych:\n" +
                     $"- Cel: {fitnessGoal}\n" +
                     $"- Poziom doświadczenia: {experienceLevel}\n" +
                     $"- Dostępny sprzęt: {equipmentAvailable}\n\n" +
                     "Zwróć plan w czytelnym formacie tekstowym, podając nazwy dni tygodnia, nazwy ćwiczeń, liczbę serii oraz liczbę powtórzeń.";

        var requestBody = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new
                {
                    role = "system", content = "Jesteś ekspertem fitness i programem AI generującym plany treningowe."
                },
                new { role = "user", content = prompt }
            },
            temperature = 0.7
        };
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            throw new Exception($"Błąd komunikacji z OpenAI API: {response.ReasonPhrase}. Szczegóły: {errorDetails}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        
        var textResponse = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return textResponse ?? "Nie udało się wygenerować planu.";
    }
}