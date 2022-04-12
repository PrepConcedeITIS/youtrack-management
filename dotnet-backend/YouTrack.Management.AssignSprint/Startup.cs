using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using YouTrack.Management.AssigneeActualize.Client;
using YouTrack.Management.AssignSprint.Interfaces;
using YouTrack.Management.AssignSprint.Services;
using YouTrack.Management.Common;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.YouTrack.Client;

namespace YouTrack.Management.AssignSprint
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
            services.AddControllers();

            AddClients(services);

            services.AddScoped<IIssueDistributionAlgorithm, StableMatchingAlghoritmService>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YouTrack.Management.AssignSprint", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "YouTrack.Management.AssignSprint v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private IServiceCollection AddClients(IServiceCollection services)
        {
            var httpClientConfigurator = new DefaultHttpClientConfigurator();
            services.AddClient<AssigneeActualizeClient, AssigneeActualizeClientSettings>(Configuration,
                httpClientConfigurator);
            services.AddClient<YouTrackClient, YouTrackClientSettings>(Configuration,
                new YouTrackClientConfigurator());
            services.AddClient<MachineLearningClient, MachineLearningClientSettings>(Configuration,
                httpClientConfigurator);
            return services;
        }
    }
}