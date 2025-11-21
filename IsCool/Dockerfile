# -----------------------------------------------------
# Etapa 1: Build (MCR .NET SDK)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura a solução e os projetos para garantir cache eficiente.
COPY *.sln .

# Copia os arquivos de projeto (.csproj)
COPY ["IsCool/IsCool.csproj", "IsCool/"]
COPY ["IsCool.Tests/IsCool.Tests.csproj", "IsCool.Tests/"]

# Restaura as dependências
RUN dotnet restore "IsCool/IsCool.csproj"

# Copia todo o código fonte (incluindo o appsettings.json gerado no CI) para a pasta de build
COPY IsCool/. IsCool/

WORKDIR "/src/IsCool"

# Faz o build da aplicação
RUN dotnet build "IsCool.csproj" -c Release -o /app/build

# -----------------------------------------------------
# Etapa 2: Publicação (Criação de Binários)
# -----------------------------------------------------
FROM build AS publish
WORKDIR "/src/IsCool"
RUN dotnet publish "IsCool.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------------------
# Etapa 3: Final (MCR ASPNET Runtime)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 1. Copia os binários publicados (DLLs)
COPY --from=publish /app/publish .

# 2. CORREÇÃO DEFINITIVA ("Lock-in" do Arquivo):
# Copia explicitamente o appsettings.json da etapa de build para a pasta final.
# Ignoramos se o dotnet publish incluiu ou não; nós garantimos que ele esteja aqui.
COPY --from=build /src/IsCool/appsettings.json /app/appsettings.json

# Define a porta do Container App para 8080 (padrão)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Inicia a aplicação
ENTRYPOINT ["dotnet", "IsCool.dll"]