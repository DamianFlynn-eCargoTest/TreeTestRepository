using System.Collections;
using System.Collections.Generic;

namespace NodeLibrary
{
    public class NodeTransformer : INodeTransformer
    {
        Stack nodes;
        INodesHelper nodesHelper;
        Dictionary<Node, Node> nodesAndReplacements;

        public NodeTransformer(INodesHelper nodesHelper)
        {
            this.nodesHelper = nodesHelper;
        }

        public Node Transform(Node node)
        {
            nodesHelper.Nodes = (nodes = new Stack());
            nodesAndReplacements = new Dictionary<Node, Node>();
            List<Node> touchedNodes = new List<Node>();

            nodesHelper.PushToStack(node);
            while (true)
            {
                while (nodes.Count > 0 && touchedNodes.Contains((Node)nodes.Peek()))
                {
                    node = (Node)nodes.Pop();
                    nodesAndReplacements.Add(node, GetReplacementNode(node));
                }
                if (nodes.Count == 0)
                {
                    return nodesAndReplacements[node];
                }
                node = (Node)nodes.Peek();
                touchedNodes.Add(node);
                nodesHelper.PushChildrenToStack(node);
            }
        }

        private Node GetReplacementNode(Node node)
        {
            if (node is NoChildrenNode)
            {
                return node;
            }
            else if (node is SingleChildNode)
            {
                return GetReplacementNode(GetReplacementNodesViaLookup(
                    nodesHelper.GetChildren((SingleChildNode)node)), node);
            }
            else if (node is TwoChildrenNode)
            {
                return GetReplacementNode(GetReplacementNodesViaLookup(
                    nodesHelper.GetChildren((TwoChildrenNode)node)), node);
            }
            if (node is ManyChildrenNode)
            {
                return GetReplacementNode(GetReplacementNodesViaLookup(
                    nodesHelper.GetChildren((ManyChildrenNode)node)), node);
            }
            return node;
        }

        private Node GetReplacementNode(List<Node> replacementNodes, Node node)
        {
            switch (replacementNodes.Count)
            {
                case 0:
                    return new NoChildrenNode(node.Name);
                case 1:
                    return new SingleChildNode(node.Name, replacementNodes[0]);
                case 2:
                    return new TwoChildrenNode(node.Name, replacementNodes[0], replacementNodes[1]);
                default:
                    return new ManyChildrenNode(node.Name, replacementNodes.ToArray());
            }
        }

        private Node GetReplacementNodeViaLookup(Node node)
        {
            Node replacement;
            if (nodesAndReplacements.TryGetValue(node, out replacement))
            {
                return replacement;
            }
            return node;
        }

        private List<Node> GetReplacementNodesViaLookup(List<Node> children)
        {
            List<Node> replacementNodes = new List<Node>();
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                {
                    replacementNodes.Add(GetReplacementNodeViaLookup(children[i]));
                }
            }
            return replacementNodes;
        }
    }
}