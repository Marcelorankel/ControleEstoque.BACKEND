# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas os csproj para restaurar pacotes (cache otimizado)
COPY ControleEstoque.API/*.csproj ControleEstoque.API/
COPY ControleEstoque.Application/*.csproj ControleEstoque.Application/
COPY ControleEstoque.Core/*.csproj ControleEstoque.Core/
COPY ControleEstoque.Infrastructure/*.csproj ControleEstoque.Infrastructure/

RUN dotnet restore ControleEstoque.API/ControleEstoque.API.csproj

# Copia todo o código fonte necessário
COPY ControleEstoque.API/. ControleEstoque.API/
COPY ControleEstoque.Application/. ControleEstoque.Application/
COPY ControleEstoque.Core/. ControleEstoque.Core/
COPY ControleEstoque.Infrastructure/. ControleEstoque.Infrastructure/

# Publica a API
RUN dotnet publish ControleEstoque.API/ControleEstoque.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Permitir que o Kestrel escute em todas interfaces
ENV DOTNET_URLS=http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "ControleEstoque.API.dll"]