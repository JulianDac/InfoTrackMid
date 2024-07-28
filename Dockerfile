FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SettlementServiceWebAPI/SettlementServiceWebAPI.csproj", "SettlementServiceWebAPI/"]
RUN dotnet restore "SettlementServiceWebAPI/SettlementServiceWebAPI.csproj"
COPY . .
WORKDIR "/src/SettlementServiceWebAPI"
RUN dotnet build "SettlementServiceWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SettlementServiceWebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "SettlementServiceWebAPI.dll"]
