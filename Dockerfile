FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Appointments Backend/Appointments Backend.csproj", "Appointments Backend/"]
RUN dotnet restore "Appointments Backend/Appointments Backend.csproj"
COPY . . 
WORKDIR "/src/Appointments Backend"
RUN dotnet build "Appointments Backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Appointments Backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN ls -al /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
RUN ls -al /app

ENTRYPOINT ["dotnet","Appointments Backend.dll"]
