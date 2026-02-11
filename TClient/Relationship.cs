using System.Text;
using Newtonsoft.Json;

public class Relationship
{
    public int FamilyMemberId { get; }
    public int RelatedMemberId { get; }
    public int RelationTypeId { get; }

    // Конструктор с параметрами
    public Relationship(int familyMemberId, int relatedMemberId, int relationTypeId)
    {
        FamilyMemberId = familyMemberId;
        RelatedMemberId = relatedMemberId;
        RelationTypeId = relationTypeId;
    }

    // Instance-метод для добавления через API
    public async Task<bool> AddAsync()
    {
        using var httpClient = new HttpClient();
        var json = JsonConvert.SerializeObject(this);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("http://localhost:5274/api/relationship", content);
        return response.IsSuccessStatusCode;
    }

    // Статичный метод для получения справочника
    public static async Task<List<FamilyRelationType>> GetRelationTypesAsync()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://localhost:5274/api/relationtypes");
        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<FamilyRelationType>>(json);
    }
}
