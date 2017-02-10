using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace DSBroker.ASP.Controllers
{
    [Route("ws")]
    public class WsController : Controller
    {
        [HttpGet]
        public async void WsEndpoint()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var queryParams = new Dictionary<string, string>();

                foreach (KeyValuePair<string, StringValues> pair in HttpContext.Request.Query)
                {
                    queryParams[pair.Key] = pair.Value.ToString();
                }

                // Pass query parameters into WebSocketHandler, and attempt to connect the client.
                Client client = null;
                try
                {
                    client = Program.Broker.WebSocketHandler.HandleClient(queryParams);
                }
                catch
                {
                    return;
                }

                if (client == null)
                {
                    return;
                }

                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
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
                    // We're done with the client now.
                    Program.Broker.ClientHandler.DisconnectClient(client);
                }
            }
        }
    }
}
