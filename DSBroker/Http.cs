using System;
using System.Collections.Generic;

namespace DSBroker
{
    public class HttpHandler
    {
        private readonly Broker _broker;

        public HttpHandler(Broker broker)
        {
            _broker = broker;
        }

        /// <summary>
        /// Method is called when /conn is requested.
        /// </summary>
        /// <param name="body">Posted body in string format.</param>
        /// <param name="queryParameters">Map of query parameters.</param>
        /// <returns>String of returned body.</returns>
        public string PostConnEndpoint(string body, Dictionary<string, string> queryParameters)
        {
            if (!queryParameters.ContainsKey("dsId"))
            {
                throw new Exception("dsId not provided");
            }

            string dsId = queryParameters["dsId"];
            Client client;
            if (queryParameters.ContainsKey("token"))
            {
                client = Handshake.HandleHandshake(body, dsId, queryParameters["token"]);
            }
            else
            {
                client = Handshake.HandleHandshake(body, dsId);
            }

            _broker.ClientHandler.AddPendingClient(client);

            return client.HandshakeResponse.ToString();
        }
    }
}
