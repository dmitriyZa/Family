public class FamilyRelationTypeDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public int? ReverseRelationTypeId { get; set; } // Только Id, не объект!
    public string? Emoji { get; set; }
    public string? Description { get; set; }
    public RelationBaseType? BaseType { get; set; }
}
