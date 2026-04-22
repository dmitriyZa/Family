using System.Text.Json.Serialization;
namespace Family.Shared;

public class FamilyMemberDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    [JsonIgnore]
    public Gender Gender { get; set; } // Теперь здесь ENUM
    public string? Biography { get; set; }
    public string? ParentName { get; set; }


    [JsonPropertyName("gender")]
    public string GenderString
    {
        get => Gender.ToString();
        set
        {
            if (Enum.TryParse<Gender>(value, true, out var result))
            {
                Gender = result;
            }
            else
            {
                Gender = Gender.Unknown;
            }
        }
    }
}