FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine3.10 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine3.10 AS build
WORKDIR /src
COPY Helper/Helper.csproj Helper/
COPY OdhApiCore/OdhApiCore.csproj OdhApiCore/
RUN dotnet restore OdhApiCore/OdhApiCore.csproj
COPY . .
WORKDIR /src/OdhApiCore
RUN dotnet build OdhApiCore.csproj -c Release -o /app

FROM build AS test
WORKDIR /src
RUN dotnet test --filter Category!=Integration OdhApiCore.sln

FROM build AS publish
RUN dotnet publish OdhApiCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OdhApiCore.dll"]
