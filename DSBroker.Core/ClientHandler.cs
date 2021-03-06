﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DSBroker
{
    // TODO: Make thread safe.
    public class ClientHandler
    {
        Dictionary<string, Client> ConnectedClients = new Dictionary<string, Client>();
        Dictionary<string, ClientTimeout> PendingClients = new Dictionary<string, ClientTimeout>();

        public ClientHandler()
        {
        }

        public void AddPendingClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            PendingClients.Add(client.DsId, new ClientTimeout(this, 30000, client));
        }

        public void ConnectPendingClient(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException("client");
            }

            // TODO: If another client is running, close their connection to make room for this one.

            PendingClients[client.DsId].CancelTimeout();
            PendingClients.Remove(client.DsId);

            ConnectedClients.Add(client.DsId, client);
        }

        public void DisconnectClient(Client client)
        {
            // TODO: Close out client connection.

            ConnectedClients.Remove(client.DsId);
        }

        private class ClientTimeout
        {
            public readonly Task TimeoutTask;
            private readonly CancellationTokenSource _tokenSource;
            public readonly Client Client;

            public ClientTimeout(ClientHandler clientHandler, int timeout, Client client)
            {
                _tokenSource = new CancellationTokenSource();
                TimeoutTask = Task.Delay(timeout, _tokenSource.Token);
                Task.Run(() =>
                {
                    Task.Delay(timeout);
                    if (!_tokenSource.IsCancellationRequested)
                    {
                        clientHandler.PendingClients.Remove(client.DsId);
                    }
                });
            }

            public void CancelTimeout()
            {
                _tokenSource.Cancel();
            }
        }
    }
}
