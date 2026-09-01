using HrSystem.Api.Extensions;
using HrSystem.Api.Middleware;
using HrSystem.Api.Security;
using HrSystem.Application.Abstractions.Security;
using HrSystem.Application.DependencyInjection;
using HrSystem.Infrastructure;
using HrSystem.Infrastructure.Persistence;
using HrSystem.Infrastructure.Security;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddApiProblemDetails();
builder.Services.AddApiRateLimiting();

builder.Services.AddCors(options => options.AddPolicy("Frontend", policy =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:3000"];
    policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
}));

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "HR System API",
        Version = "v1",
        Description = "Human Resources Management API built with Clean Architecture."
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", async (AppDbContext db, CancellationToken ct) =>
    Results.Ok(new { status = await db.Database.CanConnectAsync(ct) ? "Healthy" : "Unhealthy" }));

app.Run();
