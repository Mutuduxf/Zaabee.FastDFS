using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zaaby.DFS.Core;
using Zaaby.DFS.FastDfsProvider;
using Zaaby.DFS.FastDfsProvider.Mongo;

namespace Demo
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
            services.AddSingleton<IHandler, ZaabyFastDfsClient>(p =>
                new ZaabyFastDfsClient(new List<IPEndPoint> {new IPEndPoint(IPAddress.Parse("192.168.78.152"), 22122)},
                    "group1", services.BuildServiceProvider().GetService<IRepository>()));
            
            //Register the FastDfsClient repostory
            services.AddSingleton<IRepository, Repository>(p =>
                new Repository(new MongoDbConfiger(new List<string> {"192.168.5.61:27017"}, "FlytOaData", "FlytOaDev",
                    "2016")));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}