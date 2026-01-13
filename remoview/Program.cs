using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using remoview.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// CORS (Cross-Origin Resource Sharing) servisini ekle
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") ||
                origin.StartsWith("https://localhost"))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Veritabaný baðlantýsýný ayarla
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("RemoviewDb")));
// JWT Kimlik Doðrulamayý Ayarla
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Gerçek projede 'true' olmalý
            ValidateAudience = false, // Gerçek projede 'true' olmalý
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("BizimCokGizliAnahtarimiz12345!*-"))
        };
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Swagger'ý JWT'yi anlayacak þekilde ayarla
builder.Services.AddSwaggerGen(options =>
{
    // 1. "Authorize" butonu için bir güvenlik tanýmý ekle
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Lütfen 'Bearer ' yazdýktan sonra boþluk býrakýp token'ýnýzý girin.\n\nÖrnek: 'Bearer eyJhbGciOi...' "
    });

    // 2. Swagger'ýn bu tanýmý tüm API çaðrýlarýnda kullanmasýný saðla
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Tanýmladýðýmýz policy'yi burada kullanýyoruz
app.UseCors("AllowLocalhost");

app.UseAuthentication();
app.UseAuthorization();

// Bu ikinci UseAuthorization gereksiz, silebilirsin
// app.UseAuthorization();

app.MapControllers();

// DB seed (genre’larý ekle)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedGenresAsync(db);
}

app.Run();
