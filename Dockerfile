# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY HrSystem.Backend.slnx ./
COPY src/HrSystem.Domain/HrSystem.Domain.csproj src/HrSystem.Domain/
COPY src/HrSystem.Application/HrSystem.Application.csproj src/HrSystem.Application/
COPY src/HrSystem.Infrastructure/HrSystem.Infrastructure.csproj src/HrSystem.Infrastructure/
COPY src/HrSystem.Api/HrSystem.Api.csproj src/HrSystem.Api/
RUN dotnet restore src/HrSystem.Api/HrSystem.Api.csproj
COPY src ./src
RUN dotnet publish src/HrSystem.Api/HrSystem.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HrSystem.Api.dll"]
