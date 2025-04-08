using Microsoft.EntityFrameworkCore;
using BKAC.Data;  // Đảm bảo namespace đúng cho ApplicationDbContext
using BKAC.Models;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext và chuỗi kết nối
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BKAC1")));

// Thêm dịch vụ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình các dịch vụ
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Đảm bảo đăng ký các controller
app.MapControllers();

app.Run();
