#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base

RUN apt-get update 
RUN apt-get install -y apt-utils libgdiplus libc6-dev
RUN apt-get install -y libx11-dev

WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["PdfGenerator.csproj", "PdfGenerator/"]
RUN dotnet restore "PdfGenerator.csproj"
COPY . .
WORKDIR "/src/PdfGenerator"
RUN dotnet build "PdfGenerator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PdfGenerator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true