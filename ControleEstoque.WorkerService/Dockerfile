# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os csproj para restaurar pacotes (cache otimizado)
COPY ControleEstoque.WorkerService/*.csproj ControleEstoque.WorkerService/
COPY ControleEstoque.Application/*.csproj ControleEstoque.Application/
COPY ControleEstoque.Core/*.csproj ControleEstoque.Core/
COPY ControleEstoque.Infrastructure/*.csproj ControleEstoque.Infrastructure/

RUN dotnet restore ControleEstoque.WorkerService/ControleEstoque.WorkerService.csproj

# Copia todo o código fonte necessário
COPY ControleEstoque.WorkerService/. ControleEstoque.WorkerService/
COPY ControleEstoque.Application/. ControleEstoque.Application/
COPY ControleEstoque.Core/. ControleEstoque.Core/
COPY ControleEstoque.Infrastructure/. ControleEstoque.Infrastructure/

# Publica o WorkerService
#RUN dotnet publish ControleEstoque.WorkerService/ControleEstoque.WorkerService.csproj -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish ControleEstoque.WorkerService -c Release -r linux-x64 --self-contained false -o /app/publish /p:UseAppHost=false

## Stage 2: Runtime
#FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
#WORKDIR /app
#COPY --from=build /app/publish .
#ENTRYPOINT ["dotnet", "ControleEstoque.WorkerService.dll"]

# Runtime image apenas para rodar
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ControleEstoque.WorkerService.dll"]