FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 51712
EXPOSE 44336

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Catalog.API/Catalog.API.csproj Catalog.API/
RUN dotnet restore Catalog.API/Catalog.API.csproj
COPY . .
WORKDIR /src/Catalog.API
RUN dotnet build Catalog.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Catalog.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Catalog.API.dll"]
