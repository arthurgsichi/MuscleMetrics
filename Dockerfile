FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5241

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MuscleMetrics.csproj", "./"]
RUN dotnet restore "MuscleMetrics.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "MuscleMetrics.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MuscleMetrics.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:5241
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "MuscleMetrics.dll"]