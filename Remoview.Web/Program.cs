using Remoview.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// API'ye baðlanmak için HttpClient'ý ayarla
builder.Services.AddHttpClient("RemoviewApi", client =>
{
    // ÖNEMLÝ: Lütfen '7141' portunu kendi API'nizin (Swagger sayfasýnýn) portuyla deðiþtirin
    client.BaseAddress = new Uri("https://localhost:7141");
});

builder.Services.AddScoped<Remoview.Web.Services.ApiClient>();

// Bu yeni servisi de projemize tanýtýyoruz
builder.Services.AddScoped<Remoview.Web.Services.LocalStorageService>(); // <-- YENÝ SATIR

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
