FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

COPY ./Luciano.Serafim.Banking.Core.sln .
COPY ./Luciano.Serafim.Banking.Core.Api ./Luciano.Serafim.Banking.Core.Api
COPY ./Luciano.Serafim.Banking.Core.Abstractions ./Luciano.Serafim.Banking.Core.Abstractions
COPY ./Luciano.Serafim.Banking.Core.MediatR ./Luciano.Serafim.Banking.Core.MediatR
COPY ./Luciano.Serafim.Banking.Core.Models ./Luciano.Serafim.Banking.Core.Models
COPY ./Luciano.Serafim.Banking.Core.UseCases ./Luciano.Serafim.Banking.Core.UseCases
COPY ./Luciano.Serafim.Banking.Core.Utils ./Luciano.Serafim.Banking.Core.Utils
COPY ./Luciano.Serafim.Banking.Core.Bootstrap ./Luciano.Serafim.Banking.Core.Bootstrap
COPY ./Luciano.Serafim.Banking.Core.Infrastructure ./Luciano.Serafim.Banking.Core.Infrastructure
COPY ./Luciano.Serafim.Banking.Core.Tests ./Luciano.Serafim.Banking.Core.Tests

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App

COPY --from=build-env /App/out .

ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Luciano.Serafim.Banking.Core.Api.dll"]