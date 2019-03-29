using System;
using System.Collections.Generic;
using System.Linq;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Options;

namespace HomeLibrary.Service
{
    public class BusConfigurator
    {
        private readonly IOptions<MassTransitConfigConstants> _massTransitConfigOptions;


        public BusConfigurator(IOptions<MassTransitConfigConstants> massTransitConfigOptions)
        {
            _massTransitConfigOptions = massTransitConfigOptions;
        }

        public IBusControl ConfigureBus(
            Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost> registrationAction = null)
        {
            MassTransitConfigConstants massTransitConfigConstants = _massTransitConfigOptions.Value;
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                IRabbitMqHost host = cfg.Host(new Uri(massTransitConfigConstants.HostAddress), hst =>
                {
                    hst.Username(massTransitConfigConstants.Username);
                    hst.Password(massTransitConfigConstants.Password);
                });
                registrationAction?.Invoke(cfg, host);
            });


            return busControl;
        }
    }
}