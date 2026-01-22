using System;
using System.Reflection;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the contract for building serialization nodes.
    /// </summary>
    public interface ISerializationNodeFactory
    {
        /// <summary>
        /// Builds a root node for the specified type.
        /// </summary>
        /// <returns>A root serialization node.</returns>
        ISerializationNode BuildNode(Type valueType);

        /// <summary>
        /// Builds a child node for the specified type with additional context.
        /// </summary>
        /// <param name="memberInfo">The member information (required for child nodes).</param>
        /// <param name="index">The index in parent.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>A child serialization node.</returns>
        ISerializationNode BuildNode(
            Type valueType,
            MemberInfo memberInfo,
            int index,
            ISerializationNode parent);
    }
}
