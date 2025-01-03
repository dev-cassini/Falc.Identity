FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["src/Falc.Identity.Admin.Api/Falc.Identity.Admin.Api.csproj", "src/Falc.Identity.Admin.Api/"]
COPY ["src/Falc.Identity.Admin.Infrastructure/Falc.Identity.Admin.Infrastructure.csproj", "src/Falc.Identity.Admin.Infrastructure/"]
COPY ["src/Falc.Identity.Contracts/Falc.Identity.Contracts.csproj", "src/Falc.Identity.Contracts/"]
COPY ["src/Falc.Identity.Shared/Falc.Identity.Shared.csproj", "src/Falc.Identity.Shared/"]
RUN dotnet restore -a $TARGETARCH "src/Falc.Identity.Admin.Api/Falc.Identity.Admin.Api.csproj"
COPY . .
WORKDIR "/src/src/Falc.Identity.Admin.Api"
RUN dotnet build -a $TARGETARCH "Falc.Identity.Admin.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "Falc.Identity.Admin.Api.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Falc.Identity.Admin.Api.dll"]