using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using YouTrack.Management.AssigneeActualize.Client;
using YouTrack.Management.Common;

namespace YouTrack.Management.ApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "YouTrack.Management.AssigneeActualize", Version = "v1" });
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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "YouTrack.Management.ApiGateway v1"));
            }

            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private IServiceCollection AddClients(IServiceCollection services)
        {
            var httpClientConfigurator = new DefaultHttpClientConfigurator();

            services.AddClient<AssigneeActualizeClient, AssigneeActualizeClientSettings>(Configuration, httpClientConfigurator);

            return services;
        }
    }
}