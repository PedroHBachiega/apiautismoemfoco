using Google.Cloud.Firestore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    options.ListenAnyIP(int.Parse(port));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var projectId = builder.Configuration["Firebase:ProjectId"] ?? "";
var credPath = builder.Configuration["Firebase:CredentialsPath"] ?? "";
var credJson = builder.Configuration["Firebase:CredentialsJson"] ?? "";

GoogleCredential? appCredential = null;
if (!string.IsNullOrWhiteSpace(credPath) && File.Exists(credPath))
{
    appCredential = GoogleCredential.FromFile(credPath);
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
}
else if (!string.IsNullOrWhiteSpace(credJson))
{
    appCredential = GoogleCredential.FromJson(credJson);
    var tmpFile = Path.Combine(Path.GetTempPath(), "firebase-admin-credentials.json");
    try
    {
        File.WriteAllText(tmpFile, credJson, Encoding.UTF8);
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", tmpFile);
    }
    catch {}
}
else
{
    try
    {
        appCredential = GoogleCredential.GetApplicationDefault();
    }
    catch {}
}

try
{
    if (FirebaseApp.DefaultInstance == null && appCredential != null)
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = appCredential,
            ProjectId = string.IsNullOrWhiteSpace(projectId) ? null : projectId
        });
    }
}
catch {}

var hasCredEnv = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS"));
if (!string.IsNullOrWhiteSpace(projectId) && (appCredential != null || hasCredEnv))
{
    builder.Services.AddSingleton(provider => FirestoreDb.Create(projectId));
}

builder.Services.AddScoped<AutismoEmFoco.API.Repositories.UsuarioRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.WithOrigins(   
            "http://localhost:5173", //para dev local
            "https://autismoemfoco-9117e.web.app",
            "https://autismoemfoco-9117e.firebaseapp.com"
        ).AllowAnyHeader().AllowAnyMethod();
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

app.UseCors("Default");

app.UseAuthorization();

app.MapControllers();

app.Run();
