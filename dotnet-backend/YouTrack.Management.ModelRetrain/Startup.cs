using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Quartz;
using YouTrack.Management.Common;
using YouTrack.Management.MachineLearning.Client;
using YouTrack.Management.ModelRetrain.EF;
using YouTrack.Management.ResolvedIssues.Client;

namespace YouTrack.Management.ModelRetrain
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
            services.AddDbContext<RetrainDbContext>(builder =>
                builder.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddQuartz(q =>
            {
                q.ScheduleJob<RetrainJob>(triggerConfigurator =>
                    triggerConfigurator.WithCronSchedule("0 40 19 * * ?"));
            });

            services.AddQuartzServer(options => options.WaitForJobsToComplete = true);
            
            AddClients(services);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YouTrack.Management.ModelRetrain", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YouTrack.Management.ModelRetrain v1"));
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