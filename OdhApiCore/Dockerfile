FROM mcr.microsoft.com/dotnet/core/aspnet:3.0.0-preview7-alpine3.9 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview7-alpine3.9 AS build
WORKDIR /src
COPY OdhApiCore/OdhApiCore.csproj OdhApiCore/
RUN dotnet restore OdhApiCore/OdhApiCore.csproj
COPY . .
WORKDIR /src/OdhApiCore
RUN dotnet build OdhApiCore.csproj -c Release -o /app

FROM build AS test
WORKDIR /src
RUN dotnet test OdhApiCore.sln

FROM build AS publish
RUN dotnet publish OdhApiCore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OdhApiCore.dll"]
