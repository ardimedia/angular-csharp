using AngularCsharp.ValueObjects;

namespace AngularCsharp.Processors
{
    public interface IProcessor
    {
        ProcessResults ProcessNode(NodeContext nodeContext);
    }
}
