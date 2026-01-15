namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the type of a serialization node.
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// Atomic node without child nodes (primitive types, strings, etc.).
        /// </summary>
        Atomic,

        /// <summary>
        /// Array or collection node.
        /// </summary>
        Array,

        /// <summary>
        /// Structure node with members (classes, structs).
        /// </summary>
        Struct
    }
}
