ARG DOTNET_VERSION=8.0

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS base
ENV UID=1010 HOME=/home/appuser
RUN useradd -u $UID -U -m appuser
USER ${UID}
WORKDIR $HOME
EXPOSE 8081
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
ARG BUILD_CONFIGURATION=Release
ENV HOME=/home/appuser
USER ${UID}
WORKDIR $HOME/src
COPY ["src/API/API.csproj", "API/"]
COPY ["src/Application/Application.csproj", "Application/"]
COPY ["src/Domain/Domain.csproj", "Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"
COPY ./src .
WORKDIR API
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o $HOME/app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o $HOME/app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR app
COPY --from=publish $HOME/app/publish .
USER root
RUN chown appuser -R $HOME
USER $UID
ENTRYPOINT ["dotnet", "API.dll"]
