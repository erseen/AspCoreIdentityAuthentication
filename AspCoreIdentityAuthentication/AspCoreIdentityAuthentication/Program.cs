using AspCoreIdentityAuthentication.EmailService;
using AspCoreIdentityAuthentication.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var services = builder.Services;
//Identity config   
services.AddDbContext<ApplicationContext>(options => options.UseSqlite("Data Source = shopDb"));
services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
services.Configure<IdentityOptions>(options =>
{
    //pasword
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;

    //Lockout
    options.Lockout.MaxFailedAccessAttempts = 5;
    //Oturum açýk kalma süresi 
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

});
///Email Sender için config ayarlarý paranteze alýndý 
services.AddTransient<IEmailSender, EmailSender>();
services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/accessdenied";
    options.SlidingExpiration = false;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie = new CookieBuilder
    {
        HttpOnly = true,
        Name = ".ShopApp.Security.Cooki",
        SameSite = SameSiteMode.Strict

    };

});
   ////////////


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
