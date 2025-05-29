# Usa a imagem oficial do .NET SDK como ambiente de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia os arquivos do projeto e restaura as dependências
COPY . ./
RUN dotnet restore "EloDrinksAPI.csproj"

# Compila e publica a aplicação
RUN dotnet publish "EloDrinksAPI.csproj" -c Release -o out

# Usa a imagem oficial do ASP.NET para a aplicação final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expõe a porta em que a aplicação será executada
EXPOSE 8080

# Define o comando de entrada para o contêiner
ENTRYPOINT ["dotnet", "EloDrinksAPI.dll"]