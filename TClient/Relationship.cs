using Newtonsoft.Json;

public class Relationship
{
    public int FamilyMemberId { get; set; }
    public int RelatedMemberId { get; set; }
    public string Description { get; set; }


    public async Task<bool> AddRelationshipAsync(int familyMemberId, int relatedMemberId, string description)
    {
        var relationship = new Relationship
        {
            FamilyMemberId = familyMemberId,
            RelatedMemberId = relatedMemberId,
            Description = description // Например, "отец", "жена"
        };

        using var httpClient = new HttpClient();
        var json = JsonConvert.SerializeObject(relationship);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("http://localhost:5274/api/relationship", content);

        return response.IsSuccessStatusCode;
    }
}