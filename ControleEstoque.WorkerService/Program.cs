using ControleEstoque.Application.Services;
using ControleEstoque.Core.Interfaces.Repository;
using ControleEstoque.Core.Interfaces.Service;
using ControleEstoque.Infrastructure.Persistence;
using ControleEstoque.Infrastructure.Repositories;
using ControleEstoque.WorkerService;
using Microsoft.EntityFrameworkCore;
using System;

var builder = Host.CreateApplicationBuilder(args);

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<WorkerControleEstoque>();
        // Adiciona serviços da camada Core
        services.AddScoped<IPedidoRepository, PedidoRepository>();
    })
    .Build();
host.Run();