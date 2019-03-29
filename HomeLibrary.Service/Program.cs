using System;
using System.IO;
using System.Reflection;
using GreenPipes;
using Library.Common.Contracts.Events.BookEvents;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog.Extensions.Logging;
using Topshelf;

namespace HomeLibrary.Service
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            var configurationBuilder = new ConfigurationBuilder();
            BuildApplicationConfiguration(configurationBuilder);
            IConfigurationRoot configuration = configurationBuilder.Build();
            var massTransitConfigConstants = configuration.GetSection(AppConstants.MassTransitConfigSectionName)
                                                          .Get<MassTransitConfigConstants>();
            var serviceCollection = new ServiceCollection();
            
            AddLogging(serviceCollection);
            AddConfigurationConstants(serviceCollection, configuration);
            AddMassTransit(serviceCollection, massTransitConfigConstants);
            AddServiceControl(serviceCollection);
            
            ServiceProvider buildServiceProvider = serviceCollection.BuildServiceProvider();
            HostFactory.Run(cfg =>
            {
                cfg.SetServiceName(AppConstants.WinServiceProjectName);
                cfg.Service(s => buildServiceProvider.GetRequiredService<ServiceControl>());
            });
        }

        private static void BuildApplicationConfiguration(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddEnvironmentVariables();
        }

        private static void AddLogging(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(builder => builder.AddNLog());
        }

        private static void AddConfigurationConstants(IServiceCollection services, IConfiguration configuration)
        {
            
            services.Configure<MassTransitConfigConstants>(configuration.GetSection(AppConstants.MassTransitConfigSectionName));
        }

        private static void AddMassTransit(IServiceCollection serviceCollection,
                                           MassTransitConfigConstants massTransitConfigConstants)
        {
//            serviceCollection.AddTransient<IConsumeObserver, BasicConsumeObserver>();
//            serviceCollection.AddTransient<IPublishObserver, BasicPublishObservers>();

            serviceCollection.AddSingleton<BusConfigurator>();
            serviceCollection.AddMassTransit(x =>
                  {
                      x.AddConsumer<BookCreatedEventConsumer>();

                      x.AddBus(provider =>
                      {
                          var busConfigurator =
                              provider.GetRequiredService<BusConfigurator>();
                          IBusControl busControl =
                              busConfigurator.ConfigureBus((cfg, host) =>
                              {
                                  cfg.ReceiveEndpoint(host,
                                                      massTransitConfigConstants
                                                          .QueueNames
                                                          .BookCreatedEvent,
                                                      e =>
                                                      {
                                                          e.UseConcurrencyLimit(massTransitConfigConstants
                                                                                    .ConcurrencyLimit);
                                                          e.UseMessageRetry(r =>
                                                                                r.Incremental(massTransitConfigConstants.RetryLimitCount,
                                                                                              TimeSpan
                                                                                                  .FromSeconds(massTransitConfigConstants
                                                                                                                   .InitialIntervalSeconds),
                                                                                              TimeSpan
                                                                                                  .FromSeconds(massTransitConfigConstants
                                                                                                                   .IntervalIncrementSeconds)
                                                                                             ));
                                                          e.ConfigureConsumer<
                                                              BookCreatedEventConsumer
                                                          >(provider);
                                                      });
                              });
                          return busControl;
                      });
                  }
                 );
        }
        
        private static void AddServiceControl(IServiceCollection services)
        {
            services.AddSingleton<ServiceControl, StartupService>();
        }
    }
}