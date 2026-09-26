using gLAMS.Application.Interfaces.Logging;
using gLAMS.Infrastructure.Logging;
using gLAMS.Application.Interfaces.Factories;
using gLAMS.Infrastructure.Factories;
using gLAMS.Application.Interfaces.Repositories;
using gLAMS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");
}

builder.Services.AddSingleton<IAppLogger, WindowsEventLogger>();

builder.Services.AddSingleton<ISqlConnectionFactory>(
    provider => new SqlConnectionFactory(connectionString!)
);

builder.Services.AddScoped<IFolderRepository, FolderRepository>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
