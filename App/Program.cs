using Domain.Models.GeneralSettings;
using Microsoft.Extensions.Options;
using CrossCutting.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var AppConfigurations = new AppSettingsModel();
new ConfigureFromConfigurationOptions<AppSettingsModel>(
              builder.Configuration.GetSection("Configurations"))
                  .Configure(AppConfigurations);

AllConfigurations.ConfigureDependencies(builder.Services, AppConfigurations);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc($"v{AppConfigurations.AppVersion}", new OpenApiInfo { Title = AppConfigurations.AppName, Version = AppConfigurations.AppVersion.ToString() });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint($"v{AppConfigurations.AppVersion}/swagger.json", AppConfigurations.AppName));
}

app.UseCors(opt => opt.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
