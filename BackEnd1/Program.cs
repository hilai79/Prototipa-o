using Microsoft.EntityFrameworkCore;
using ReservaSenacApi.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MySql");

builder.Services.AddDbContext<ReservaSenacContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
        policy.WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500",
                "http://localhost")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("FrontEnd");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
