public class FamilyMemberDialogState
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ParentName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Biography { get; set; }
    public string? RelationshipDegree { get; set; }
    public Gender? Gender { get; set; }
    public DialogStep Step { get; set; } = DialogStep.FirstName;

<<<<<<< HEAD
    public enum DialogStep { FirstName, LastName, ParentName, DateOfBirth, Gender, Biography, Finished }

=======
    public enum DialogStep
    {
        FirstName,
        LastName,
        ParentName,
        DateOfBirth,
        Biography,
        Finished
    }
>>>>>>> 0964266c5af2f0d27fbddc00cd6c4e53da05f70d
}
