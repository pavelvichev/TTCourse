using GFL.NET;
using GFL.NET.Interfaces;
using GFL.NET.Mocks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICatalogRepository, CatalogMock>();
builder.Services.AddDbContextPool<AppDbContext>(options => 
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogsDbConnection"));
	});
 
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
    pattern: "{controller=Catalog}/{action=Catalog}/{id?}"   
);
app.Run();
