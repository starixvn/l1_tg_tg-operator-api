FROM hub.lumigame.com/base/dotnet/core/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

USER app

FROM --platform=$BUILDPLATFORM hub.lumigame.com/base/dotnet/core/sdk:9.0-alpine AS builder
ARG configuration=Release
WORKDIR /src

# Copy project files first for better layer caching
COPY ["TGOperatorApi/TGOperatorApi.csproj", "TGOperatorApi/"]

# Restore packages
RUN dotnet restore "TGOperatorApi/TGOperatorApi.csproj"

# Copy the rest of the code
COPY . .
WORKDIR "/src/TGOperatorApi"

# Publish directly (includes build)
RUN dotnet build "TGOperatorApi.csproj" -c $configuration -o /app/build

FROM builder AS publish
ARG configuration=Release
RUN dotnet publish "TGOperatorApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "TGOperatorApi.dll"] 