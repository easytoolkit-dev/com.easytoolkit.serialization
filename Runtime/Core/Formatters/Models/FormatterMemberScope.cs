using System;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization
{
    /// <summary>
    /// Manages the scope of a member serialization operation, ensuring proper cleanup.
    /// </summary>
    /// <remarks>
    /// This class implements the IDisposable pattern to automatically call EndMember
    /// when the scope is exited, either through explicit disposal or the using statement.
    /// Instances are pooled and should be created using the static Create method.
    /// </remarks>
    public sealed class FormatterMemberScope : IPoolItem, IDisposable
    {
        private IDataFormatter _formatter;
        private bool _disposed;

        /// <summary>
        /// Creates a new instance of the <see cref="FormatterMemberScope"/> class from the object pool.
        /// </summary>
        /// <param name="formatter">The data formatter to manage the member scope for.</param>
        /// <param name="name">The member name, or null for unnamed members.</param>
        /// <returns>A new or reused instance of <see cref="FormatterMemberScope"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when formatter is null.</exception>
        public static FormatterMemberScope Create(IDataFormatter formatter, string name)
        {
            var scope = PoolUtility.RentObject<FormatterMemberScope>();
            scope._formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            scope._disposed = false;
            scope._formatter.BeginMember(name);
            return scope;
        }

        /// <summary>
        /// Ends the current member scope and releases the instance back to the object pool.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _formatter?.EndMember();
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
