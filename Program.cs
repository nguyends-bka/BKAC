using Microsoft.EntityFrameworkCore;
using BKAC.Data;  // Đảm bảo namespace đúng cho ApplicationDbContext
using BKAC.Models;
using BKAC.Services;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext và chuỗi kết nối
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BKAC1")));

// Thêm dịch vụ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient(); // Register IHttpClientFactory
builder.Services.AddScoped<RpcService>(); // Register your custom service


// Thêm dịch vụ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Cho phép mọi nguồn gốc
              .AllowAnyMethod()  // Cho phép mọi phương thức HTTP
              .AllowAnyHeader(); // Cho phép mọi header
    });
});

// Cấu hình các dịch vụ
builder.Services.AddControllers();

var app = builder.Build();

// Cấu hình các middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Áp dụng chính sách CORS
app.UseCors("AllowAll");

// Đảm bảo đăng ký các controller
app.MapControllers();

app.Run();
