using System;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Defines a contract for validating whether a serializer can handle a specific value type.
    /// </summary>
    public interface ISerializationTypeValidator
    {
        /// <summary>
        /// Determines whether the specified value type can be serialized.
        /// </summary>
        /// <param name="valueType">The type to validate for serialization support.</param>
        /// <returns>True if the type can be serialized; otherwise, false.</returns>
        bool CanSerialize(Type valueType);
    }
}
