using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    /* Inicializa la configuración */
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    /* Dependency injection */
    public void ConfigureServices(IServiceCollection services)
    {
        /* Configuración del contexto de la base de datos (definido en appsettings.json)*/
       /* services.AddDbContext<ApplicationDbContext>(
            options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
       */
        /* uso de autenticación basada en jwt integrando el AD de Azure (esto se define en el appsettings.json)*/
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));
        /* services agregan servicios necesarios para controladores MVC */
        services.AddControllers();
    }

    /* Definimos cóno la aplicación responde las solicitudes */
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        /* Redirigimos http a https */
        app.UseHttpsRedirection();

        app.UseRouting();
        /* Manejo de la autenticación y autorización en las solicitudes */
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
