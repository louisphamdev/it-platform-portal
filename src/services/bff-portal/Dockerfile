# BFF Portal Service
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/services/bff-portal/Bff.Portal.csproj", "src/services/bff-portal/"]
COPY ["src/modules/User/Modules.User.csproj", "src/modules/User/"]
COPY ["src/modules/Tenant/Modules.Tenant.csproj", "src/modules/Tenant/"]
COPY ["src/modules/Audit/Modules.Audit.csproj", "src/modules/Audit/"]
COPY ["src/modules/Permission/Modules.Permission.csproj", "src/modules/Permission/"]
RUN dotnet restore "src/services/bff-portal/Bff.Portal.csproj"
COPY src/ src/
WORKDIR "/src/src/services/bff-portal"
RUN dotnet build "Bff.Portal.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bff.Portal.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:7002
ENTRYPOINT ["dotnet", "Bff.Portal.dll"]
