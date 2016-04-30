namespace NodeLibrary
{
    public class SingleChildNode : Node
    {
        public Node Child { get; }

        public SingleChildNode(string name, Node child) : base(name)
        {
            Child = child;
        }
    }
}