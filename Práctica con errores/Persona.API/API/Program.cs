using Abstracciones.DA;
using Abstracciones.Interfaces.DA;
using Abstracciones.Interfaces.Flujo;
using Abstracciones.Interfaces.Reglas;
using Abstracciones.Interfaces.Servicios;
using Abstracciones.Modelos;
using Autorizacion.Middleware;
using DA.Dapper.Repositorios;
using Flujo;
using Flujo.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Reglas;
using Servicios;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var tokenConfiguration = builder.Configuration.GetSection("Token").Get<TokenConfiguracion>();
var jwtIssuer = tokenConfiguration.Issuer;
var jwtAudience = tokenConfiguration.Audience;
var jwtKey = tokenConfiguration.key;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options => {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    }
    );

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();
builder.Services.AddLogging();

builder.Services.AddScoped<IRepositorioDapper, RepositorioDapper>();
builder.Services.AddScoped<IPersonaDA, DA.Dapper.PersonaDA>();
builder.Services.AddScoped<IDocumentoDA, DA.Dapper.DocumentoDA>();
builder.Services.AddScoped<IPerfilDA, DA.Dapper.PerfilDA>();
builder.Services.AddScoped<IPokemonServicio, PokemonServicio>();
builder.Services.AddScoped<IDocumentoServicio, DocumentoServicio>();
builder.Services.AddScoped<ICorreoServicio, CorreoServicio>();
builder.Services.AddScoped<IFormatoHelper, Formato>();
builder.Services.AddScoped<IPersonaReglas, PersonaReglas>();
builder.Services.AddScoped<IPersonaFlujo, PersonaFlujo>();
builder.Services.AddScoped<IPokemonFlujo, PokemonFlujo>();
builder.Services.AddScoped<IPerfilFlujo, PerfilFlujo>();
builder.Services.AddScoped<IDocumentoFlujo, DocumentoFlujo>();
builder.Services.AddScoped<ICorreoFlujo, CorreoFlujo>();
builder.Services.AddScoped<IConfiguracion, Configuracion>();


builder.Services.AddSingleton<Autorizacion.Abstracciones.Flujo.IAutorizacionFlujo, Autorizacion.Flujo.AutorizacionFlujo>();
builder.Services.AddSingleton<Autorizacion.Abstracciones.DA.ISeguridadDA, Autorizacion.DA.SeguridadDA>();
builder.Services.AddSingleton<Autorizacion.Abstracciones.DA.IRepositorioDapper, Autorizacion.DA.Repositorios.RepositorioDapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AutorizacionClaims();
app.UseAuthorization();

app.MapControllers();

app.Run();
