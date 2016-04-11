using AngularCSharp.ValueObjects;

namespace AngularCSharp.Processors
{
    public interface IProcessor
    {
        void ProcessNode(NodeContext nodeContext, ProcessResults results);
    }
}
