# Sử dụng image .NET SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Đặt thư mục làm việc trong container
WORKDIR /app

# Copy các file csproj và restore các dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy phần còn lại của mã nguồn vào container
COPY . ./

# Build ứng dụng
RUN dotnet publish -c Release -o /app/publish

# Sử dụng image runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Đặt thư mục làm việc trong container
WORKDIR /app

# Copy các file đã build từ container build vào
COPY --from=build /app/publish .

# Mở cổng mà ứng dụng sẽ sử dụng
EXPOSE 80

# Chạy ứng dụng
ENTRYPOINT ["dotnet", "BKAC.dll"]
