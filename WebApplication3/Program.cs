using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Agrega esto si estás usando tu clase personalizada
using WebApplication.Models; // <-- cambia esto al namespace donde está tu clase Usuario
using WebApplication.Data;   // <-- cambia esto al namespace donde está tu AppDbContext

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN DE LA CADENA DE CONEXIÓN A SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURACIÓN DE IDENTITY CON LA CLASE USUARIO PERSONALIZADA
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// OPCIONAL: CONFIGURACIÓN JWT (si lo vas a usar para login)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],      // configurado en appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"],  // configurado en appsettings.json
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// AGREGAR CONTROLADORES
builder.Services.AddControllers();

// CONFIGURAR SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Si estás usando JWT y quieres probarlo desde Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Las Pirámides", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CONSTRUIR LA APP
var app = builder.Build();

// USO DE SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MIDDLEWARES
app.UseHttpsRedirection();

app.UseAuthentication(); // Importante para JWT
app.UseAuthorization();

app.MapControllers();

app.Run();
