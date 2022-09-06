#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY . .
RUN dotnet publish "back-end/CoinGardenWorld.MobileApps.Grpc/CoinGardenWorld.MobileApps.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoinGardenWorld.MobileApps.Grpc.dll"]