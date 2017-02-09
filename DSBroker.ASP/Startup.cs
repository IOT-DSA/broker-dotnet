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
            
            app.Use(async (http, next) => {
                Console.WriteLine(http.WebSockets.IsWebSocketRequest);
                if (http.Request.Path == "/ws" && http.WebSockets.IsWebSocketRequest)
                {
                    var queryParams = new Dictionary<string, string>();

                    foreach (KeyValuePair<string, StringValues> pair in http.Request.Query)
                    {
                        queryParams[pair.Key] = pair.Value.ToString();
                    }

                    // Pass query parameterrs into WebSocketHandler, and attempt to connect the client.
                    Client client = null;
                    try
                    {
                        client = Program.Broker.WebSocketHandler.HandleClient(queryParams);
                    }
                    catch
                    {
                        goto disconnect;
                    }

                    if (client == null)
                    {
                        goto disconnect;
                    }

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
                                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("{}")), WebSocketMessageType.Text, true, CancellationToken.None);
                                    break;
                            }
                        }
                    }

                    disconnect:
                    // We're done with the client now.
                    Program.Broker.ClientHandler.DisconnectClient(client);
                }
                else
                {
                    await next();
                }
            });
            app.UseMvc();
            app.UseWebSockets();
        }
    }
}
