using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resend;
using UrlSamurai.Components;
using UrlSamurai.Components.Account;
using UrlSamurai.Components.Services;
using UrlSamurai.Data;

// ------------------------------------
// 1. Create builder & setup services
// ------------------------------------
var builder = WebApplication.CreateBuilder(args);

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Identity-related
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
}).AddIdentityCookies();

// DB Context
var postgresConnectionString = builder.Configuration.GetValue<string>("DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("Connection string 'DB_CONNECTION_STRING' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(postgresConnectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity setup
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// Email (Resend)
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = builder.Configuration.GetValue<string>("RESEND_APITOKEN")
        ?? throw new InvalidOperationException("Api key 'RESEND_APITOKEN' not found.");
});
builder.Services.AddScoped<IResend, ResendClient>();
builder.Services.AddScoped<IEmailSender<ApplicationUser>, IdentityResendClient>();

// CORS for public API access
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// QRCodes Generator
builder.Services.AddSingleton<QrCodeService>();

// Headers fix
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});


// ------------------------------------
// 2. Build app & configure middleware
// ------------------------------------
var app = builder.Build();

// Use CORS
app.UseCors();

// Global error handling
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseForwardedHeaders();

// Swagger (publicly available)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrlSamurai API V1");
    c.RoutePrefix = "swagger";
});

// ------------------------------------
// 3. Map endpoints
// ------------------------------------

// API controllers
app.MapControllers();

// Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Identity endpoints
app.MapAdditionalIdentityEndpoints();

app.Run();
