using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using ATAFurniture.Server.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Kroiko.Domain;
using Kroiko.Domain.CellsExtracting;
using Kroiko.Domain.ExcelFilesGeneration;
using Kroiko.Domain.ExcelFilesGeneration.XlsxWrapper;
using Kroiko.Domain.TemplateBuilding;
using Kroiko.Domain.TemplateBuilding.Lonira;
using Kroiko.Domain.TemplateBuilding.Suliver;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Radzen;
using Serilog;
using Syncfusion.Blazor;

namespace ATAFurniture.Server;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        // This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
        // By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
        // 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
        // This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token.
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        // Configuration to sign-in users with Azure AD B2C.
        services.AddMicrosoftIdentityWebAppAuthentication(Configuration);
        
        services.AddHttpContextAccessor();
            
        services.AddControllersWithViews(
            /*options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }*/
        ).AddMicrosoftIdentityUI();

        services.AddScoped<DialogService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<ContextMenuService>();
        services.AddScoped<TooltipService>();
        services.AddRazorComponents();
        
        services.AddSyncfusionBlazor();
        
        services.AddRazorPages();
        services.AddServerSideBlazor();

        var connectionString = Configuration.GetSection("SharkAspNetConnectionString").Value;
        services.AddDbContext<KroikoDataContext>(opt =>
        {
            opt.UseSqlServer(connectionString);
        });
        services.AddScoped<IKroikoDataRepository, KroikoDataRepository>();
        services.AddScoped<UserContextService>();
        
        services.AddScoped<IDetailsExtractorService, DetailsExtractorService>();
        services.AddKeyedScoped<ITemplateBuilder, LoniraTemplateBuilder>(nameof(SupportedCompanies.Lonira));
        services.AddKeyedScoped<ITableRowProvider, LoniraTableRowProvider>(nameof(SupportedCompanies.Lonira));
        services.AddKeyedScoped<IFileNameProvider, LoniraFileNameProvider>(nameof(SupportedCompanies.Lonira));
        services.AddKeyedScoped<ITemplateBuilder, SuliverTemplateBuilder>(nameof(SupportedCompanies.Suliver));
        services.AddKeyedScoped<ITableRowProvider, SuliverTableRowProvider>(nameof(SupportedCompanies.Suliver));
        services.AddKeyedScoped<IFileNameProvider, SuliverFileNameProvider>(nameof(SupportedCompanies.Suliver));
        services.AddScoped<IExcelFileGenerator, ExcelFileGenerator>();
        services.AddScoped<FileGeneratorService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<KroikoDataContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                Log.Logger.Warning("Applying migrations...");
                context.Database.Migrate();
                Log.Logger.Warning("Applying migrations completed...");
            }
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRewriter(
            new RewriteOptions().Add(
                context =>
                {
                    if (context.HttpContext.Request.Path == "/MicrosoftIdentity/Account/SignedOut")
                    {
                        context.HttpContext.Response.Redirect("/signout");
                    }
                }));

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}