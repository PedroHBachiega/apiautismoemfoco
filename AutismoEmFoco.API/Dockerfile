# Imagem base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Imagem para buildar o projeto
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia tudo
COPY . .

# Restaura dependências
RUN dotnet restore "AutismoEmFoco.API.csproj"

# Build
RUN dotnet build "AutismoEmFoco.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "AutismoEmFoco.API.csproj" -c Release -o /app/publish

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render usa a PORT dinâmica, então você precisa disso:
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

ENTRYPOINT ["dotnet", "AutismoEmFoco.API.dll"]
