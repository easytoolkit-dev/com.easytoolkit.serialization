using System;
using EasyToolkit.Serialization.Processors;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Resolvers
{
    /// <summary>
    /// Defines the contract for resolving serialization structure of types.
    /// </summary>
    public interface ISerializationStructureResolver : ISerializationResolver
    {
        /// <summary>
        /// Resolves the serialization structure of the specified type using the specified context.
        /// </summary>
        /// <param name="valueType">The type to resolve.</param>
        /// <param name="context">The serialization context providing reflection settings and processor cache.</param>
        /// <param name="parent">The parent processor in the serialization hierarchy.</param>
        /// <returns>An array of member definitions. Returns empty array for atomic types.</returns>
        SerializationMemberDefinition[] Resolve(Type valueType, SerializationContext context, [CanBeNull] ISerializationProcessor parent);
    }
}
