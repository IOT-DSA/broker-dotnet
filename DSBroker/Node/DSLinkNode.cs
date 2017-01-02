namespace DSBroker.Node
{
    public class DSLinkNode : BrokerNode
    {
        public readonly string DsId;
        public readonly Client Client;

        public DSLinkNode(string name, BrokerNode parent)
            : base(name, parent, "dslink")
        {
        }
    }
}
