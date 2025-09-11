# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o .sln e os arquivos de projeto
COPY ["ControleEstoque.API/ControleEstoque.API.csproj", "ControleEstoque.API/"]
COPY ["ControleEstoque.Application/ControleEstoque.Application.csproj", "ControleEstoque.Application/"]
COPY ["ControleEstoque.Core/ControleEstoque.Core.csproj", "ControleEstoque.Core/"]
COPY ["ControleEstoque.Infrastructure/ControleEstoque.Infrastructure.csproj", "ControleEstoque.Infrastructure/"]

# Restaura dependências
RUN dotnet restore "ControleEstoque.API/ControleEstoque.API.csproj"

# Copia tudo e publica
COPY . .
WORKDIR "/src/ControleEstoque.API"
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ControleEstoque.API.dll"]