using AngularCSharp.ValueObjects;

namespace AngularCSharp.Processors
{
    /// <summary>
    /// Processor interface, any processor must implement this.
    /// </summary>
    public interface IProcessor
    {
        #region Public methods

        /// <summary>
        /// Process a node with this processor, this is called for each node in template DOM tree.
        /// </summary>
        /// <param name="nodeContext">Contains the node context and some dependencies</param>
        /// <param name="results">Contains results which was prepared from TemplateEngine and should be changed by processor instances</param>
        void ProcessNode(NodeContext nodeContext, ProcessResults results);

        #endregion
    }
}
