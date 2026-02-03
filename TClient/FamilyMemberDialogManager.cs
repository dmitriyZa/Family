public class FamilyMemberDialogManager
{
    private readonly Dictionary<long, FamilyMemberDialogState> _dialogs = new();

    public string StartDialog(long chatId)
    {
        _dialogs[chatId] = new FamilyMemberDialogState();
        return "Введите имя:";
    }

    public (string? reply, FamilyMember? result) ProcessInput(long chatId, string input)
    {
        if (!_dialogs.TryGetValue(chatId, out var state))
            return ("Начните с команды 'Добавить члена семьи'", null);

        switch (state.Step)
        {
            case FamilyMemberDialogState.DialogStep.FirstName:
                state.FirstName = input;
                state.Step = FamilyMemberDialogState.DialogStep.LastName;
                return ("Введите фамилию:", null);

            case FamilyMemberDialogState.DialogStep.LastName:
                state.LastName = input;
                state.Step = FamilyMemberDialogState.DialogStep.DateOfBirth;
                return ("Введите дату рождения (ГГГГ-ММ-ДД):", null);

            case FamilyMemberDialogState.DialogStep.DateOfBirth:
                if (DateTime.TryParse(input, out var date))
                {
                    state.DateOfBirth = date;
                    state.Step = FamilyMemberDialogState.DialogStep.Biography;
                    return ("Введите биографию:", null);
                }
                else
                {
                    return ("Неверный формат даты. Попробуйте ещё раз (ГГГГ-ММ-ДД):", null);
                }

            case FamilyMemberDialogState.DialogStep.Biography:
                state.Biography = input;
                state.Step = FamilyMemberDialogState.DialogStep.Finished;

                var member = new FamilyMember
                {
                    FirstName = state.FirstName,
                    LastName = state.LastName,
                    DateOfBirth = state.DateOfBirth ?? DateTime.MinValue,
                    Biography = state.Biography
                };
                _dialogs.Remove(chatId);
                return (null, member); // всё заполнено, форма готова

            default:
                _dialogs.Remove(chatId);
                return (null, null);
        }
    }
}
