using Newtonsoft.Json;

public class FamilyRelationType
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public string Code { get; set; }

    private static async Task<List<FamilyRelationType>> GetRelationTypesAsync()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://localhost:5274/api/relationtypes");
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<FamilyRelationType>>(json);
    }
}