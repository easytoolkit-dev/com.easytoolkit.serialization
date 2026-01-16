using System;
using System.Reflection;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the contract for building serialization nodes.
    /// </summary>
    public interface ISerializationNodeBuilder
    {
        /// <summary>
        /// Builds a root node for the specified type.
        /// </summary>
        /// <param name="type">The type to build a root node for.</param>
        /// <returns>A root serialization node.</returns>
        IStructSerializationNode BuildNode(Type type);

        /// <summary>
        /// Builds a child node for the specified type with additional context.
        /// </summary>
        /// <param name="type">The type to build a child node for.</param>
        /// <param name="memberInfo">The member information (required for child nodes).</param>
        /// <param name="index">The index in parent.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>A child serialization node.</returns>
        ISerializationNode BuildNode(
            Type type,
            MemberInfo memberInfo,
            int index,
            ISerializationNode parent);
    }
}
