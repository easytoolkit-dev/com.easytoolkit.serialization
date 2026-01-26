using System;
using EasyToolKit.Core.Pooling;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Formatters
{
    /// <summary>
    /// Manages the scope of an object serialization operation, ensuring proper cleanup.
    /// </summary>
    /// <remarks>
    /// This class implements the IDisposable pattern to automatically call EndObject
    /// when the scope is exited, either through explicit disposal or the using statement.
    /// Instances are pooled and should be created using the static Create method.
    /// </remarks>
    public sealed class FormatterObjectScope : IPoolItem, IDisposable
    {
        private IDataFormatter _formatter;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="FormatterObjectScope"/> class from the object pool.
        /// </summary>
        /// <param name="formatter">The data formatter to manage the object scope for.</param>
        /// <param name="type">The type of the object, or null if type information is not needed.</param>
        /// <returns>A new or reused instance of <see cref="FormatterObjectScope"/>.</returns>
        public static FormatterObjectScope Create(IDataFormatter formatter, [CanBeNull] Type type = null)
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
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _formatter?.EndObject();
            _disposed = true;
            PoolUtility.ReleaseObject(this);
        }

        void IPoolItem.Rent()
        {
        }

        void IPoolItem.Release()
        {
            _formatter = null;
            _disposed = false;
        }
    }
}
