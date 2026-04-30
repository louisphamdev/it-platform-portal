# API Gateway - YARP Reverse Proxy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/services/api-gateway/ApiGateway.csproj", "src/services/api-gateway/"]
RUN dotnet restore "src/services/api-gateway/ApiGateway.csproj"
COPY src/ src/
WORKDIR "/src/src/services/api-gateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:7000
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
