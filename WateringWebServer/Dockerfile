FROM microsoft/dotnet:2.1-runtime AS runtime
COPY netcoreapp2.1/. ./netcoreapp2.1/
WORKDIR /netcoreapp2.1/
ENTRYPOINT ["dotnet", "WateringWebServer.dll"]
