using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Zaabee.FastDfsProvider.Abstractions;
using Zaabee.FastDfsProvider.Repository.Abstractions;

namespace Zaabee.FastDfsProvider.Demo
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
            //Register the FastDfsClient
            services.AddSingleton<IHandler, ZaabyFastDfsClient>(serviceProvide =>
                new ZaabyFastDfsClient(new List<IPEndPoint> {new(IPAddress.Parse("192.168.78.152"), 22122)},
                    "group1", serviceProvide.GetService<IRepository>()));

            //Register the FastDfsClient repository
            services.AddSingleton<IRepository, Mongo.Repository>(p =>
                new Mongo.Repository(new MongoClient(
                        "mongodb://admin:123@192.168.78.140:27017,192.168.78.141:27017,192.168.78.142/admin?authSource=admin&replicaSet=rs"),
                    "FastDFS"));

            services.AddControllers();
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Alice API",
                    Description = "API for AliceHost",
                    Contact = new OpenApiContact {Name = "DuXiaoFei", Email = "aeondxf@live.com"}
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();    
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alice API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}