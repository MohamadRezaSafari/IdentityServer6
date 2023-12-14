
FROM nginx:1 AS base
WORKDIR /usr/share/nginx/html
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SinglePageApplication/SinglePageApplication.csproj", "SinglePageApplication/"]
RUN dotnet restore "./SinglePageApplication/./SinglePageApplication.csproj"
COPY . .
WORKDIR "/src/SinglePageApplication"
RUN dotnet build "./SinglePageApplication.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SinglePageApplication.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish/wwwroot .