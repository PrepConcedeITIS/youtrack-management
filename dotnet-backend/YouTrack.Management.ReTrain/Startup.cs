using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Quartz;
using YouTrack.Management.Common;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.ResolvedIssues.Client;

namespace YouTrack.Management.ReTrain
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
            services.AddQuartz(q =>
            {
                q.ScheduleJob<ReTrainJob>(triggerConfigurator =>
                    triggerConfigurator.WithCronSchedule("0 40 19 * * ?"));
            });

            services.AddQuartzServer(options => options.WaitForJobsToComplete = true);
            
            AddClients(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YouTrack.Management.ReTrain", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YouTrack.Management.ReTrain v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private void AddClients(IServiceCollection services)
        {
            var httpClientConfigurator = new DefaultHttpClientConfigurator();
            services.AddClient<ResolvedIssuesClient, ResolvedIssuesClientSettings>(Configuration,
                httpClientConfigurator);
            services.AddClient<MachineLearningClient, MachineLearningClientSettings>(Configuration,
                httpClientConfigurator);
        }
    }
}