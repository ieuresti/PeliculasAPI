using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI;
using PeliculasAPI.Servicios;
using PeliculasAPI.Utilidades;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configuracion de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuracion AutoMapper
builder.Services.AddSingleton(proveedor => new MapperConfiguration(configuracion =>
{
    var geometryFactory = proveedor.GetRequiredService<GeometryFactory>();
    configuracion.AddProfile(new AutoMapperProfiles(geometryFactory));
}).CreateMapper());

// Configuracion de Identity
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>() // Configuracion para utilizar Entity Framework con Identity
    .AddDefaultTokenProviders(); // Configuracion para utilizar los token providers por defecto de Identity (para generar tokens de confirmacion de email, restablecimiento de contraseña, etc.)

builder.Services.AddScoped<UserManager<IdentityUser>>(); // Nos va a permitir gestionar los usuarios (crear, eliminar, actualizar, etc.)
builder.Services.AddScoped<SignInManager<IdentityUser>>(); // Nos va a permitir gestionar el inicio de sesión de los usuarios (iniciar sesión, cerrar sesión, etc.)

// Configuracion del servicio de autenticacion JWT
builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false; // Esto es para evitar que no se cambien los nombres de los claims que ASP.NET Core cambia por defecto
    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // No validamos el emisor del token
        ValidateAudience = false, // No validamos el destinatario del token
        ValidateLifetime = true, // Validamos la expiracion del token
        ValidateIssuerSigningKey = true, // Validamos la firma del token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["llavejwt"]!)), // Configuracion de la clave de firma del token y el algoritmo de firma
        ClockSkew = TimeSpan.Zero // Esto es para evitar que se permita un margen de tiempo para la expiracion del token (por defecto es de 5 minutos)
    };
});

// Configuracion politicas de autorizacion
builder.Services.AddAuthorization(opciones =>
{
    // Esta política exige que el usuario autenticado tenga un claim cuyo tipo sea esAdmin. Si el claim no existe, la autorización falla (403)
    opciones.AddPolicy("esAdmin", politica => politica.RequireClaim("esAdmin"));
});

// Configuracion DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opciones => 
    opciones.UseSqlServer("name=DefaultConnection", sqlServer => 
    sqlServer.UseNetTopologySuite())); // Configuracion para poder utilizar NetTopologySuite con SqlServer

// Configuracion de GeometryFactory para poder realizar operaciones con distancias
builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326)); // Para trabajar con coordenadas en la Tierra

// Configuracion de OutputCache
builder.Services.AddOutputCache(opciones =>
{
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);
});

var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!.Split(",");

// Habilitar CORS
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(opcionesCORS =>
    {
        opcionesCORS.WithOrigins(origenesPermitidos).AllowAnyMethod().AllowAnyHeader()
            .WithExposedHeaders("cantidad-total-registros");
    });
});

// Configuracion de archivos
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Habilita el middleware de Swagger
    app.UseSwaggerUI(); // Habilita la interfaz de usuario de Swagger
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

app.Run();
