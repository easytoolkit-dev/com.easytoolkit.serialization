using System;
using System.Reflection;
using EasyToolKit.Core.Reflection;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Represents a node in the serialized structure hierarchy.
    /// </summary>
    public interface ISerializationNode
    {
        /// <summary>
        /// Gets the member definition containing metadata about this node.
        /// </summary>
        SerializationMemberDefinition Definition { get; }

        /// <summary>
        /// Gets the node type.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        /// Gets the value type.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Gets the serializer associated with this node.
        /// </summary>
        IEasySerializer Serializer { get; }

        /// <summary>
        /// Gets the value getter delegate.
        /// </summary>
        InstanceGetter ValueGetter { get; }

        /// <summary>
        /// Gets the value setter delegate.
        /// </summary>
        InstanceSetter ValueSetter { get; }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        ISerializationNode Parent { get; }

        /// <summary>
        /// Gets the index of this node in its parent.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the full path of the node.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Determines whether this node is a child of the specified node.
        /// </summary>
        bool IsChildOf(ISerializationNode node);

        void Process(string name, ref object value, IDataFormatter formatter);
    }
}
