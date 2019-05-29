FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
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
