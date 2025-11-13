# Use a imagem oficial do .NET SDK para a etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências
COPY ["IsCool.csproj", "./"]
RUN dotnet restore "IsCool.csproj"

# Copia o restante do código-fonte da aplicação (incluindo o appsettings.json criado pelo CI)
COPY . .

# Executa o comando de publicação
RUN dotnet publish "IsCool.csproj" -c Release -o /app/publish

# =========================================================================
# 🚩 CORREÇÃO FINAL: Cópia Explícita do appsettings.json
#
# O arquivo está em /src (WORKDIR). Usamos o path relativo.
# Copia o arquivo de configuração da raiz do build (/src) para o destino da publicação (/app/publish).
# =========================================================================
COPY appsettings.json /app/publish/

# Etapa final: Usa a imagem de runtime (menor)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS final
WORKDIR /app

# Copia a saída da publicação da etapa 'build' para a imagem final
COPY --from=build /app/publish .

# Expõe a porta que é usada pelo Kestrel (8080, que configuramos no Azure)
EXPOSE 8080

# Define o ponto de entrada para o contêiner
ENTRYPOINT ["dotnet", "IsCool.dll"]
