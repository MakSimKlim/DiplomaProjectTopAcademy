# ============================
# BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# ============================
# RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "DiplomaProjectTopAcademy.dll"]