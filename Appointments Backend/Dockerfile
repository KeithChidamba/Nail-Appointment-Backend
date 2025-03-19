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

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Run the DLL (note the space escape)
ENTRYPOINT ["/app/Appointments Backend.dll"]

# List contents to verify that DLL is present
RUN ls -l /app
