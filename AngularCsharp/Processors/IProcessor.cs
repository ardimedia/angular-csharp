using AngularCSharp.ValueObjects;

namespace AngularCSharp.Processors
{
    public interface IProcessor
    {
        ProcessResults ProcessNode(NodeContext nodeContext);
    }
}
