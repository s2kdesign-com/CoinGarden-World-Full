#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["back-end/CoinGardenWorld.Grpc/CoinGardenWorld.Grpc.csproj", "back-end/CoinGardenWorld.Grpc/"]
RUN dotnet restore "back-end/CoinGardenWorld.Grpc/CoinGardenWorld.Grpc.csproj"
COPY . .
WORKDIR "/src/back-end/CoinGardenWorld.Grpc"
RUN dotnet build "CoinGardenWorld.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoinGardenWorld.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoinGardenWorld.Grpc.dll"]