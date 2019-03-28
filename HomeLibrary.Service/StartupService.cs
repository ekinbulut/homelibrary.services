using System;
using System.Threading;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Topshelf;

namespace HomeLibrary.Service
{
    public class StartupService : ServiceControl
    {
        private readonly ILogger<StartupService>              _logger;
        private readonly IBusControl                          _busControl;
        private readonly IOptions<MassTransitConfigConstants> _massTransitConfigConstantsOptions;

        public StartupService(ILogger<StartupService> logger, IBusControl busControl,
                              IOptions<MassTransitConfigConstants> massTransitConfigConstantsOptions)
        {
            _logger = logger;
            _busControl = busControl;
            _massTransitConfigConstantsOptions = massTransitConfigConstantsOptions;
        }

        public bool Start(HostControl hostControl)
        {
            MassTransitConfigConstants massTransitConfigConstants = _massTransitConfigConstantsOptions.Value;
            _logger.LogInformation($"{AppConstants.WinServiceProjectName} will be start");
            try
            {
                double milliSeconds = TimeSpan.FromSeconds(massTransitConfigConstants.BusStartStopTimeoutSeconds)
                                              .TotalMilliseconds;
                CancellationToken cancellationToken = new CancellationTokenSource(Convert.ToInt32(milliSeconds)).Token;
                _busControl.StartAsync(cancellationToken)
                           .GetAwaiter()
                           .GetResult();
            }
            catch (Exception e)
            {
                string message = $"Bus could not started in {AppConstants.WinServiceProjectName}";
                _logger.Log(LogLevel.Critical,
                            default(EventId),
                            GetType(),
                            e,
                            (type, exception) => message);
            }

            _logger.LogInformation($"{AppConstants.WinServiceProjectName} started");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            MassTransitConfigConstants massTransitConfigConstants = _massTransitConfigConstantsOptions.Value;
            _logger.LogInformation($"{AppConstants.WinServiceProjectName} will be stop");
            try
            {
                double milliSeconds = TimeSpan.FromSeconds(massTransitConfigConstants.BusStartStopTimeoutSeconds)
                                              .TotalMilliseconds;
                CancellationToken cancellationToken = new CancellationTokenSource(Convert.ToInt32(milliSeconds)).Token;
                _busControl.StopAsync(cancellationToken)
                           .GetAwaiter()
                           .GetResult();
            }
            catch (Exception e)
            {
                string message = $"Bus could not started in {AppConstants.WinServiceProjectName}";
                _logger.Log(LogLevel.Critical,
                            default(EventId),
                            GetType(),
                            e,
                            (type, exception) => message);
            }

            _logger.LogInformation($"{AppConstants.WinServiceProjectName} stopped");
            return true;
        }
    }
}