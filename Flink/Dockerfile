FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Flink/Flink.csproj", "Flink/"]
RUN dotnet restore "Flink/Flink.csproj"
COPY . .
WORKDIR "/src/Flink"
RUN dotnet build "Flink.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Flink.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

USER flink
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Flink.dll"]
