using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Interface for writing Unity object references during serialization.
    /// Separated from core formatter interface for cleaner separation of concerns.
    /// </summary>
    public interface IObjectReferenceWriter
    {
        /// <summary>
        /// Gets the registered Unity object reference table.
        /// </summary>
        IReadOnlyList<Object> GetObjectTable();

        /// <summary>
        /// Registers a Unity object and returns its index in the reference table.
        /// </summary>
        int RegisterReference(Object obj);
    }
}
