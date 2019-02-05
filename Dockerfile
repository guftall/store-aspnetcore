FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY OnlineShopV1/*.csproj ./onlineshopapp/
COPY TestV1/*.csproj ./tests/
RUN dotnet restore

# copy and build everything else
COPY OnlineShopV1/. ./onlineshopapp/
COPY TestV1/. ./tests/

RUN dotnet build

FROM build AS testrunner
WORKDIR /app/tests
ENTRYPOINT ["dotnet", "test","--logger:trx"]

FROM build AS test
WORKDIR /app/tests
RUN dotnet test

FROM test AS publish
WORKDIR /app/onlineshopapp
RUN dotnet publish -o out

FROM microsoft/dotnet:2.2-runtime AS runtime
WORKDIR /app
COPY --from=publish /app/onlineshopapp/out ./
ENTRYPOINT ["dotnet", "OnlineShopV1.dll"]