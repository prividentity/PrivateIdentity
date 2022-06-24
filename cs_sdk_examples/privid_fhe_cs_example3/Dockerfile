#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["privid_fhe_cs_example2/privid_fhe_cs_example2.csproj", "privid_fhe_cs_example2/"]
RUN dotnet restore "privid_fhe_cs_example2/privid_fhe_cs_example2.csproj"
COPY . .
WORKDIR "/src/privid_fhe_cs_example2"
RUN dotnet build "privid_fhe_cs_example2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "privid_fhe_cs_example2.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "privid_fhe_cs_example2.dll"]