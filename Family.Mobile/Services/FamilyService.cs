using System.Diagnostics;
using System.Net.Http.Json;
using Family.Shared; // Подключаем нашу общую библиотеку

namespace Family.Mobile.Services;

public class FamilyService
{
    private readonly HttpClient _httpClient;

    // 10.0.2.2 — специальный адрес в Android-эмуляторе для связи с localhost твоего ПК.
    // Если запускаешь на реальном устройстве, замени на свой IP (например, 192.168.1.5)
    private const string BaseUrl = "http://10.0.2.2:5274/api/Family/";

    public FamilyService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    // Получаем всех членов семьи
    public async Task<List<FamilyMemberDto>> GetMembersAsync()
    {
        try
        {
            // Метод GetFromJsonAsync сам десериализует JSON в список объектов
            var res= await _httpClient.GetFromJsonAsync<List<FamilyMemberDto>>("all");
            return res;
        }
        catch (Exception ex)
        {
            // Здесь можно добавить логирование ошибки
            Debug.WriteLine($"Ошибка Сети : {ex.Message}");
            return new List<FamilyMemberDto>();
        }
    }

    // Добавляем новую связь
    public async Task<bool> AddRelationshipAsync(RelationshipDto relationship)
    {
        var response = await _httpClient.PostAsJsonAsync("relationship", relationship);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> AddMemberAsync(FamilyMemberDto member)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Family", member); // Путь "Family" согласно вашему контроллеру
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeleteMemberAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"Family/{id}");
        return response.IsSuccessStatusCode;
    }


    // Получаем справочник типов отношений
    public async Task<List<FamilyRelationTypeDto>> GetRelationTypesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<FamilyRelationTypeDto>>("relationtypes") ?? new();
        }
        catch { return new List<FamilyRelationTypeDto>(); }
    }
}
