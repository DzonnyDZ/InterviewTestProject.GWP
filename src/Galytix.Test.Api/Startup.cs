using Galytix.Test.Business;
using Galytix.Test.Business.Configuration;
using Galytix.Test.Data.Csv;
using Galytix.Test.Data.Csv.Configuration;
using Microsoft.OpenApi.Models;

namespace Galytix.Test.API;

/// <summary>Configures the web application at startup</summary>
public class Startup
{
    /// <summary>Version number do display in swagger documentation</summary>
    private const string SwaggerVersion = "v1";

    /// <summary>Initializes a new instance of the <see cref="Startup"/> class.</summary>
    /// <param name="configuration">Provides access to application configuration</param>
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>Gets an object which provides access to application configuration</summary>
    public IConfiguration Configuration { get; }

    /// <summary>Registers services for dependency injection</summary>
    /// <param name="services">The service collection to register the services with</param>
    public void ConfigureServices(IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));
        services.AddControllers();
        services.AddDataLayer();
        services.AddBusinessLayer();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(SwaggerVersion, new OpenApiInfo { Title = "Galytix.Test", Version = SwaggerVersion });
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, typeof(Startup).Assembly.GetName().Name + ".xml"), true);
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, typeof(Dto.LobRequest).Assembly.GetName().Name + ".xml"));
        });

        services.AddOptions<CsvRepositoryOptions>().BindConfiguration("CsvRepository");
        services.AddOptions<LobStatsBusinessOptions>().BindConfiguration("LobStatsBusiness");
    }

    /// <summary>Sets up application configuration</summary>
    /// <param name="app">Application builder</param>
    /// <param name="env">Provides access to hosting environment</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (app is null) throw new ArgumentNullException(nameof(app));

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{typeof(Startup).Namespace} {SwaggerVersion}"));

        app.UseRouting();
        //app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
