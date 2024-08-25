FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /src

COPY ["src/Falc.Identity.Admin/Falc.Identity.Admin.csproj", "src/Falc.Identity.Admin/"]
COPY ["src/Falc.Identity.Admin.EntityFramework.Shared/Falc.Identity.Admin.EntityFramework.Shared.csproj", "src/Falc.Identity.Admin.EntityFramework.Shared/"]
COPY ["src/Falc.Identity.Admin.EntityFramework.SqlServer/Falc.Identity.Admin.EntityFramework.SqlServer.csproj", "src/Falc.Identity.Admin.EntityFramework.SqlServer/"]
COPY ["src/Falc.Identity.Shared/Falc.Identity.Shared.csproj", "src/Falc.Identity.Shared/"]
COPY ["src/Falc.Identity.Admin.EntityFramework.PostgreSQL/Falc.Identity.Admin.EntityFramework.PostgreSQL.csproj", "src/Falc.Identity.Admin.EntityFramework.PostgreSQL/"]
COPY ["src/Falc.Identity.Admin.EntityFramework.MySql/Falc.Identity.Admin.EntityFramework.MySql.csproj", "src/Falc.Identity.Admin.EntityFramework.MySql/"]
RUN dotnet restore -a $TARGETARCH "src/Falc.Identity.Admin/Falc.Identity.Admin.csproj"
COPY . .
WORKDIR "/src/src/Falc.Identity.Admin"
RUN dotnet build -a $TARGETARCH "Falc.Identity.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -a $TARGETARCH "Falc.Identity.Admin.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "Falc.Identity.Admin.dll"]