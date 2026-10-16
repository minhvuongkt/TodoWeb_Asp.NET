
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
        .AddMvcOptions(option =>
        {
            option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });
// Add services to the container.
builder.Services.AddControllersWithViews();
// Add Distrubuted Caching
builder.Services.AddDistributedMemoryCache();
// Add Options Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
//Using Session To Stored Account Info
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

string connectionString = @"server=.\SQLEXPRESS;user id=sa;password=123;database=TodoDB;TrustServerCertificate=True;";
_21T1020851_Todo.BusinessLayers.Config.Initialize(connectionString);

app.Run();