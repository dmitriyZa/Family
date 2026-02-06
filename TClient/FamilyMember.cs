public record FamilyMember
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ParentName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Biography { get; set; }
    public string? RelationshipDegree { get; set; }
}