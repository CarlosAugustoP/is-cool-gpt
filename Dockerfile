# ----------------------------------------------------------------------------------
# 1. BUILD (Etapa de Compilação): Usamos a imagem do SDK para compilar e restaurar pacotes.
# ----------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src
COPY ["IsCool.csproj", "./"]
RUN dotnet restore "IsCool.csproj"

# Copia o restante do código
COPY . .

# Compila e PUBLICA a aplicação na pasta /app/publish
RUN dotnet publish "IsCool.csproj" -c Release -o /app/publish

# ----------------------------------------------------------------------------------
# 2. FINAL (Etapa de Execução): Usamos a imagem ASPNET menor e copiamos apenas os arquivos publicados.
# ----------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app

# Copia os arquivos publicados da etapa 'build' para o contêiner final
COPY --from=build /app/publish .

# 🎯 INCLUSÃO CORRETA: Informa à plataforma que esta é a porta do serviço.
EXPOSE 8080 

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "IsCool.dll"]
