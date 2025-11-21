# -----------------------------------------------------
# Etapa 1: Build (MCR .NET SDK)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura a solução e os projetos para garantir cache eficiente.
# Assume que a Solution (.sln) está na raiz do repositório.
COPY *.sln .

# Copia os arquivos de projeto (incluindo o arquivo principal IsCool.csproj).
COPY ["IsCool/IsCool.csproj", "IsCool/"]
COPY ["IsCool.Tests/IsCool.Tests.csproj", "IsCool.Tests/"]

# Restaura as dependências
RUN dotnet restore "IsCool/IsCool.csproj"

# CRUCIAL: Copia TODO o conteúdo do diretório IsCool/ para o WORKDIR /src/IsCool/.
# Isso garante que todos os Controllers, Services e o appsettings.json
# criado pelo CI/CD estejam disponíveis na pasta de trabalho para o BUILD.
# O contexto de build é a raiz do repo, então ele pega IsCool/ e joga para /src/IsCool/
COPY IsCool/ .
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
# Copia APENAS os binários publicados (que já contêm o appsettings.json e a IsCool.dll)
COPY --from=publish /app/publish .

# Define a porta do Container App para 8080 (padrão)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Inicia a aplicação
ENTRYPOINT ["dotnet", "IsCool.dll"]