using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RizenSoftApiV2.Domains;
using RizenSoftApiV2.Interfaces;
using RizenSoftApiV2.Models;
using RizenSoftApiV2.Services;

string RizenSoftAllowSpecificOrigins = "_rizenSoftAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

//connection string
string connectionString;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.Development.json", true)
                                .AddEnvironmentVariables();
    connectionString = builder.Configuration["SQLCONNSTRING"];
} 
else
    connectionString = builder.Configuration.GetConnectionString("SQLCONNSTRING");
builder.Services.AddDbContext<RizenSoftDBContext>(builder =>
{
    builder.UseNpgsql(connectionString);
});

// Add services to the container..S

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rizensoft API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: RizenSoftAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7290", "https://rizensoft-azure-api.azurewebsites.net", "https://rizensoft-azure-api-apim.azure-api.net");
                          policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                      });
});
builder.Services.Configure<AppSettings>(builder.Configuration.GetRequiredSection("AppSettings"));
builder.Services.AddScoped<AddressDomain>()
    .AddScoped<UserDomain>()
    .AddScoped<AuthenticationDomain>()
    .AddScoped<TokenDomain>()
    .AddScoped<ITokenService, TokenService>()
    .AddScoped<AppSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseStaticFiles();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rizensoft API V1");
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

