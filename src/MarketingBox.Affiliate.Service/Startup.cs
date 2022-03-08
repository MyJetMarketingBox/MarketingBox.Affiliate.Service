﻿using System.Reflection;
using Autofac;
using MarketingBox.Affiliate.Postgres;
using MarketingBox.Affiliate.Service.Grpc;
using MarketingBox.Affiliate.Service.Modules;
using MarketingBox.Affiliate.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyJetWallet.Sdk.GrpcSchema;
using MyJetWallet.Sdk.Postgres;
using MyJetWallet.Sdk.Service;
using Prometheus;
using SimpleTrading.ServiceStatusReporterConnector;

namespace MarketingBox.Affiliate.Service
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.BindCodeFirstGrpc();

            services.AddHostedService<ApplicationLifetimeManager>();

            DatabaseContext.LoggerFactory = Program.LogFactory;
            services.AddDatabase(DatabaseContext.Schema, 
                Program.Settings.PostgresConnectionString, 
                o => new DatabaseContext(o));
            //DatabaseContext.LoggerFactory = null;

            services.AddMyTelemetry("MB-", Program.Settings.JaegerUrl);

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseMetricServer();

            app.BindServicesTree(Assembly.GetExecutingAssembly());

            app.BindIsAlive();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcSchema<AffiliateService, IAffiliateService>();
                endpoints.MapGrpcSchema<AffiliateAccessService, IAffiliateAccessService>();
                endpoints.MapGrpcSchema<CampaignService, ICampaignService>();
                endpoints.MapGrpcSchema<IntegrationService, IIntegrationService>();
                endpoints.MapGrpcSchema<BrandService, IBrandService>();
                endpoints.MapGrpcSchema<CampaignRowService, ICampaignRowService>();
                endpoints.MapGrpcSchema<OfferService, IOfferService>();
                endpoints.MapGrpcSchema<CountryService, ICountryService>();
                endpoints.MapGrpcSchema<GeoService, IGeoService>();

                endpoints.MapGrpcSchemaRegistry();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule<SettingsModule>();
            builder.RegisterModule<ServiceModule>();
        }
    }
}
