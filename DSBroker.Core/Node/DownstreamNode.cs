using System;
using System.Text;

namespace DSBroker.Node
{
    public class DownstreamNode : BrokerNode
    {
        private const string _characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        private static readonly Random _random = new Random();

        public DownstreamNode(string name, BrokerNode parent)
            : base(name, parent, "node")
        {
        }

        public string CreateDSLink(Client client)
        {
            var node = GetChild(client.Name);
            DSLinkNode dslink;
            if (node != null && node is DSLinkNode)
            {
                dslink = (DSLinkNode)node;
                if (dslink.DsId.Equals(client.DsId))
                {
                    return client.Name;
                }
            }

            string name = client.Name;

            if (node != null && HasChild(client.Name))
            {
                StringBuilder str = new StringBuilder(name);
                str.Append("-");
                str.Append(getRandomCharacter());

                while (HasChild(str.ToString()))
                {
                    str.Append(getRandomCharacter());
                }
                name = str.ToString();
            }

            dslink = new DSLinkNode(name, client.DsId, this);
            dslink.Accessible = false;
            client.Node = dslink;
            AddChild(dslink);

            return name;
        }

        private char getRandomCharacter()
        {
            return _characters[_random.Next(0, _characters.Length - 1)];
        }
    }
}
