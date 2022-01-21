using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;

namespace PlatformService
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // While applyin the database migrations, comment out the else part and line 35 as well so that 
            // the build does not complain about migrations not being supported in InMemDb. 
           if (_env.IsProduction()) {
               Console.WriteLine("--> Using SqlServer Db");
               /* services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));
                */
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(Environment.GetEnvironmentVariable("PlatformsConn")));
            }
            else
            {
                Console.WriteLine("--> Using InMem Db");
                services.AddDbContext<AppDbContext>(opt =>
                     opt.UseInMemoryDatabase("InMem"));
            }
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
             services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddControllers();
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            PrepDb.PrepPopulation(app, env.IsProduction());
        }
    }
}
