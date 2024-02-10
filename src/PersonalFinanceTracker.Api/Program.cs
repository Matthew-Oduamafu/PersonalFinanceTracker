using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using PersonalFinanceTracker.Api;
using PersonalFinanceTracker.Api.Extensions;
using PersonalFinanceTracker.Api.Extensions.EndpointsExtensions;
using PersonalFinanceTracker.Api.Services.Interfaces;
using PersonalFinanceTracker.Api.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllerConfiguration();

builder.Services.AddCors(options => options
    .AddPolicy(CommonConstants.CorsPolicyName, policy => policy
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddJwtConfiguration(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocumentation();

builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddFluentValidationRulesToSwagger();

var app = builder.Build();

{
    var logger = app.Logger;
    await app.MigrateDatabaseAsync();
// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseCustomExceptionHandler(logger);

    app.UseSwaggerDocumentation();

    app.UseCors(CommonConstants.CorsPolicyName);
    app.UseRouting();
    app.UseHttpsRedirection();
    
    using var serviceScope = app.Services
        .GetRequiredService<IServiceScopeFactory>()
        .CreateScope();

    var linkService = serviceScope.ServiceProvider.GetRequiredService<ILinkService>();
    
    app.MapAuthEndpoints();
    app.MapEmployeeEndpoints(linkService);

    await app.RunAsync();
}