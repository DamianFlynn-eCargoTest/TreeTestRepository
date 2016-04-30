using System.Threading.Tasks;

namespace NodeLibrary
{
    public interface INodeWriter
    {
        Task WriteToFileAsync(Node node, string filePath);
    }
}