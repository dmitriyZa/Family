using Blazor.FamilyTreeJS.Components.Interop.Options;

namespace WebFamily.Models
{
    // Наследуемся от базового класса Node, чтобы библиотека могла его отрисовать
    public record CustomNode : Node
    {
        // Дополнительные поля из твоего DTO
        public string? Biography { get; init; }
        public string? BirthDate { get; init; }
    }
}
