FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as dotnetbuild

RUN mkdir /app

WORKDIR /app

COPY ./Stats.csproj .
RUN dotnet restore

COPY . /app

RUN dotnet restore

RUN dotnet publish -c Release --output ./out

# Run image.
FROM mcr.microsoft.com/dotnet/core/runtime:2.2

WORKDIR /app
COPY --from=dotnetbuild /app/out .
CMD ["/usr/bin/dotnet", "/app/Stats.dll"]