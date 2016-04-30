using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NodeLibrary
{
    public class NodeWriter : INodeWriter
    {
        private INodeDescriber nodeDescriber;

        public NodeWriter(INodeDescriber nodeDescriber)
        {
            this.nodeDescriber = nodeDescriber;
        }

        public Task WriteToFileAsync(Node node, string filePath)
        {
            return WriteToFileAsync(filePath, nodeDescriber.Describe(node));            
        }

        private async Task WriteToFileAsync(string filePath, string description)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(description);
            using (FileStream sourceStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                Task task = sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                await task;
            };
        }
    }
}