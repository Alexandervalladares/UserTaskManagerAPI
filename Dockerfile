# Etapa de compilación con .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# Copiar proyecto y restaurar dependencias (mejora cache)
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y publicar
COPY . ./
RUN dotnet publish -c Release -o out --no-restore

# Etapa de ejecución con runtime .NET 10 (imagen más ligera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# Exponer puerto
EXPOSE 8080

# Configurar URL de escucha
ENV ASPNETCORE_URLS=http://+:8080

# Punto de entrada (ajusta el nombre del DLL si tu proyecto se llama diferente)
ENTRYPOINT ["dotnet", "UserTaskManagerAPI.dll"]