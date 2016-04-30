using System.Collections;
using System.Collections.Generic;

namespace NodeLibrary
{
    public class NodesHelper : INodesHelper
    {
        private Stack nodes;
        public Stack Nodes
        {
            set
            {
                nodes = value;
            }
        }

        private void PushToStack(List<Node> nodesToPush)
        {
            for (int i = nodesToPush.Count - 1; i > -1; i--)
            {
                PushToStack(nodesToPush[i]);
            }
        }

        public void PushToStack(Node node)
        {
            if (node != null)
            {
                nodes.Push(node);
            }
        }

        public void PushChildrenToStack(Node node)
        {
            if (node is SingleChildNode)
            {
                PushToStack(GetChildren((SingleChildNode)node));
            }
            else if (node is TwoChildrenNode)
            {
                PushToStack(GetChildren((TwoChildrenNode)node));
            }
            else if (node is ManyChildrenNode)
            {
                PushToStack(GetChildren((ManyChildrenNode)node));
            }
        }

        public List<Node> GetChildren(SingleChildNode node)
        {
            return new List<Node>() { (node).Child };
        }

        public List<Node> GetChildren(TwoChildrenNode node)
        {
            return new List<Node>() { node.FirstChild, node.SecondChild };
        }

        public List<Node> GetChildren(ManyChildrenNode manyChildrenNode)
        {
            List<Node> children = new List<Node>();
            using (IEnumerator<Node> enumerator = manyChildrenNode.Children.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    children.Add(enumerator.Current);
                }
            }
            return children;
        }
    }
}
