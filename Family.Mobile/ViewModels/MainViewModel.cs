using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Family.Mobile.Services;
using Family.Mobile.Views;
using Family.Shared;
using System.Collections.ObjectModel;

namespace Family.Mobile.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly FamilyService _familyService;

    // Список, к которому привяжется UI
    [ObservableProperty]
    private ObservableCollection<FamilyMemberDto> members = new();

    [ObservableProperty]
    private bool isRefreshing;

    public MainViewModel(FamilyService familyService)
    {
        _familyService = familyService;
    }

    // Команда для загрузки данных
    [RelayCommand]
    public async Task LoadMembersAsync()
    {
        IsRefreshing = true;

        var result = await _familyService.GetMembersAsync();

        Members.Clear();
        foreach (var member in result)
        {
            Members.Add(member);
        }

        IsRefreshing = false;
    }

    [RelayCommand]
    public async Task DeleteMemberAsync(FamilyMemberDto member)
    {
        bool confirm = await Shell.Current.DisplayAlert("Подтверждение", $"Удалить {member.FirstName}?", "Да", "Нет");
        if (!confirm) return;

        var success = await _familyService.DeleteMemberAsync(member.Id);
        if (success)
        {
            Members.Remove(member);
        }
    }

    [RelayCommand]
    async Task GoToAddMember()
    {
        // Переходим на страницу AddMemberPage
        await Shell.Current.GoToAsync(nameof(AddMemberPage));
    }


}
