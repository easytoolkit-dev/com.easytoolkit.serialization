using System;

namespace EasyToolkit.Serialization.Resolvers
{
    /// <summary>
    /// Defines the base contract for serialization resolvers.
    /// </summary>
    public interface ISerializationResolver
    {
        /// <summary>
        /// Determines whether this resolver can handle the specified type.
        /// </summary>
        /// <param name="valueType">The type to check.</param>
        /// <returns>true if this resolver can handle the type; otherwise, false.</returns>
        bool CanResolve(Type valueType);
    }
}
