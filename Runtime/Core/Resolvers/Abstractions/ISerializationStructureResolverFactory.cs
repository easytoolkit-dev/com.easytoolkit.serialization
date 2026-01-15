using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines the contract for creating structure resolvers.
    /// </summary>
    public interface ISerializationStructureResolverFactory
    {
        /// <summary>
        /// Creates a resolver for the specified type.
        /// </summary>
        /// <param name="type">The type to create a resolver for.</param>
        /// <returns>A resolver instance, or null if no suitable resolver is found.</returns>
        ISerializationStructureResolver CreateResolver(Type type);
    }
}
