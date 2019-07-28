using System;
using System.Threading;
using JustEat.StatsD;
using Microsoft.Extensions.DependencyInjection;

namespace Stats
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DotNetEnv.Env.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine(".env file not available");
            }

            Console.WriteLine("Application Starting...");
            var shutdownCts = new CancellationTokenSource();

            // Create service collection and configure our services
            var services = ConfigureServices();
            // Generate a provider
            var serviceProvider = services.BuildServiceProvider();

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                Console.WriteLine("Received Stop Event.");
                shutdownCts.Cancel();
            };

            var settings = serviceProvider.GetService<Settings>();
            services.AddStatsD(settings.StatsDServer);


            while (!shutdownCts.IsCancellationRequested)
            {
                try
                {
                    serviceProvider.GetService<Runner>().Run(shutdownCts.Token).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Thread.Sleep(TimeSpan.FromSeconds(int.Parse(settings.Interval)));
            }

            Console.WriteLine("Application closed.");
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<Runner>();
            services.AddSingleton<Settings>();
            services.AddHttpClient();


            services.AddStatsD(
                (provider) =>
                {
                    var options = provider.GetRequiredService<Settings>();

                    return new StatsDConfiguration()
                    {
                        Host = options.StatsDServer,
                        Port = 8125,
                        Prefix = options.AppName,
                        OnError = ex =>
                        {
                            Console.WriteLine(ex);
                            return true;
                        }
                    };
                });

            return services;
        }
    }
}
