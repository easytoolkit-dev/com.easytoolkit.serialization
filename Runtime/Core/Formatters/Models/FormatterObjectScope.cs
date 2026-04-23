using System;
using EasyToolkit.Core.Pooling;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolkit.Serialization.Formatters
{
    /// <summary>
    /// Manages the scope of an object serialization operation, ensuring proper cleanup.
    /// </summary>
    /// <remarks>
    /// This class implements the IDisposable pattern to automatically call EndObject
    /// when the scope is exited, either through explicit disposal or the using statement.
    /// Instances are pooled and should be created using the static Create method.
    /// </remarks>
    public sealed class FormatterObjectScope : IDisposable
    {
        private IDataFormatter _formatter;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="FormatterObjectScope"/> class from the object pool.
        /// </summary>
        /// <param name="formatter">The data formatter to manage the object scope for.</param>
        /// <returns>A new or reused instance of <see cref="FormatterObjectScope"/>.</returns>
        public static FormatterObjectScope Create(IDataFormatter formatter)
        {
            var scope = PoolUtility.RentObject<FormatterObjectScope>();
            scope._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            scope._disposed = false;
            scope._formatter.BeginObject();
            return scope;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="FormatterObjectScope"/> class from the object pool.
        /// </summary>
        /// <param name="formatter">The data formatter to manage the object scope for.</param>
        /// <param name="type">
        /// The type of the object. In Write mode, this type is written to the output.
        /// In Read mode, validates that the type in the data matches the expected type.
        /// </param>
        /// <returns>A new or reused instance of <see cref="FormatterObjectScope"/>.</returns>
        public static FormatterObjectScope Create(IDataFormatter formatter, Type type)
        {
            var scope = PoolUtility.RentObject<FormatterObjectScope>();
            scope._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            scope._disposed = false;
            scope._formatter.BeginObject(type);
            return scope;
        }

        /// <summary>
        /// Ends the current object scope and releases the instance back to the object pool.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _formatter?.EndObject();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            _formatter = null;
            _disposed = true;
            PoolUtility.ReleaseObject(this);
        }
    }
}
