#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
RUN apt-get update && \
    apt-get install -y krb5-user && \
    apt-get clean
COPY ./SupportFiles/krb/krb5.conf /etc/krb5.conf
COPY ./SupportFiles/krb/mzhang.keytab /etc/mzhang.keytab
RUN chmod 600 /etc/mzhang.keytab
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DocGenDispatcher/DocGenDispatcher.csproj", "DocGenDispatcher/"]
RUN dotnet restore "DocGenDispatcher/DocGenDispatcher.csproj"
COPY . .
WORKDIR "/src/DocGenDispatcher"
RUN dotnet build "DocGenDispatcher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocGenDispatcher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true