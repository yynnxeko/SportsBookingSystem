using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Application.Interfaces.IRepositories;
using SportsBookingSystem.Application.Interfaces.IService;
using SportsBookingSystem.Application.Mappings;
using SportsBookingSystem.Application.Services;
using SportsBookingSystem.Infrastructure.Data;
using SportsBookingSystem.Infrastructure.Repositories;
using VNPAY.NET;
using SportsBookingSystem.Application.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<SportsBookingSystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddControllers();

//AutoMapper
builder.Services.AddAutoMapper(typeof(UserMappingProfile));

//Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

// VnPay api
builder.Services.AddSingleton<IVnpay, Vnpay>();

// Options config
builder.Services.Configure<VnpayOptions>(builder.Configuration.GetSection(VnpayOptions.Vnpay));
builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection(PaymentSettings.SectionName));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
