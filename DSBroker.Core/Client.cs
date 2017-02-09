using DSBroker.Node;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace DSBroker
{
    public class Client
    {
        public string Name;
        public string DsId;
        public string Token;
        public string PublicKey;
        public bool Responder;
        public bool Requester;
        public string ReportingVersion;
        public List<string> Formats;
        public bool WebSocketCompression;
        public DSLinkNode Node;
        public KeyPair TempKey;
        public JObject HandshakeResponse = new JObject();
        public WebSocketClient WebSocketClient;
    }
}
