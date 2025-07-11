namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>().
                    UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50 MB
                    })
                    .ConfigureServices(services =>
                        {
                            services.AddHealthChecks(); // Add health checks
                        })
                        .UseIISIntegration()
                        .CaptureStartupErrors(true);
                });
    }
}