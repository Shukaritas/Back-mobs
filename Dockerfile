
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY RentalPeAPI/RentalPeAPI.csproj RentalPeAPI/
RUN dotnet restore RentalPeAPI/RentalPeAPI.csproj

COPY . .

RUN dotnet publish "RentalPeAPI/RentalPeAPI.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .


ENV ASPNETCORE_URLS=http://+:8080


EXPOSE 8080

ENTRYPOINT ["dotnet", "RentalPeAPI.dll"]