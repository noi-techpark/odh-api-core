FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

#FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
FROM mcr.microsoft.com/dotnet/sdk:5.0.102-alpine3.12-amd64 AS build
WORKDIR /src
COPY DataModel/DataModel.csproj DataModel/
COPY Helper/Helper.csproj Helper/
COPY EBMS/EBMS.csproj EBMS/
COPY MSS/MSS.csproj MSS/
COPY LCS/LCS.csproj LCS/
COPY SIAG/SIAG.csproj SIAG/
COPY CDB/CDB.csproj CDB/
COPY SuedtirolWein/SuedtirolWein.csproj SuedtirolWein/
COPY NINJA/NINJA.csproj NINJA/
COPY RAVEN/RAVEN.csproj RAVEN/
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
COPY OdhApiCore/xml/Weather.xml xml/
COPY OdhApiCore/xml/STACategories.xml xml/
COPY OdhApiCore/xml/BezirkWeather.xml xml/
COPY OdhApiCore/xml/RequestInterceptor.xml xml/

ENTRYPOINT ["dotnet", "OdhApiCore.dll"]
