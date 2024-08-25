FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["src/Falc.Identity.STS.Identity/Falc.Identity.STS.Identity.csproj", "src/Falc.Identity.STS.Identity/"]
COPY ["src/Falc.Identity.Admin.EntityFramework.Shared/Falc.Identity.Admin.EntityFramework.Shared.csproj", "src/Falc.Identity.Admin.EntityFramework.Shared/"]
COPY ["src/Falc.Identity.Shared/Falc.Identity.Shared.csproj", "src/Falc.Identity.Shared/"]
RUN dotnet restore -a $TARGETARCH "src/Falc.Identity.STS.Identity/Falc.Identity.STS.Identity.csproj"
COPY . .
WORKDIR "/src/src/Falc.Identity.STS.Identity"
RUN dotnet build -a $TARGETARCH "Falc.Identity.STS.Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "Falc.Identity.STS.Identity.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Falc.Identity.STS.Identity.dll"]