using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HrSystem.Backend.Data;
using HrSystem.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
    connectionString = "Data Source=hrsystem.db";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Use SQLite by default. Switch to UseSqlServer for production.
    if (connectionString.Contains("Server=") || connectionString.Contains("server="))
        options.UseSqlServer(connectionString);
    else
        options.UseSqlite(connectionString);
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:3000") // المسار بتاع الـ Frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // <<---- السطر ده إجباري عشان الكوكي تتقبل!
    });
});
// ── JWT Authentication ────────────────────────────────────
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"] ?? "Fallback_SecretKey_That_Is_32CharsLong!!";
var jwtIssuer = jwtSection["Issuer"] ?? "HrSystem.Backend";
var jwtAudience = jwtSection["Audience"] ?? "HrSystem.Frontend";
var jwtExpiration = int.Parse(jwtSection["ExpirationInMinutes"] ?? "1440");

builder.Services.AddSingleton(new JwtSettings(jwtKey, jwtIssuer, jwtAudience, jwtExpiration));
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };

    // Return 401 properly on auth failure
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsJsonAsync(new { message = "Token expired" });
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
        }
    };
});

builder.Services.AddAuthorization();

// ── Domain services ───────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<PayrollSettings>(builder.Configuration.GetSection("PayrollSettings"));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PayrollSettings>>().Value);
builder.Services.AddScoped<IPayrollEngine, PayrollEngine>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ILeaveBalanceService, LeaveBalanceService>();

// ── CORS ──────────────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// ── MVC / Controllers ─────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // camelCase matches the frontend (wwwroot/index.html) which reads e.fullName, data.token, etc.
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── Static Files (serve the frontend) ─────────────────────
builder.Services.AddDirectoryBrowser();

// ── Build App ─────────────────────────────────────────────
var app = builder.Build();

// ── Middleware Pipeline ────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Global exception handler: returns a clean JSON error body
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);

        if (context.Response.HasStarted)
        {
            throw;
        }

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { message = "An unexpected error occurred. Please try again." });
    }
});

app.UseHttpsRedirection();
app.UseDefaultFiles(); // Serves index.html at root
app.UseStaticFiles(); // Serves wwwroot/
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Seed Database ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

// ── Fallback to index.html (SPA support) ──────────────────
app.MapFallbackToFile("index.html");

app.Run();
