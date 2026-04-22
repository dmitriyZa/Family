using Microsoft.Extensions.DependencyInjection;

namespace Family.Mobile;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
        // Страница должна быть внутри NavigationPage, иначе PushAsync не сработает
        // БЕЗ NavigationPage навигация (кнопка Добавить) работать не будет
        MainPage = new NavigationPage(new MainPage());
    }
	
}