namespace Galytix.Test.API;

/// <summary>Entry point class</summary>
public class Program
{
    /// <summary>Application entry point</summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        var app = CreateHostBuilder(args).Build();

        app.Run();
    }

    /// <summary>Creates web application host builder</summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Web application host builder</returns>
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
}