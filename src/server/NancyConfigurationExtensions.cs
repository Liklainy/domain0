﻿using Nancy.Bootstrapper;
using System;
using System.Security.Cryptography.X509Certificates;
using Nancy.Hosting.Self;
using Topshelf;
using Topshelf.HostConfigurators;

namespace Domain0.WinService.Infrastructure
{
    public static class NancyConfigurationExtensions
    {
        public static void WithNancy(this HostConfigurator configurator, 
            Uri uri, 
            HostConfiguration configuration, 
            INancyBootstrapper bootstrapper, 
            X509Certificate2 x509Cert)
        {
            configurator.BeforeInstall(settings =>
            {
                try
                {
                    NancyService.InstallService(uri, x509Cert);
                }
                catch (Exception ex)
                {
                    // Install service exception
                    settings.ExceptionCallback(ex);
                    throw;
                }
            });
            configurator.BeforeUninstall(() => NancyService.UninstallService(uri));
            configurator.Service(settings =>
            {
                try
                {
                    return new NancyService(uri, configuration, bootstrapper, x509Cert);
                }
                catch (Exception ex)
                {
                    // Application startup exception
                    settings.ExceptionCallback(ex);
                    throw;
                }
            });


        }
    }
}
