using BusinessLayer;
using BusinessLayer.Services.Abouts;
using BusinessLayer.Services.Admins;
using BusinessLayer.Services.AuditLog;
using BusinessLayer.Services.Auth;
using BusinessLayer.Services.Branches;
using BusinessLayer.Services.CargoPricing;
using BusinessLayer.Services.Cargos;
using BusinessLayer.Services.CargoService;
using BusinessLayer.Services.Cities;
using BusinessLayer.Services.Customers;
using BusinessLayer.Services.Delivery;
using BusinessLayer.Services.Employee;
using BusinessLayer.Services.QuikServices;
using BusinessLayer.Services.TransferCenters;
using BusinessLayer.Validators.Abouts;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories.Abouts;
using DataAccessLayer.Repositories.AdminDashboard;
using DataAccessLayer.Repositories.AuditLog;
using DataAccessLayer.Repositories.Branches;
using DataAccessLayer.Repositories.CargoMovements;
using DataAccessLayer.Repositories.CargoPrice;
using DataAccessLayer.Repositories.Cargos;
using DataAccessLayer.Repositories.Cities;
using DataAccessLayer.Repositories.Customers;
using DataAccessLayer.Repositories.Delivery;
using DataAccessLayer.Repositories.DeliveryException;
using DataAccessLayer.Repositories.Employees;
using DataAccessLayer.Repositories.QuikService;
using DataAccessLayer.Repositories.TransferCenters;
using EntityLayer.Entities;

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyAcademy_CargoTrack.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters()
    .AddValidatorsFromAssemblyContaining<BusinessAssembly>();



builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransferCenterService, TransferCenterService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<ICargoService, CargoService>();
builder.Services.AddScoped<ICargoPriceService, CargoPriceService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ICargoPricingService, CargoPricingService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<BusinessLayer.Services.QuikServices.IQuikService, BusinessLayer.Services.QuikServices.QuikService>();
builder.Services.AddScoped<BusinessLayer.Services.DeliveryExceptions.IDeliveryExceptionService, BusinessLayer.Services.DeliveryExceptions.DeliveryExceptionService>();




builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddIdentity<AppUser,AppRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddBusinessServices();
builder.Services.AddScoped<ICargoRepository, CargoRepository>();

builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<ITransferCenterRepository, TransferCenterRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ICargoPriceRepository, CargoPriceRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ICargoMovementRepository, CargoMovementRepository>();
builder.Services.AddScoped<IDeliveryExceptionRepository, DeliveryExceptionRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IQuikServiceRepository, QuikServiceRepository>();


builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
var app = builder.Build();
// Roller oluþturuluyor
await IdentitySeedData.SeedRolesAsync(app.Services);
await IdentitySeedData.SeedAdminAsync(app.Services);
await CargoTrackTestDataSeeder.SeedAsync(app.Services);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
           name: "areas",
           pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
         );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
