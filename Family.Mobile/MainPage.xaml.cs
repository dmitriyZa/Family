using Family.Mobile.ViewModels;
using Family.Mobile.Views;

namespace Family.Mobile;

public partial class MainPage : ContentPage
{
    private readonly IServiceProvider _services;

    public MainPage()
    {
        //InitializeComponent();
        //BindingContext = viewModel;
        //_services = services;
        Title = "Main";
        Button toCommonPageBtn = new Button
        {
            Text = "Common",
            HorizontalOptions = LayoutOptions.Start
        };
        toCommonPageBtn.Clicked += OnAddClicked;

        Content = new StackLayout { Children = { toCommonPageBtn} };
    }

 //   protected override async void OnAppearing()
	//{
	//	base.OnAppearing();
	//	var vm = (ViewModels.MainViewModel)BindingContext;
	//	await vm.LoadMembersAsync();
	//}

    private async void OnAddClicked(object sender, EventArgs e)
    {        
        {
            try
            {
                await Navigation.PushModalAsync(new ModalPage());
                // Попробуй сначала ТЕСТ 1 еще раз через Dispatcher
                //await Navigation.PushAsync(new ContentPage { Title = "Тест", Content = new Label { Text = "Привет" } });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", ex.Message, "ОК");
            }
        };

        //try
        //{

        //    // ТЕСТ 2: Проверяем создание только ViewModel
        //    var vm = _services.GetService<AddMemberViewModel>();
        //    if (vm != null) await DisplayAlert("ОК", "ViewModel создана", "ОК");

        //    // ТЕСТ 3: Проверяем создание страницы
        //    var addPage = _services.GetService<AddMemberPage>();
        //    await Navigation.PushAsync(addPage);
        //}
        //catch (Exception ex)
        //{
        //    await DisplayAlert("Ошибка", ex.Message, "ОК");
        //}        
    }

}
