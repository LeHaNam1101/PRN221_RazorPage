using Microsoft.EntityFrameworkCore;
using SignalR;
using RazorPage.Model;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using EmailService;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
//Bổ sung service làm việc với các Pages vào container Kestrel
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
    options.LogoutPath = "/Account/SignOut";
});

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddDbContext<PRN221_DBContext>();

builder.Services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(5));

builder.Services.AddSignalR();

var app = builder.Build();

app.UseAuthentication();


if (!app.Environment.IsDevelopment())
{
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapHub<HubServer>("/hub");

app.Run();
