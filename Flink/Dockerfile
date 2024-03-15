FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG VERSION="1.0.0"
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Flink/Flink.csproj", "Flink/"]
RUN dotnet restore "Flink/Flink.csproj"
COPY . .
WORKDIR "/src/Flink"
RUN dotnet build "Flink.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:Version=$VERSION

FROM build AS publish
ARG VERSION="1.0.0"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Flink.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

USER flink
ENTRYPOINT ["dotnet", "Flink.dll"]
