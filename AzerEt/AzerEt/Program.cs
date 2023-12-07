using AzerEt.Data;
using AzerEt.Hubs;
using AzerEt.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;




var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer("name=ConnectionStrings:AzerEtCS"));
builder.Services.AddSession();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>() 
//    .AddDefaultTokenProviders();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddSignalR();
builder.Services.AddScoped<PaymentService>();
builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => {
                options.Cookie.Name = "RememberMecookie"; // cookie name
                options.LoginPath = "/Account/Login"; // view where the cookie will be issued for the first time
                options.ExpireTimeSpan = TimeSpan.FromDays(30); // time for the cookei to last in the browser
                options.SlidingExpiration = true; // the cookie would be re-issued on any request half way through the ExpireTimeSpan
                options.EventsType = typeof(CookieAuthEvent);
            });

//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.Name = ".AspNetCore.Identity.Application";
//    options.ExpireTimeSpan = TimeSpan.FromDays(30);
//    options.SlidingExpiration = true;
    
   
//});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



//app.MapControllerRoute(
//     name: "admin",
////pattern: "admin/{controller=Home}/{action=Index}/{id?}");
//pattern: "admin/{controller=Register}/{action=Login}");



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chatHub");

app.Run();

public class CookieAuthEvent : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        context.Request.HttpContext.Items.Add("ExpiresUTC", context.Properties.ExpiresUtc);
    }
}
