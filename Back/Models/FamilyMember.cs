using Family.Shared;

public class FamilyMember
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Biography { get; set; }
    public Gender Gender { get; set; } // <--- используй enum
    public ICollection<Relationship>? Relationships { get; set; }
}




