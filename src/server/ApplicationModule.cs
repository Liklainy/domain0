﻿using System;
using System.Collections.Generic;
using System.Configuration;
using Autofac;
using AutoMapper;
using Domain0.Nancy.Infrastructure;
using Domain0.Nancy.Service;
using Domain0.Service;
using Microsoft.IdentityModel.Tokens;

namespace Domain0
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(ReadTokenSettings());
            builder.RegisterType<TokenGenerator>().As<ITokenGenerator>().SingleInstance();

            builder.RegisterType<MapperProfile>().As<Profile>().SingleInstance();
            builder.RegisterType<PasswordGenerator>().As<IPasswordGenerator>().SingleInstance();
            builder.RegisterType<AccountService>().As<IAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<AdminService>().As<IAdminService>().InstancePerLifetimeScope();

            builder.RegisterInstance(ReadSmsQueueSettings());
            builder.RegisterType<SqlQueueSmsClient>().As<ISmsClient>();
            builder
                .RegisterType<AuthenticationConfigurationBuilder>()
                .As<IAuthenticationConfigurationBuilder>()
                .SingleInstance();

            builder.RegisterInstance(ReadEmailClientSettings());
            builder.RegisterType<EmailClient>().As<IEmailClient>();

            builder.Register(container =>
            {
                var profiles = container.Resolve<IEnumerable<Profile>>();
                var mapper = new MapperConfiguration(c =>
                {
                    foreach (var profile in profiles)
                        c.AddProfile(profile);
                }).CreateMapper();
                return mapper;

            }).As<IMapper>().SingleInstance();
        }

        private static TokenGeneratorSettings ReadTokenSettings()
        {
            var settings = new TokenGeneratorSettings
            {
                Audience = ConfigurationManager.AppSettings["Token_Audience"],
                Issuer = ConfigurationManager.AppSettings["Token_Issuer"],
                Lifetime = TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings["Token_LifeTime"])),
                RefreshLifetime = TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings["Token_RefreshLifeTime"])),
                Secret = ConfigurationManager.AppSettings["Token_Secret"],
                Private = ConfigurationManager.AppSettings["Token_Private"],
                Public = ConfigurationManager.AppSettings["Token_Public"],
                Alg = ConfigurationManager.AppSettings["Token_Alg"],
            };
            return settings;
        }

        private static EmailClientSettings ReadEmailClientSettings()
        {
            return new EmailClientSettings
            {
                ServerHost = ConfigurationManager.AppSettings["Email_ServerHost"],
                Port = int.Parse(ConfigurationManager.AppSettings["Email_Port"]),
                Email = ConfigurationManager.AppSettings["Email_Address"],
                Username = ConfigurationManager.AppSettings["Email_Username"],
                Password = ConfigurationManager.AppSettings["Email_Password"],
            };
        }

        private static SqlQueueSmsClientSettings ReadSmsQueueSettings()
        {
            return new SqlQueueSmsClientSettings
            {
                ConnectionString = ConfigurationManager.AppSettings["SmsQueueClient_ConnectionString"],
                QueueName = ConfigurationManager.AppSettings["SmsQueueClient_QueueName"]
            };
        }
    }
}
