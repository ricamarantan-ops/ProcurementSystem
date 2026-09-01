using Microsoft.EntityFrameworkCore;
using ProcurementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Framework Controller setup
builder.Services.AddControllersWithViews();

// 2. Local Database engine connection string configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=LGCHA_ProcurementDb.db"));

// 3. Short-term application memory allocation rules
builder.Services.AddSession();

// 4. View Context Injection Pipeline Handler (FIXES YOUR TOP-LEVEL EXCEPTION)
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5. Active Session execution state management initialization
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();