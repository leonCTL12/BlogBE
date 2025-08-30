# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# in the container, go to the /src directory
WORKDIR /src

# Copy the project files from host machine to container's current dir (src)
COPY *.csproj ./
# Restore dependencies
RUN dotnet restore

# Copy the rest of the application code
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BlogBE.dll"]
