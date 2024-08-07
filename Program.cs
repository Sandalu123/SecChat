using Microsoft.EntityFrameworkCore;
using SecureChat.Data;
using SecureChat.Hubs;
using SecureChat.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add SignalR services
builder.Services.AddSignalR();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5000", "http://localhost:5001", "https://localhost:5001")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Entity Framework services
builder.Services.AddDbContext<SecureChatContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add name generator service
builder.Services.AddScoped<INameGenerator, NameGenerator>();

// Add background service for session cleanup
builder.Services.AddHostedService<SessionCleanupService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure SignalR hub endpoints
app.MapHub<ChatHub>("/chatHub");

app.Run();