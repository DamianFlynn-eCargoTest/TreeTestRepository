using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NodeLibrary
{
    public class NodeDescriber : INodeDescriber
    {
        public static string Tab = "    ";

        Stack nodes;
        INodesHelper nodesHelper;

        int level;
        private StringBuilder sb;
        bool isAppendedRightBracket;               

        public NodeDescriber(INodesHelper nodesHelper)
        {
            this.nodesHelper = nodesHelper;
        }

        public string Describe(Node node)
        {
            level = 0;
            sb = new StringBuilder();
            isAppendedRightBracket = false;

            nodesHelper.Nodes = (nodes = new Stack());
            List<Node> appendedNodes = new List<Node>();            
            int countOfNodes;

            nodesHelper.PushToStack(node);
            while (true)
            {
                // While details of next (peek) node have already been appended,
                while (nodes.Count > 0 && appendedNodes.Contains((Node)nodes.Peek()))
                {
                    // pop the node, 
                    nodes.Pop();

                    // append right bracket (signifying end of call to node constructor)
                    AppendRightBracket();                    
                }

                // If there are no nodes left in stack, 
                if (nodes.Count == 0)
                {
                    // append final line with semicolon,
                    AppendSemicolon();

                    // return description
                    return sb.ToString();
                }

                // If right bracket appended, 
                if (isAppendedRightBracket)
                {
                    // we are in between arguments, so add comma then new line
                    AppendCommaThenNewLine();
                    isAppendedRightBracket = false;
                }

                node = (Node)nodes.Peek();

                // Append line with initial details, including node class name and node name
                AppendUpToNodeName(node.GetType().Name, node.Name);
                appendedNodes.Add(node);

                // Record count of nodes, to later help to determine number of children
                countOfNodes = nodes.Count;

                // Push children to stack
                nodesHelper.PushChildrenToStack(node);

                // If node has children
                if (nodes.Count - countOfNodes > 0)
                {
                    // append comma, then new line (for next node)
                    AppendCommaThenNewLine();
                }
            }
        }

        private void AppendUpToNodeName(string className, string nodeName)
        {
            for (int i = 0; i < level; i++)
            {
                sb.Append(Tab);
            }
            sb.Append("new ");
            sb.Append(className);
            AppendLeftBracket();
            sb.Append("\"");
            sb.Append(nodeName);
            sb.Append("\"");
        }

        private void AppendLeftBracket()
        {
            sb.Append("(");
            level++;
        }

        private void AppendRightBracket()
        {
            sb.Append(")");
            isAppendedRightBracket = true;
            level--;
        }

        private void AppendCommaThenNewLine()
        {
            sb.Append(",");
            sb.Append(Environment.NewLine);
        }

        private void AppendSemicolon()
        {
            sb.Append(";");
        }
    }
}