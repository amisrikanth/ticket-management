FROM microsoft/dotnet:2.1-sdk
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 5000
RUN chmod +x docker_entrypoint.sh
CMD /bin/bash docker_entrypoint.sh