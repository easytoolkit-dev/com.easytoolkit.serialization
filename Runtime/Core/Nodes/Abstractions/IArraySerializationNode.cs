namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Represents an array or collection node.
    /// </summary>
    public interface IArraySerializationNode : ISerializationNode
    {
        /// <summary>
        /// Gets the array rank (dimension count).
        /// </summary>
        int Rank { get; }
    }
}
