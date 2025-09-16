using ControleEstoque.Application.Middlewares;
using ControleEstoque.Application.Services;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Core.Utils;
using ControleEstoque.Infrastructure.Persistence;
using ControleEstoque.Infrastructure.Repositories;
//using ControleEstoque.WorkerService;
using Elastic.Apm;
using Elastic.Apm.AspNetCore;
using Elastic.Apm.DiagnosticSource;
using Elastic.Apm.EntityFrameworkCore;
using Elastic.Apm.NetCoreAll;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(5000); // Escuta em todas as interfaces no container
//});

// Elastic APM
//builder.Services.AddElasticApm();
builder.Services.AddElasticApmForAspNetCore(
    new HttpDiagnosticsSubscriber(),
    new EfCoreDiagnosticsSubscriber()
    );

// Recupera a string de conexão do MySQL
var connectionString = builder.Configuration.GetConnectionString("MySql");

// Registra o DbContext com MySQL
builder.Services.AddDbContext<ControleEstoqueDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
    ServiceLifetime.Scoped);

//RabbitMQ
var configuration = builder.Configuration;

var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST")
                 ?? configuration["RabbitMQ:HostName"]
                 ?? "localhost";
Console.WriteLine("Variavel : " + rabbitHost);
builder.Services.AddSingleton(new ConnectionFactory
{
    HostName = rabbitHost
});

////Dependency Injection
////Repository
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IProdutoPedidoRepository, ProdutoPedidoRepository>();
////Service
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IProdutoPedidoService, ProdutoPedidoService>();

// Adiciona Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Mantém enums como string no JSON
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGenNewtonsoftSupport();

// Configuração JWT
var key = Encoding.ASCII.GetBytes("BancoDigital2025CuritibaPRBrasil");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuerSigningKey = true,
         IssuerSigningKey = new SymmetricSecurityKey(key),
         ValidateIssuer = false,
         ValidateAudience = false,
         ValidateLifetime = true,
         ClockSkew = TimeSpan.Zero
     };

     // Eventos para customizar resposta
     options.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context =>
         {
             // Token inválido ou assinatura incorreta
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
             context.Response.ContentType = "application/json";
             return context.Response.WriteAsync("{\"error\":\"Token inválido\"}");
         },
         OnChallenge = context =>
         {
             // Token ausente ou expirado
             context.HandleResponse(); // impede comportamento padrão (401)
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
             context.Response.ContentType = "application/json";
             return context.Response.WriteAsync("{\"error\":\"Token expirado ou não fornecido\"}");
         },
         OnForbidden = context =>
         {
             // Usuário autenticado, mas sem permissão (não é admin)
             context.Response.StatusCode = StatusCodes.Status403Forbidden;
             context.Response.ContentType = "application/json";
             return context.Response.WriteAsync("{\"error\":\"Acesso negado: somente administradores podem acessar este recurso.\"}");
         }
     };
 });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMIN", policy =>
        policy.RequireRole("ADMIN")); // Policy que exige role "Administrador"
    options.AddPolicy("VENDEDOR", policy =>
       policy.RequireRole("VENDEDOR")); // Policy que exige role "Vendedor"
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Controle Estoque API com JWT", Version = "v1" });
    c.SchemaGeneratorOptions.UseAllOfForInheritance = true;
    c.SchemaFilter<EnumSchemaFilter>();
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Insira o token JWT sem o prefixo 'Bearer '",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();



app.UseCors("AllowAll");

// Ativa Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API com JWT v1");
        c.RoutePrefix = string.Empty; // Para acessar Swagger na raiz, opcional
    });
}

// Middleware que libera Swagger e OpenAPI sem autenticação
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (path.StartsWith("/swagger") || path.StartsWith("/openapi"))
    {
        await next();
        return;
    }
    await next();
});

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Mapear Controllers normalmente
app.MapControllers();

app.Run();
//app.Run("http://0.0.0.0:80");