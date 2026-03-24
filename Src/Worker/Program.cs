using MailVault.Worker;

var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(o => o.ServiceName = "MailVault")
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();

        logging.AddConsole();

        if (OperatingSystem.IsWindows())
        {
            logging.AddEventLog(s => s.SourceName = "MailVault");
        }
    })
    .ConfigureServices((ctx, services) =>
    {
        services.Configure<AppConfig>(ctx.Configuration.GetSection("MailVault"));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();