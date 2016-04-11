using System;

namespace AngularCSharp.Exceptions
{
    /// <summary>
    /// Internal expception, is thrown when specified property is not found in model
    /// </summary>
    [Serializable]
    public class ValueNotFoundException : Exception
    {
    }
}
