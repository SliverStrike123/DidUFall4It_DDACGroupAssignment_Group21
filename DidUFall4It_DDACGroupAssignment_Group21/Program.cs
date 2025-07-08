using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DidUFall4It_DDACGroupAssignment_Group21.Data;
using DidUFall4It_DDACGroupAssignment_Group21.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DidUFall4It_DDACGroupAssignment_Group21ContextConnection") ?? throw new InvalidOperationException("Connection string 'DidUFall4It_DDACGroupAssignment_Group21ContextConnection' not found.");;

builder.Services.AddDbContext<DidUFall4It_DDACGroupAssignment_Group21Context>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<DidUFall4It_DDACGroupAssignment_Group21User>
    (options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DidUFall4It_DDACGroupAssignment_Group21Context>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 20971520; 
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.Run();
