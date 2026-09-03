using Microsoft.EntityFrameworkCore;
using StudentExam.Application;
using StudentExam.Infrastructure;
using StudentExam.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Swagger is left on in every environment so the hosted demo stays browsable.
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS enforcement is left to the host (Azure App Service "HTTPS Only" setting):
// UseHttpsRedirection() here would loop behind App Service's Linux reverse proxy,
// which doesn't set forwarded headers by default.

app.UseAuthorization();

app.MapControllers();

app.Run();
