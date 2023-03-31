using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ElasticEmailTask.Services;
using ElasticEmailTask.Settings;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ElasticEmailTask.Interfaces;
using ElasticEmailTask;
using ElasticEmail.Api;
using ElasticEmail.Client;

var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location))
    .AddJsonFile("appsettings.json").Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var elasticEmailSettings = config.GetSection(nameof(ElasticEmailSettings)).Get<ElasticEmailSettings>();
        Configuration elasticEmailConfig = new Configuration();
        elasticEmailConfig.BasePath = elasticEmailSettings.Address;
        elasticEmailConfig.AddApiKey("X-ElasticEmail-ApiKey", elasticEmailSettings.ApiKey);
        services.AddSingleton(elasticEmailConfig);
        services.AddScoped<IElasticEmailService, ElasticEmailService>();
        services.AddScoped<IEmailsApi, EmailsApi>();

    })
    .Build();

using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;
var main = new Main(provider.GetService<IElasticEmailService>());
main.Run();

await host.RunAsync();

