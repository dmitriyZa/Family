namespace Family.Mobile.Views; // Должно быть именно так

public partial class AddMemberPage : ContentPage
{
    public AddMemberPage(ViewModels.AddMemberViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
