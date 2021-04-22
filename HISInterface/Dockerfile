#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
CMD ["dotnet", "HISInterface.dll"]

#设置时区（部署docker必须加上去）
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["AbpvNextHISInterface/HISInterface/HISInterface.csproj", "HISInterface/"]
COPY ["AbpvNextHISInterface/AbpvNext.Application.HISInterface/AbpvNext.Application.HISInterface.csproj", "AbpvNext.Application.HISInterface/"]
COPY ["AbpvNextHISInterface/AbpvNext.DDD.Domain.HISInterface/AbpvNext.DDD.Domain.HISInterface.csproj", "AbpvNext.DDD.Domain.HISInterface/"]
COPY ["AbpvNextHISInterface/AbpvNextEntityFrameworkCoreForOracle/AbpvNextEntityFrameworkCoreForOracle.csproj", "AbpvNextEntityFrameworkCoreForOracle/"]
COPY ["AbpvNextHISInterface/AbpvNext.HISInterface.Filters/AbpvNext.HISInterface.Filters.csproj", "AbpvNext.HISInterface.Filters/"]
COPY ["AbpvNextHISInterface/AbpvNext.DapperCore.HISInterface/AbpvNext.DapperCore.HISInterface.csproj", "AbpvNext.DapperCore.HISInterface/"]
RUN dotnet restore "HISInterface/HISInterface.csproj"

COPY . .
WORKDIR "/src/HISInterface"
RUN dotnet build "HISInterface.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HISInterface.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./HISInterface.xml /app
ENTRYPOINT ["dotnet", "HISInterface.dll"]