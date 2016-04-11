using AngularCSharp.ValueObjects;

namespace AngularCSharp.Processors
{
    public interface IProcessor
    {
        #region Public methods

        void ProcessNode(NodeContext nodeContext, ProcessResults results);

        #endregion
    }
}
