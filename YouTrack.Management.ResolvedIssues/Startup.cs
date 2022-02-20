using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Newtonsoft;
using YouTrack.Management.ResolvedIssues.Interfaces;
using YouTrack.Management.ResolvedIssues.Services;

namespace YouTrack.Management.ResolvedIssues
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo { Title = "YouTrack.Management.ResolvedIssues", Version = "v1" });
            });

            services.AddHttpClient("youtack", client =>
            {
                client.BaseAddress = new Uri("https://itis-showcase.youtrack.cloud/api/");
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Configuration.GetSection("YouTrack")?["Token"]);
                client.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("no-cache");
            });
            services.AddHttpClient("MockDataService", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetSection("MockDataService")["Url"]);
            });
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddScoped<IIssueLoader, YouTrackDoneIssuesLoader>();
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis")
                .Get<RedisConfiguration>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "YouTrack.Management.ResolvedIssues v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}