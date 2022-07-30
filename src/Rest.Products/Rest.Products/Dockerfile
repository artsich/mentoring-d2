FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV DatabaseSetting__ConnectionString=mongodb://host.docker.internal:27017
ENV DatabaseSetting__Name=Market

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Rest.Products/Rest.Products.csproj", "Rest.Products/"]
RUN dotnet restore "Rest.Products/Rest.Products.csproj"
COPY . .
WORKDIR "/src/Rest.Products"
RUN dotnet build "Rest.Products.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rest.Products.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rest.Products.dll"]