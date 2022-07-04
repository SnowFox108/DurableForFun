#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
RUN apt-get update -qq && apt-get -y install libgdiplus libc6-dev
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PdfDockerTest/PdfDockerTest.csproj", "PdfDockerTest/"]
RUN dotnet restore "PdfDockerTest/PdfDockerTest.csproj"
COPY . .
WORKDIR "/src/PdfDockerTest"
RUN dotnet build "PdfDockerTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PdfDockerTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PdfDockerTest.dll"]