using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Family.Shared;
using Family.Mobile.Services;

namespace Family.Mobile.ViewModels;

public partial class AddMemberViewModel : ObservableObject
{
    private readonly FamilyService _service;

    [ObservableProperty]
    private FamilyMemberDto newMember = new() { DateOfBirth = DateTime.Now };

    public List<string> Genders { get; } = Enum.GetNames(typeof(Gender)).ToList();

    [ObservableProperty]
    private string selectedGender = "Unknown";

    public AddMemberViewModel(FamilyService service) => _service = service;

    [RelayCommand]
    async Task Save()
    {
        // Присваиваем выбранный пол
        if (Enum.TryParse<Gender>(SelectedGender, out var gender))
            NewMember.Gender = gender;

        var success = await _service.AddMemberAsync(NewMember);

        if (success)
            await Shell.Current.GoToAsync(".."); // Возврат к списку
        else
            await App.Current.MainPage.DisplayAlert("Ошибка", "Не удалось сохранить данные", "ОК");
    }
}
