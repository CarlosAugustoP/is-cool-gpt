# -----------------------------------------------------
# Etapa 1: Build (MCR .NET SDK)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura a solução e os projetos para garantir cache eficiente.
# Assume que a Solution (.sln) está na raiz do repositório.
COPY *.sln .

# Copia os arquivos de projeto.
# IsCool/IsCool.csproj e IsCool.Tests/IsCool.Tests.csproj são irmãos no repositório.
COPY ["IsCool/IsCool.csproj", "IsCool/"]
COPY ["IsCool.Tests/IsCool.Tests.csproj", "IsCool.Tests/"]

# Restaura as dependências (usa o arquivo .sln implicitamente ou o IsCool.csproj)
# Rodamos restore no projeto principal
RUN dotnet restore "IsCool/IsCool.csproj"

# Copia o restante do código da aplicação, incluindo o appsettings.json 
# criado pelo GitHub Action em ./IsCool/appsettings.json
COPY . .
WORKDIR "/src/IsCool"

# Faz o build da aplicação
RUN dotnet build "IsCool.csproj" -c Release -o /app/build

# -----------------------------------------------------
# Etapa 2: Publicação (Criação de Binários)
# -----------------------------------------------------
FROM build AS publish
WORKDIR "/src/IsCool"
# O dotnet publish já copia o appsettings.json (que está em /src/IsCool/) 
# para o diretório de publicação (/app/publish).
RUN dotnet publish "IsCool.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------------------
# Etapa 3: Final (MCR ASPNET Runtime)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia APENAS os binários publicados (que já contêm o appsettings.json)
COPY --from=publish /app/publish .

# Define a porta do Container App para 8080 (padrão)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Inicia a aplicação
ENTRYPOINT ["dotnet", "IsCool.dll"]
