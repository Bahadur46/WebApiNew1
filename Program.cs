using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebApiNew.Comman;
using WebApiNew.Repository;


var builder = WebApplication.CreateBuilder(args);

// =======================================================
// Controllers (NO auto validation)
// =======================================================
builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Disable automatic model validation (Required ignore hoga)
    options.SuppressModelStateInvalidFilter = true;
});

// =======================================================
// MongoDB Dependency Injection
// =======================================================
builder.Services.AddSingleton<UserRepository>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new UserRepository(configuration);
});

builder.Services.AddSingleton<AuthService>();

// =======================================================
// JWT Authentication Configuration
// =======================================================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        RoleClaimType = ClaimTypes.Role
    };
});

// =======================================================
// Authorization Policies
// =======================================================
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy =>
//        policy.RequireRole("Admin"));
//});

// =======================================================
// Swagger (Optional but recommended)
// =======================================================
builder.Services.AddEndpointsApiExplorer();


// =======================================================
// Build App
// =======================================================
var app = builder.Build();

// =======================================================
// Middleware Pipeline
// =======================================================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ApiPerformanceMiddleware>();
app.MapControllers();

// =======================================================
// Run Application
// =======================================================
app.Run();
