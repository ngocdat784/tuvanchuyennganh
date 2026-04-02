# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out ./

# Port cho Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Chạy app
ENTRYPOINT ["dotnet", "HuongnghiepAPI.dll"]
# Dockerfile for HuongnghiepAPI