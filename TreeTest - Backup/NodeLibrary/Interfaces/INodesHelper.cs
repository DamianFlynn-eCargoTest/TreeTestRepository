using System.Collections;
using System.Collections.Generic;

namespace NodeLibrary
{
    public interface INodesHelper
    {
        Stack Nodes { set; }
        void PushToStack(Node node);
        void PushChildrenToStack(Node node);
        List<Node> GetChildren(SingleChildNode node);
        List<Node> GetChildren(TwoChildrenNode node);
        List<Node> GetChildren(ManyChildrenNode manyChildrenNode);
    }
}