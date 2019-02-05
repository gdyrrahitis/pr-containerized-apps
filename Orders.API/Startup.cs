using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orders.API.Models;
using RabbitMQ.Client;

namespace Orders.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EnvironmentConfiguration>(Configuration);
            services.AddSingleton<IRabbitMqManager>(s =>
            {
                var options = s.GetService<IOptions<EnvironmentConfiguration>>();
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = options.Value.EventBusConnection
                };
                return new RabbitMqManager(connectionFactory, options);
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRabbitListener();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }

    public static class ApplicationBuilderExtensions
    {
        public static IRabbitMqManager Listener { get; set; }

        public static IApplicationBuilder UseRabbitListener(this IApplicationBuilder app)
        {
            Listener = app.ApplicationServices.GetService<IRabbitMqManager>();
            var life = app.ApplicationServices.GetService<IApplicationLifetime>();
            life.ApplicationStarted.Register(OnStarted);
            life.ApplicationStopping.Register(OnStopping);
            return app;
        }

        private static void OnStarted() => Listener.CreateConsumerChannel();

        private static void OnStopping() => Listener.Disconnect();
    }
}
