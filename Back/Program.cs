using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:80");
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorCorsPolicy", policy =>
    {
        policy.WithOrigins("https://dmitriyza.github.io") // Разрешаем ваш фронтенд
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFamilyRepository, FamilyRepository>();
builder.Services.AddScoped<FamilyMemberService>();
builder.Services.AddScoped<IRelationshipRepository, RelationshipRepository>();
builder.Services.AddScoped<IRelationshipService, RelationshipService>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddScoped<IRelationTypeRepository, RelationTypeRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();


var app = builder.Build();

// --- БЛОК АВТОМАТИЧЕСКОГО ОБНОВЛЕНИЯ И СИДИРОВАНИЯ БАЗЫ ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // 1. Сначала принудительно обновляем схему SQLite до самой последней миграции
        await context.Database.MigrateAsync();
        Console.WriteLine("База данных SQLite успешно обновлена.");

        // 2. Только после этого запускаем наполнение справочников дефолтными данными
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Произошла критическая ошибка при обновлении или сидировании базы данных SQLite.");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseStaticFiles();
var dataPhotosPath = "/data/photos";
if (Directory.Exists(dataPhotosPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(dataPhotosPath),
        RequestPath = "/photos"
    });
}
app.UseCors("BlazorCorsPolicy");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();


