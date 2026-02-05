public class RelationshipDialogManager
{
    private class State
    {
        public int FamilyMemberId;
        public int? RelatedMemberId;
        public string? Description;
        public Step Stage = Step.WaitingForRelative;
        public enum Step { WaitingForRelative, WaitingForDescription, Finished }
    }

    private readonly Dictionary<long, State> _dialogStates = new();

    public void Start(long chatId, int familyMemberId)
    {
        _dialogStates[chatId] = new State { FamilyMemberId = familyMemberId };
    }
    public bool IsActive(long chatId) => _dialogStates.ContainsKey(chatId);

    public void SetRelatedMember(long chatId, int relatedMemberId)
    {
        if (_dialogStates.TryGetValue(chatId, out var state))
        {
            state.RelatedMemberId = relatedMemberId;
            state.Stage = State.Step.WaitingForDescription;
        }
    }

    public (string? nextPrompt, Relationship? result) ProcessInput(long chatId, string input, bool isRelationType = false)
    {
        if (!_dialogStates.TryGetValue(chatId, out var state)) return (null, null);

        if (state.Stage == State.Step.WaitingForDescription || isRelationType)
        {
            state.Description = input;
            state.Stage = State.Step.Finished;
            var rel = new Relationship
            {
                FamilyMemberId = state.FamilyMemberId,
                RelatedMemberId = state.RelatedMemberId ?? 0,
                Description = state.Description
            };
            _dialogStates.Remove(chatId);
            return (null, rel);
        }
        else
        {
            return ("Ошибка состояния. Начните сначала.", null);
        }
    }

}

