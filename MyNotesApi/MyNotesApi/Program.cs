using Microsoft.EntityFrameworkCore;
using MyNotesApi.Data;
using MyNotesApi.Filters;
using MyNotesApi.Logging;
using MyNotesApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();  
builder.Logging.AddConsole();

var isEfCommand = args.Any(arg => 
    arg.Contains("ef", StringComparison.OrdinalIgnoreCase) ||
    Environment.GetEnvironmentVariable("EF_CORE_COMMAND") == "true");

if (!isEfCommand)
{
    // builder.Logging.AddPostgresLogger();
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();