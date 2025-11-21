# -----------------------------------------------------
# Etapa 1: Build (MCR .NET SDK)
# -----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura a solução e os projetos para garantir cache eficiente.
# Assume que a Solution (.sln) está na raiz do repositório.
COPY *.sln .

# Copia os arquivos de projeto (incluindo o arquivo principal IsCool.csproj).
# Eles são copiados para /src/IsCool/ e /src/IsCool.Tests/
COPY ["IsCool/IsCool.csproj", "IsCool/"]
COPY ["IsCool.Tests/IsCool.Tests.csproj", "IsCool.Tests/"]

# Restaura as dependências
RUN dotnet restore "IsCool/IsCool.csproj"

# CRUCIAL: Copia TODO o conteúdo do diretório IsCool/ (os arquivos de código-fonte como Program.cs, Controllers/) 
# para o diretório IsCool/ DENTRO do WORKDIR (/src/IsCool/).
# Isso resolve o erro CS5001 e garante que o compilador encontre o Program.cs.
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
# Copia APENAS os binários publicados (que já contêm o appsettings.json e a IsCool.dll)
COPY --from=publish /app/publish .

# Define a porta do Container App para 8080 (padrão)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Inicia a aplicação
ENTRYPOINT ["dotnet", "IsCool.dll"]