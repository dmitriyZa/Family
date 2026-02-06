public class FamilyMember
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ParentName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Biography { get; set; }
    public Gender Gender { get; set; } // <--- используй enum
    public ICollection<Relationship>? Relationships { get; set; }
}

public enum Gender
{
    Unknown = 0,
    Male = 1,
    Female = 2
}


