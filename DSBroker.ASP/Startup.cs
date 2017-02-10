using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace DSBroker.ASP
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseMvc();

            /*app.Use(async (http, next) => {
                Console.WriteLine(http.WebSockets.IsWebSocketRequest);
                //if (http.Request.Path.Equals("/ws") && http.WebSockets.IsWebSocketRequest)
                {
                    
                }
                else
                {
                    await next();
                }
            });*/
            /*app.Use(async (http, next) =>
            {
                Console.WriteLine(http.WebSockets.IsWebSocketRequest);
                next();
            });*/
            //app.UseMiddleware();
        }
    }
}
