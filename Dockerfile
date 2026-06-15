FROM mcr.microsoft.com/dotnet/sdk:8.0 AS compilacion
WORKDIR /codigo

COPY ["Lab13sardonmax/Lab13sardonmax.csproj", "Lab13sardonmax/"]
RUN dotnet restore "Lab13sardonmax/Lab13sardonmax.csproj"

COPY . .
RUN dotnet publish "Lab13sardonmax/Lab13sardonmax.csproj" \
    --configuration Release \
    --output /aplicacion/publicada \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /aplicacion
COPY --from=compilacion /aplicacion/publicada .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Lab13sardonmax.dll"]
