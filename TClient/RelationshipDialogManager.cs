public class RelationshipDialogManager
{
    private class State
    {
        public int FamilyMemberId;
        public int RelatedMemberId;
        public int RelationTypeId;
        public Step Stage = Step.WaitingForRelative;
        public enum Step { WaitingForRelative, WaitingForType, Finished }
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
            state.Stage = State.Step.WaitingForType;
        }
    }

    public (string? nextPrompt, Relationship? result) ProcessInput(long chatId, int relationTypeId)
    {
        if (!_dialogStates.TryGetValue(chatId, out var state))
            return ("Ошибка состояния. Начните сначала.", null);

        if (state.Stage == State.Step.WaitingForType)
        {
            state.RelationTypeId = relationTypeId;
            state.Stage = State.Step.Finished;
            var rel = new Relationship(state.FamilyMemberId, state.RelatedMemberId, state.RelationTypeId);
            _dialogStates.Remove(chatId);
            return (null, rel);
        }
        else
        {
            return ("Ошибка состояния. Начните сначала.", null);
        }
    }
}
