// Program.cs
using KnowledgeHub.Api.Data;
using KnowledgeHub.Api.Services;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OpenAI;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// Add services
builder.Services.AddControllers();

// Enable Swagger for development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IChatService, ChatService>();

// Configure CORS to allow Vite frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite dev server URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<OpenAIClient>(provider =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIClient(apiKey);
});

builder.Services.AddScoped<IRagService, RagService>();

var app = builder.Build();

// Use Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Storage")),
    RequestPath = "/storage"
});

// Enable CORS
app.UseCors("AllowFrontend");

// Map controllers
app.MapControllers();

// Run the app
app.Run();
