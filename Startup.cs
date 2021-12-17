using System.Data.Common;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToDoApi.Interfaces;
using ToDoApi.Repositories;
using ToDoApi.Services;
using ToDoApi.ServiceBus.Services;
using ToDoApi.ServiceBus.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using ToDoApi.ServiceBus;

namespace ToDoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services for DB Context
            services.AddDbContext<ToDoContext>(options => options.UseSqlServer(Configuration["SqlConnectionString"], action => action.MigrationsAssembly("ToDoApi")));

            // Add services for service bus
            var serviceProvider = services.BuildServiceProvider();
            services.AddSingleton<ISubscriptionManager, SubscriptionManager>();
            services.AddSingleton<IServiceBusConnectionManager>(s =>
            {
                var logger = s.GetRequiredService<ILogger<ServiceBusConnectionManager>>();
                var servicebusConnection = new ServiceBusConnectionStringBuilder(Configuration["ServiceBusConnectionString"]);
                return new ServiceBusConnectionManager(logger, servicebusConnection);
            });
            services.AddSingleton<IEventBus, AzureServiceBusEventBus>(s =>
            {
                var serviceBusConnectionManager = s.GetRequiredService<IServiceBusConnectionManager>();
                var logger = s.GetRequiredService<ILogger<AzureServiceBusEventBus>>();
                var subscriptionManager = s.GetRequiredService<ISubscriptionManager>();
                var eventBus = new AzureServiceBusEventBus(serviceBusConnectionManager, subscriptionManager, serviceProvider, logger, Configuration["SubscriptionClientName"]);
                eventBus.SetupAsync().GetAwaiter().GetResult();
                return eventBus;
            });

            services.AddTransient<Func<DbConnection, IEventLogService>>(s => (DbConnection connection) => new EventLogService(connection));
            services.AddTransient<IToDoEventService, ToDoEventService>(s => 
            {
                var toDoEventService = new ToDoEventService(
                    s.GetRequiredService<ToDoContext>(), 
                    s.GetRequiredService<Func<DbConnection, IEventLogService>>(),
                    s.GetRequiredService<IEventBus>(),
                    s.GetRequiredService<ILogger<ToDoEventService>>());
                return toDoEventService;
            });

            // Add other services
            services.AddControllers();
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "ToDo API";
                    document.Info.Description = "ToDo API for Rush Test";
                    document.Info.TermsOfService = "None";
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
