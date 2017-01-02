using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace DSBroker.ASPdotNET
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseWebSockets();
            app.Use(async (http, next) =>
            {
                if (http.Request.Path == "/ws" && http.WebSockets.IsWebSocketRequest)
                {
                    var webSocket = await http.WebSockets.AcceptWebSocketAsync();
                    if (webSocket != null && webSocket.State == WebSocketState.Open)
                    {
                        while (webSocket.State == WebSocketState.Open)
                        {
                            var token = CancellationToken.None;
                            var buffer = new ArraySegment<byte>(new byte[4096]);

                            var received = await webSocket.ReceiveAsync(buffer, token);

                            switch (received.MessageType)
                            {
                                case WebSocketMessageType.Text:
                                    var request = Encoding.UTF8.GetString(buffer.Array,
                                                                          buffer.Offset,
                                                                          buffer.Count);
                                    await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    await next();
                }
            });

            app.UseMvc();
        }
    }
}
