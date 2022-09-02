#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
COPY . .
RUN dotnet publish "CoinGardenWorld.Web.csproj" -c Release -o /app/publish -self-contained --runtime linux-x64

FROM nginx:alpine AS final
WORKDIR /var/www/web

COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf