using Application;
using Infrastructure;
using Microsoft.AspNetCore.Http.Features;
using Persistence;
using WebApi;
using WebApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.ConfigureOptions<SwaggerOptionsSetup>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 5;
});
builder.Services
    .AddApplication()
    .AddPersistence(builder.Configuration)
    .AddInfrastructure()
    .AddAuth(builder.Configuration)
    .AddExceptionHandling();

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures("en-US")
    .AddSupportedUICultures("en-US"));

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseExceptionHandler("/error");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
