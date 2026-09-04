using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using StudentExam.Application;
using StudentExam.Infrastructure;
using StudentExam.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomOperationIds(apiDesc =>
        (apiDesc.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.Name);
});

const string LocalDevCorsPolicy = "LocalDevCors";
builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalDevCorsPolicy, policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(LocalDevCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
