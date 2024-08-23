using Microsoft.EntityFrameworkCore;
using TrainVault.DataAccess;
using TrainVault.Interfaces;
using TrainVault.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//for accessing the DbContext 
var provider = builder.Services.BuildServiceProvider();
var config = provider.GetRequiredService<IConfiguration>();
builder.Services.AddDbContext<TrainVaultContext>(item => item.UseSqlServer(config.GetConnectionString("DefaultConnection")));

//to access repository and interface 
builder.Services.AddScoped<IOrganization, OrganizationRepository>();
builder.Services.AddScoped<IEmployee, EmployeeRepository>();
builder.Services.AddScoped<ITraining, TrainingRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


//Scaffold - DbContext "server=DESKTOP-LI40AFI\SQLEXPRESS; database=TrainVault; trusted_connection=true; TrustServerCertificate=true;" Microsoft.EntityFrameworkCore.SqlServer - OutputDir DataAccess