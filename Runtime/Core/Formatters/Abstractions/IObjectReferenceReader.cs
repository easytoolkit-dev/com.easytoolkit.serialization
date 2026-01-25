using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolKit.Serialization.Formatters
{
    /// <summary>
    /// Interface for reading Unity object references during deserialization.
    /// Separated from core formatter interface for cleaner separation of concerns.
    /// </summary>
    public interface IObjectReferenceReader
    {
        /// <summary>
        /// Sets the object reference table for resolving Unity object references.
        /// </summary>
        void SetObjectTable([CanBeNull] IReadOnlyList<Object> objects);

        /// <summary>
        /// Resolves a Unity object reference from its index in the object table.
        /// </summary>
        Object ResolveReference(int index);
    }
}
