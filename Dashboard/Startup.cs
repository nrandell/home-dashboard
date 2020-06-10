using System.Threading.Channels;

using Dashboard.Hubs;
using Dashboard.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MQTTnet;

using Nick.Mqtt;

namespace Dashboard
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MqttConfiguration>(Configuration.GetSection("Mqtt"));

            services.AddControllersWithViews();
            services.AddSignalR();
            services.AddSingleton<IMqttFactory, MqttFactory>();
            services.AddSingleton<MqttReceiverService>();
            services.AddSingleton<ProcessingService>();

            services.AddSingleton(_ => Channel.CreateBounded<MqttApplicationMessage>(new BoundedChannelOptions(1000)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.DropNewest,
                SingleReader = true,
                SingleWriter = true
            }));
            services.AddSingleton(sp => sp.GetRequiredService<Channel<MqttApplicationMessage>>().Reader);
            services.AddSingleton(sp => sp.GetRequiredService<Channel<MqttApplicationMessage>>().Writer);

            services.AddHostedService(sp => sp.GetRequiredService<ProcessingService>());


            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "dashboard-app/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<DataHub>("/data");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "dashboard-app";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4400");
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
