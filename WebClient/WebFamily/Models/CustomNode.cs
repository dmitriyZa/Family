using Blazor.FamilyTreeJS.Components.Interop.Options;
using System.Text.Json.Serialization;

namespace WebFamily.Models
{
    // Наследуемся от базового класса Node, чтобы библиотека могла его отрисовать
    public record CustomNode : Node
    {
        // Дополнительные поля из твоего DTO
        public string? Biography { get; init; }
        public string? BirthDate { get; init; }
        

        [JsonPropertyName("fid")]
        public string? FatherId { get; set; }

        [JsonPropertyName("mid")]
        public string? MotherId { get; set; }
    }
}
