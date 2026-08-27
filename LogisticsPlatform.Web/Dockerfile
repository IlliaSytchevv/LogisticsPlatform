FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY LogisticsPlatform.sln ./
COPY LogisticsPlatform.Domain/LogisticsPlatform.Domain.csproj LogisticsPlatform.Domain/
COPY LogisticsPlatform.Application/LogisticsPlatform.Application.csproj LogisticsPlatform.Application/
COPY LogisticsPlatform.Infrastructure/LogisticsPlatform.Infrastructure.csproj LogisticsPlatform.Infrastructure/
COPY LogisticsPlatform.Web/LogisticsPlatform.Web.csproj LogisticsPlatform.Web/

RUN dotnet restore LogisticsPlatform.Web/LogisticsPlatform.Web.csproj

COPY LogisticsPlatform.Domain/ LogisticsPlatform.Domain/
COPY LogisticsPlatform.Application/ LogisticsPlatform.Application/
COPY LogisticsPlatform.Infrastructure/ LogisticsPlatform.Infrastructure/
COPY LogisticsPlatform.Web/ LogisticsPlatform.Web/

RUN dotnet publish LogisticsPlatform.Web/LogisticsPlatform.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LogisticsPlatform.Web.dll"]
