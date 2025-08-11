FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ModuleBankApp.API/ModuleBankApp.API.csproj", "ModuleBankApp.API/"]
RUN dotnet restore "ModuleBankApp.API/ModuleBankApp.API.csproj"
COPY . .
WORKDIR "/src/ModuleBankApp.API"
RUN dotnet build "ModuleBankApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ModuleBankApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ./ModuleBankApp.API/Data/Procedures/accrue_interest.sql /app/Data/Procedures/accrue_interest.sql
ENTRYPOINT ["dotnet", "ModuleBankApp.API.dll"]
