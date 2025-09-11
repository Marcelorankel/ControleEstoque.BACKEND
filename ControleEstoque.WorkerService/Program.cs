using ControleEstoque.Application.Services;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Infrastructure.Persistence;
using ControleEstoque.Infrastructure.Repositories;
using ControleEstoque.WorkerService;
using Microsoft.EntityFrameworkCore;
using System;


var builder = Host.CreateApplicationBuilder(args);

// Recupera a string de conexão do MySQL
var connectionString = builder.Configuration.GetConnectionString("MySql");

// Registra o DbContext com MySQL
builder.Services.AddDbContext<ControleEstoqueDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
    ServiceLifetime.Scoped);

// Registra os repositórios
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoPedidoRepository, ProdutoPedidoRepository>();
// Adicione outros repositórios aqui

// Registra o Worker
builder.Services.AddHostedService<WorkerControleEstoque>();

// Cria e roda o host
var host = builder.Build();
host.Run();



//var builder = Host.CreateApplicationBuilder(args);

//IHost host = Host.CreateDefaultBuilder(args)
//    .ConfigureServices((hostContext, services) =>
//    {
//        // Recupera a string de conexão do MySQL
//        var connectionString = builder.Configuration.GetConnectionString("MySql");

//        // Registra o DbContext com MySQL
//        builder.Services.AddDbContext<ControleEstoqueDbContext>(options =>
//            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)),
//            ServiceLifetime.Scoped);

//        // Adiciona serviços da camada Core
//        services.AddScoped<IPedidoRepository, PedidoRepository>();

//        services.AddHostedService<WorkerControleEstoque>();
//    })
//    .Build();
//host.Run();