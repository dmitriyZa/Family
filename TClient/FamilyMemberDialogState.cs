public class FamilyMemberDialogState
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Biography { get; set; }
    public string? RelationshipDegree { get; set; }
    public DialogStep Step { get; set; } = DialogStep.FirstName;

    public enum DialogStep
    {
        FirstName,
        LastName,
        DateOfBirth,
        Biography,
        Finished
    }
}
