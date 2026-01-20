public class Relationship
{
    public int Id { get; set; }
    public int FamilyMemberId { get; set; }
    public FamilyMember FamilyMember { get; set; }

    public int RelatedMemberId { get; set; }
    public FamilyMember RelatedMember { get; set; }

    public string Description { get; set; } // Описание связи (например, "брат", "сестра")
}
