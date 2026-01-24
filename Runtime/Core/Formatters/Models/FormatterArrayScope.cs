using System;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Manages the scope of an array serialization operation, ensuring proper cleanup.
    /// </summary>
    /// <remarks>
    /// This class implements the IDisposable pattern to automatically call EndArray
    /// when the scope is exited, either through explicit disposal or the using statement.
    /// Instances are pooled and should be created using the static Create method.
    /// </remarks>
    public sealed class FormatterArrayScope : IPoolItem, IDisposable
    {
        private IDataFormatter _formatter;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="FormatterArrayScope"/> class from the object pool.
        /// </summary>
        /// <param name="formatter">The data formatter to manage the array scope for.</param>
        /// <param name="length">The array length to read or write.</param>
        /// <returns>A new or reused instance of <see cref="FormatterArrayScope"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when formatter is null.</exception>
        public static FormatterArrayScope Create(IDataFormatter formatter, ref int length)
        {
            var scope = PoolUtility.RentObject<FormatterArrayScope>();
            scope._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            scope._disposed = false;
            scope._formatter.BeginArray(ref length);
            return scope;
        }

        /// <summary>
        /// Ends the current array scope and releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _formatter?.EndArray();
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
