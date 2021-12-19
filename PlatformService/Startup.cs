using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.Repositories;
using PlatformService.DataServices.SyncDataServices;
using PlatformService.DataServices.SyncDataServices.Http;
using System;
using PlatformService.DataServices.ASyncDataServices;

namespace PlatformService
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsProduction())
            {
                Console.WriteLine("--> Using SQL server DB");
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(_configuration.GetConnectionString("PlatformConn"))
                );
            }
            else
            {
                Console.WriteLine("--> Using InMem DB");
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseInMemoryDatabase("InMem")
                );
            }
            
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });

            services.AddScoped<IPlatformRepo, PlatformRepo>();

            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

            Console.WriteLine(_configuration["CommandService"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UsePathBase("/platform");

            app.UseSwagger(opt => {
                opt.RouteTemplate = "psui/swagger/{documentname}/swagger.json";
            });
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/psui/swagger/v1/swagger.json", "PlatformService v1");
                opt.RoutePrefix = "psui/swagger";
            });

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
