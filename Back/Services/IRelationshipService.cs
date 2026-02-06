public interface IRelationshipService
{
    /// <summary>
    /// Получает обратное описание связи (relation) с учетом пола, если передан.
    /// </summary>
    /// <param name="relation">Тип связи, например, "отец", "мать", "брат"</param>
    /// <param name="gender">"male" или "female" (опционально, если нужно тонко разграничивать "сын" и "дочь")</param>
    /// <returns>Текст для обратной связи (например, "Сын", "Дочь")</returns>
    string? GetReverseRelation(string relation, Gender? gender = null);
}

public class RelationshipService : IRelationshipService
{
    public string? GetReverseRelation(string relation, Gender? gender)
    {
        switch (relation.Trim().ToLowerInvariant())
        {
            case "отец":
            case "мать":
                return gender switch
                {
                    Gender.Female => "Дочь",
                    Gender.Male => "Сын",
                    _ => "Ребенок"
                };
            case "сын":
                return "Отец";
            case "дочь":
                return "Мать";
            case "брат":
                return gender == Gender.Female ? "Сестра" : "Брат";
            case "сестра":
                return gender == Gender.Male ? "Брат" : "Сестра";
            case "муж":
                return "Жена";
            case "жена":
                return "Муж";
            default:
                return null;
        }
    }

}
