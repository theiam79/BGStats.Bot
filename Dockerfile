#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/BGStats.Bot/BGStats.Bot.csproj", "src/BGStats.Bot/"]
RUN dotnet restore "src/BGStats.Bot/BGStats.Bot.csproj"
COPY . .
WORKDIR "/src/src/BGStats.Bot"
RUN dotnet build "BGStats.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BGStats.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BGStats.Bot.dll"]