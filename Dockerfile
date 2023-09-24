FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebLaunchPad.Api/WebLaunchPad.Api.csproj", "./WebLaunchPad.Api/"]
COPY ["WebLaunchPad.Communication/WebLaunchPad.Communication.csproj", "./WebLaunchPad.Communication/"]
COPY ["WebLaunchPad.Images/WebLaunchPad.Images.csproj", "./WebLaunchPad.Images/"]
RUN dotnet restore "WebLaunchPad.Api/WebLaunchPad.Api.csproj"

COPY WebLaunchPad.Api WebLaunchPad.Api
COPY WebLaunchPad.Communication WebLaunchPad.Communication
COPY WebLaunchPad.Images WebLaunchPad.Images

WORKDIR "/src/"
RUN dotnet build "WebLaunchPad.Api/WebLaunchPad.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebLaunchPad.Api/WebLaunchPad.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebLaunchPad.Api.dll"]
