FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG VERSION="1.0.0"
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Flink/Flink.csproj", "Flink/"]
COPY ["Flink.Domain/Flink.Domain.csproj", "Flink.Domain/"]
RUN dotnet restore "Flink/Flink.csproj"
COPY . .
WORKDIR "/src/Flink"
RUN dotnet restore --locked-mode && dotnet build  "Flink.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/build /p:Version=$VERSION

FROM build AS publish
ARG VERSION="1.0.0"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Flink.csproj" --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:Version=$VERSION

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -R app:app /app
USER $APP_UID
ENTRYPOINT ["dotnet", "Flink.dll"]
