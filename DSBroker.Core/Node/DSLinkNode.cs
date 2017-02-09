namespace DSBroker.Node
{
    public class DSLinkNode : BrokerNode
    {
        public readonly string DsId;

        public DSLinkNode(string name, string dsId, BrokerNode parent)
            : base(name, parent, "dslink")
        {
            DsId = dsId;
        }
    }
}
