public class FamilyRelationType
{
    public int Id { get; set; }
    public string Code { get; set; }          // Например: "father", "mother", "son" и т.д.
    public string DisplayName { get; set; }   // То, как показываем пользователю: "Отец", "Мама", "Сын"
    public int? ReverseRelationTypeId { get; set; } // FK на обратную связь (например, "Отец" <-> "Сын")
    public FamilyRelationType? ReverseRelationType { get; set; } // навигация
    public string? Emoji { get; set; }        // Например: "👨‍🦰"
    public string? Description { get; set; }  // Дополнительное описание
    public RelationBaseType? BaseType { get; set; } // (связь с enum, если используешь совмещение)
}

public enum RelationBaseType
{
    Unknown = 0,
    Father = 1,
    Mother = 2,
    Son = 3,
    Daughter = 4,
    Brother = 5,
    Sister = 6,
    Husband = 7,
    Wife = 8
    // ... можно добавить основные типы
}
