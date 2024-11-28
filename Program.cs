using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using TeaWork.Components;
using TeaWork.Components.Account;
using TeaWork.Data;
using TeaWork.Logic.Services.Interfaces;
using TeaWork.Logic.Services;
using TeaWork.Logic.Hubs;
using Radzen;
using TeaWork.Logic.DbContextFactory;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSignalR();

builder.Services.AddBlazorBootstrap();

builder.Services.AddRadzenComponents();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<UserIdentity>();
builder.Services.AddScoped<IDbContextFactory, DbContextFactory>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDesignConceptService, DesignConceptService>();
builder.Services.AddScoped<INotificationService, TeaWork.Logic.Services.NotificationService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

builder.Services.AddLocalization();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<LanguageService>();


builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
builder.Services.AddAuthentication()
   .AddGoogle(options =>
   {
       options.ClientId = builder.Configuration["GoogleKey:ClientId"];
       options.ClientSecret = builder.Configuration["GoogleKey:SecretKey"];
   });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IDbContextFactory, DbContextFactory>();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddSignalR();

var app = builder.Build();
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("pl"),
};
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapAdditionalIdentityEndpoints();

app.MapHub<CommunicationHub>("/communicationhub");

app.Run();
