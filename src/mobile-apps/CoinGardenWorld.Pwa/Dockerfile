# Release .NET
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS publish
WORKDIR /src
COPY . .
RUN dotnet publish "mobile-apps/CoinGardenWorld.Pwa/CoinGardenWorld.Pwa.csproj" -c Release -o /app/publish

# Release NGINX
FROM nginx:alpine AS final
RUN apk add bash icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY "mobile-apps/CoinGardenWorld.Pwa/nginx.conf /etc/nginx/nginx.conf
SHELL ["/bin/bash", "-c"]
CMD /usr/sbin/nginx -g "daemon off;"
EXPOSE 80



# TODO: Argument on top of file
#ARG NODE_IMAGE=node:12.0
## TODO: If needed for npm builds
#FROM ${NODE_IMAGE} as node-build
#WORKDIR /web/src
#COPY Web/WebSPA/Client/package.json .
#COPY Web/WebSPA/Client/package-lock.json .
#RUN npm install -g npm@6
#RUN npm install
#COPY Web/WebSPA/Client .
#RUN npm run build:prod