using System;
using System.Collections.Generic;

namespace DSBroker
{
    public class WebSocketHandler
    {
        private readonly Broker _broker;

        public WebSocketHandler(Broker broker)
        {
            _broker = broker;
        }

        public Client HandleClient(Dictionary<string, string> queryParams)
        {
            if (!queryParams.ContainsKey("dsId"))
            {
                throw new ArgumentNullException("dsId");
            }
            if (!queryParams.ContainsKey("auth"))
            {
                throw new ArgumentNullException("auth");
            }

            var dsId = queryParams["dsId"];
            var auth = queryParams["auth"];

            if (!_broker.ClientHandler.PendingClients.ContainsKey(dsId))
            {
                return null;
            }

            // TODO: Check auth!!!

            var client = _broker.ClientHandler.PendingClients[dsId].Client;
            _broker.ClientHandler.ConnectPendingClient(client);
            var wsc = new WebSocketClient(client);
            client.WebSocketClient = wsc;

            return client;
        }
    }

    public class WebSocketClient
    {
        private Client _client;

        public WebSocketClient(Client client)
        {
            _client = client;
        }
    }
}
