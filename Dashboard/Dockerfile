#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0.0-rc.2 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM node:12 as node-build
WORKDIR /src
COPY ["Dashboard/dashboard-app/package.json", "Dashboard/dashboard-app/package-lock.json", "Dashboard/dashboard-app/"]
WORKDIR /src/Dashboard/dashboard-app
RUN npm ci

WORKDIR /src
COPY ["Dashboard/dashboard-app", "Dashboard/dashboard-app"]
WORKDIR /src/Dashboard/dashboard-app
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:5.0.100-rc.2 AS build
WORKDIR /src
COPY ["Directory.Build.props", "global.json", "GlobalSuppressions.cs", "./"]
COPY ["Dashboard/Dashboard.csproj", "Dashboard/"]
RUN dotnet restore "Dashboard/Dashboard.csproj"
COPY . .
WORKDIR "/src/Dashboard"
RUN dotnet build "Dashboard.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dashboard.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node-build /src/Dashboard/dashboard-app/build ./dashboard-app/build
ENTRYPOINT ["dotnet", "Dashboard.dll"]