using System;

namespace EasyToolKit.Serialization.Resolvers
{
    public static class SerializationStructureResolverFactory
    {
        /// <summary>
        /// Gets a structure resolver for the specified type.
        /// </summary>
        /// <param name="type">The type to create a resolver for.</param>
        /// <returns>A structure resolver instance, or null if no suitable resolver is found.</returns>
        public static ISerializationStructureResolver GetResolver(Type type)
        {
            var resolverType = SerializationResolverUtility.GetResolverType(type);
            return resolverType != null
                ? Activator.CreateInstance(resolverType) as ISerializationStructureResolver
                : null;
        }
    }
}
