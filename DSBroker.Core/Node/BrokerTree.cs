namespace DSBroker.Node
{
    public class BrokerTree
    {
        private readonly Broker _broker;
        private readonly DownstreamNode _downstream;

        public readonly BrokerNode SuperRoot;

        public BrokerTree(Broker broker)
        {
            _broker = broker;
            SuperRoot = new BrokerNode(null, null, "dsa/broker");
            _downstream = new DownstreamNode("downstream", SuperRoot);
            SuperRoot.AddChild(_downstream);
        }

        public string InitDSLink(Client client)
        {
            return _downstream.CreateDSLink(client);
        }
    }
}
