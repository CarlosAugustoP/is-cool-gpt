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

# **CORREÇÃO CRUCIAL PARA appsettings.json:**
# 1. Copia o appsettings.json (que contém os secrets injetados pelo GH Action) de forma explícita.
# Isso resolve o erro 'FALHA - NÃO ENCONTRADO'.
COPY IsCool/appsettings.json IsCool/appsettings.json

# 2. Copia o restante do conteúdo de IsCool/ (código-fonte, incluindo Program.cs e Controllers/)
COPY IsCool/. IsCool/

WORKDIR "/src/IsCool"

# Faz o build da aplicação.
RUN dotnet build "IsCool.csproj" -c Release -o /app/build

# -----------------------------------------------------
# Etapa 2: Publicação (Criação de Binários)
# -----------------------------------------------------
FROM build AS publish
WORKDIR "/src/IsCool"
# O dotnet publish irá incluir o appsettings.json que foi copiado acima.
RUN dotnet publish "IsCool.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------------------
# Etapa 3: Final (MCR ASPNET Runtime)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia APENAS os binários publicados (agora com appsettings.json garantido)
COPY --from=publish /app/publish .

# Define a porta do Container App para 8080 (padrão)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Inicia a aplicação
ENTRYPOINT ["dotnet", "IsCool.dll"]