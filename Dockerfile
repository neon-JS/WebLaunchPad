# Note that this doesn't currently work as the docker container can't write data to the file handle of the host system.
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0-bullseye-slim-amd64 AS build

ARG TARGETARCH
ARG TARGETOS

RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-$arch > /tmp/rid

WORKDIR /src

COPY ["WebLaunchPad.Api/WebLaunchPad.Api.csproj", "./WebLaunchPad.Api/"]
COPY ["WebLaunchPad.Communication/WebLaunchPad.Communication.csproj", "./WebLaunchPad.Communication/"]
COPY ["WebLaunchPad.Images/WebLaunchPad.Images.csproj", "./WebLaunchPad.Images/"]

RUN dotnet restore -r $(cat /tmp/rid) "WebLaunchPad.Api/WebLaunchPad.Api.csproj"

COPY WebLaunchPad.Api WebLaunchPad.Api
COPY WebLaunchPad.Communication WebLaunchPad.Communication
COPY WebLaunchPad.Images WebLaunchPad.Images

WORKDIR "/src/"
RUN dotnet build "WebLaunchPad.Api/WebLaunchPad.Api.csproj" -c Release -o /app/build -r $(cat /tmp/rid) --no-self-contained

FROM build AS publish
RUN dotnet publish "WebLaunchPad.Api/WebLaunchPad.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false -r $(cat /tmp/rid) --no-self-contained --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebLaunchPad.Api.dll"]
