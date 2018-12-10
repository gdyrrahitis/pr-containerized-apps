FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 51705
EXPOSE 44372

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Orders.API/Orders.API.csproj Orders.API/
RUN dotnet restore Orders.API/Orders.API.csproj
COPY . .
WORKDIR /src/Orders.API
RUN dotnet build Orders.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Orders.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Orders.API.dll"]
