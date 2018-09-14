FROM microsoft/dotnet:2.1-runtime
ENV TZ=Europe/Berlin
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
COPY netcoreapp2.1/. ./netcoreapp2.1/
WORKDIR /netcoreapp2.1/
ENTRYPOINT ["dotnet", "PiWeb.dll"]
