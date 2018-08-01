FROM microsoft/dotnet:2.1-aspnetcore-runtime
ENV TZ=Europe/Berlin
#RUN mkdir /usr/share/man/man1 && \
#    mkdir /usr/share/man/man7 && \
#    apt-get update && apt-get install -y \
#    postgresql-client \
# && rm -rf /var/lib/apt/lists/* \
# && ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone \
# && rm -rf /usr/share/man/man1 \
# && rm -rf /usr/share/man/man7
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
COPY netcoreapp2.1/. ./netcoreapp2.1/
WORKDIR /netcoreapp2.1/
ENTRYPOINT ["dotnet", "WateringWebServer.dll"]
