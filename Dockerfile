FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY OnlineShopV1/*.csproj ./OnlineShopV1/
COPY TestV1/*.csproj ./TestV1/
RUN dotnet restore

# copy and build everything else
COPY OnlineShopV1/. ./OnlineShopV1/
COPY TestV1/. ./TestV1/

RUN dotnet build

FROM build AS testrunner
WORKDIR /app/TestV1
ENTRYPOINT ["dotnet", "test","--logger:trx"]

FROM build AS test
WORKDIR /app/TestV1
RUN dotnet test

FROM test AS publish
WORKDIR /app/OnlineShopV1
RUN dotnet publish -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/OnlineShopV1/out ./
ENTRYPOINT ["dotnet", "OnlineShopV1.dll"]