using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Family.Shared;

public class FamilyMemberDto
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 50 символов")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 50 символов")]
    public string LastName { get; set; } = string.Empty;
    public string? ParentName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Дата рождения обязательна")]
    [DataType(DataType.Date, ErrorMessage = "Введите корректную дату")]
    public DateTime DateOfBirth { get; set; }
    [DataType(DataType.Date, ErrorMessage = "Введите корректную дату")]
    public DateTime? DateOfDeath { get; set; }
    [JsonIgnore]
    [Required(ErrorMessage = "Пол обязателен для выбора")]
    public Gender Gender { get; set; } // Теперь здесь ENUM
    [Display(Name = "Профессия")]
    public string? Occupation { get; set; } // Профессия
    [Display(Name = "Биография")]
    [StringLength(1000, ErrorMessage = "Биография не должна превышать 1000 символов")]
    public string? Biography { get; set; }
    [Display(Name = "Отец")]
    public int? FatherId { get; set; }
    [Display(Name = "Мать")]
    public int? MotherId { get; set; }
    public List<int> SpouseIds { get; set; } = new();
    public string? PhotoUrl { get; set; } // Ссылка на изображение




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