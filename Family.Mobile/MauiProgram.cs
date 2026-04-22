using Microsoft.Extensions.Logging;
using Family.Mobile.Services;
using Family.Mobile.ViewModels;
using Family.Mobile.Views;

namespace Family.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		// Регистрируем наш сервис для работы с API
		builder.Services.AddSingleton<FamilyService>();
		// Регистрируем ViewModel и Страницы
		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<MainViewModel>();
		builder.Services.AddTransient<AddMemberPage>();
        builder.Services.AddTransient<ModalPage>();
        builder.Services.AddTransient<AddMemberViewModel>();

        // Регистрация самого приложения
        builder.Services.AddSingleton<App>();

        return builder.Build();
	}
}
