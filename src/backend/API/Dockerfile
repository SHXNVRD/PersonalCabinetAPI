ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
WORKDIR /app
EXPOSE 8080-8082

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["src/Templates/Templates.csproj", "Templates/"]
COPY ["src/Tcp/Tcp.csproj", "Tcp/"]
RUN dotnet restore "API/API.csproj"
COPY ./src .
WORKDIR API
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV API_USER=apiuser
RUN adduser --system --uid 1001 --group --shell /bin/false $API_USER
RUN mkdir logs && \
    chown $API_USER:$API_USER /app/logs
USER $API_USER
ENTRYPOINT ["dotnet", "API.dll"]
