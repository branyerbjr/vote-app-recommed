# Usar la imagen de .NET 8 SDK para construir la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar archivos de proyecto y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto de los archivos y publicar la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

# Usar la imagen de ASP.NET Core Runtime para la imagen final
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto
EXPOSE 80

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "VoteAppBackend.dll"]
