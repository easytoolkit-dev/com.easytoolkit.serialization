using System;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Default implementation of <see cref="ISerializationStructureResolverFactory"/> that uses
    /// <see cref="SerializationResolverUtility"/> for automatic resolver discovery.
    /// </summary>
    public sealed class DefaultSerializationStructureResolverFactory : ISerializationStructureResolverFactory
    {
        /// <summary>
        /// Creates a structure resolver for the specified type.
        /// </summary>
        /// <param name="type">The type to create a resolver for.</param>
        /// <returns>A structure resolver instance, or null if no suitable resolver is found.</returns>
        public ISerializationStructureResolver CreateResolver(Type type)
        {
            var resolverType = SerializationResolverUtility.GetResolverType(type);
            return resolverType != null
                ? Activator.CreateInstance(resolverType) as ISerializationStructureResolver
                : null;
        }
    }
}
