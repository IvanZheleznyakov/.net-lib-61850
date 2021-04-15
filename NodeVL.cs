namespace lib61850net
{
    internal class NodeVL : NodeBase
    {
        public NodeVL(string Name)
            : base(Name)
        {
            Defined = false;
            Activated = false;
            Deletable = false;
        }

        public bool Deletable { get; set; }

        public bool Activated { get; set; }

        public bool Defined { get; set; }

        public NodeData urcb { get; set; }
    }
}
