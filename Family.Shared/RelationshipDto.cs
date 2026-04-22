namespace Family.Shared;

public class RelationshipDto
{
    public int FamilyMemberId { get; set; }
    public int RelatedMemberId { get; set; }
    public int RelationTypeId { get; set; }

    // Пустой конструктор важен для работы JSON-сериализатора
    public RelationshipDto() { }

    // Опциональный конструктор для удобства создания в коде
    public RelationshipDto(int memberId, int relatedId, int typeId)
    {
        FamilyMemberId = memberId;
        RelatedMemberId = relatedId;
        RelationTypeId = typeId;
    }
}
