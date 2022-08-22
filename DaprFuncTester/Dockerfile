#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base

#RUN apt-get update
#RUN apt-get -y install curl gnupg
#RUN curl -fsSL https://deb.nodesource.com/setup_current.x | bash -
#RUN apt-get install -y nodejs
#RUN npm i -g azure-functions-core-tools@4 --unsafe-perm true
#RUN apt-get install -y unzip
#RUN curl -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /remote_debugger

WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DaprFuncTester/DaprFuncTester.csproj", "DaprFuncTester/"]
RUN dotnet restore "DaprFuncTester/DaprFuncTester.csproj"
COPY . .
WORKDIR "/src/DaprFuncTester"
RUN dotnet build "DaprFuncTester.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DaprFuncTester.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true