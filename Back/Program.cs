using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    await DbInitializer.SeedAsync(context);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseStaticFiles();
app.UseCors("BlazorCorsPolicy");
app.UseHttpsRedirection();
app.MapControllers();
app.Run();


