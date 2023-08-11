FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebLaunchPad.csproj", "./"]
RUN dotnet restore "WebLaunchPad.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "WebLaunchPad.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebLaunchPad.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebLaunchPad.dll"]
