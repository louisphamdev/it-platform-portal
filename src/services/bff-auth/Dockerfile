# BFF Auth Service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/services/bff-auth/Bff.Auth.csproj", "src/services/bff-auth/"]
COPY ["src/modules/Auth/Modules.Auth.csproj", "src/modules/Auth/"]
RUN dotnet restore "src/services/bff-auth/Bff.Auth.csproj"
COPY src/ src/
WORKDIR "/src/src/services/bff-auth"
RUN dotnet build "Bff.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bff.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:7001
ENTRYPOINT ["dotnet", "Bff.Auth.dll"]
