using System;
using System.Collections.Generic;
using EasyToolkit.Core.Pooling;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// Abstract base class for writing formatters.
    /// Provides common Unity object reference tracking logic and Begin/End pairing validation.
    /// </summary>
    public abstract class WritingFormatterBase : IWritingFormatter, IPoolObject
    {
        /// <summary>Represents the type of formatting operation for tracking Begin/End pairs.</summary>
        private enum OperationType
        {
            Object,
            Array
        }

        private readonly List<UnityEngine.Object> _objectTable = new();
        private readonly Stack<OperationType> _operationStack = new();
        private int _anonymousMemberId;
        private DataFormatterSettings _settings;
        private bool _disposed;

        /// <inheritdoc />
        public abstract SerializationFormat FormatType { get; }

        /// <inheritdoc />
        public DataFormatterSettings Settings
        {
            get => _settings;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!ReferenceEquals(_settings, value))
                {
                    OnSettingsChanged(value);
                    _settings = value;
                }
            }
        }

        /// <inheritdoc />
        public FormatterOperation Operation => FormatterOperation.Write;

        protected virtual IReadOnlyList<UnityEngine.Object> GetObjectTable() => _objectTable;

        protected virtual int RegisterReference(UnityEngine.Object obj)
        {
            if (obj == null) return 0;
            _objectTable.Add(obj);
            return _objectTable.Count;
        }

        protected abstract byte[] GetBuffer();

        protected abstract int GetPosition();

        protected abstract int GetLength();

        protected abstract byte[] ToArray();

        protected abstract void BeginMember(string name);

        protected abstract void BeginObject(Type type);

        protected abstract void EndObject();

        protected abstract void BeginArray(ref int length);

        protected abstract void EndArray();

        protected abstract void Format(ref int value);

        protected abstract void Format(ref sbyte value);

        protected abstract void Format(ref short value);

        protected abstract void Format(ref long value);

        protected abstract void Format(ref byte value);

        protected abstract void Format(ref ushort value);

        protected abstract void Format(ref uint value);

        protected abstract void Format(ref ulong value);

        protected abstract void Format(ref bool value);

        protected virtual void Format(ref bool[] data)
        {
            var length = data.Length;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected abstract void Format(ref float value);

        protected abstract void Format(ref double value);

        protected abstract void Format(ref decimal value);

        protected abstract void Format(ref string str);

        protected virtual void Format(ref byte[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref sbyte[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref short[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref int[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref long[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref ushort[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref uint[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected virtual void Format(ref ulong[] data)
        {
            var length = data?.Length ?? 0;
            using var scope = this.EnterArray(ref length);
            for (int i = 0; i < length; i++)
            {
                var item = data[i];
                Format(ref item);
            }
        }

        protected abstract void Format(ref UnityEngine.Object unityObject);

        /// <summary>
        /// Formats the null state as a tag marker for any nullable type.
        /// </summary>
        /// <param name="isNull">Whether the value is null (true) or has a value (false).</param>
        /// <remarks>
        /// This method handles the null flag serialization for any type that can be null, including:
        /// - Nullable value types (e.g., int?, bool?, float?)
        /// - Reference types (e.g., class instances, strings)
        ///
        /// Format-specific behavior:
        /// - Binary: Writes a boolean value directly
        /// - JSON: Writes a null token as a marker
        /// </remarks>
        protected abstract void FormatNullable(ref bool isNull);

        protected virtual void FormatGenericPrimitive<T>(ref T value) where T : unmanaged
        {
            throw new NotSupportedException(
                $"FormatGenericPrimitive is not supported in format type '{FormatType}'. " +
                $"Use the typed Format methods (e.g., Format(ref int value)) instead.");
        }

        protected virtual void FormatGenericPrimitive<T>(ref T[] data) where T : unmanaged
        {
            throw new NotSupportedException(
                $"FormatGenericPrimitive array is not supported in format type '{FormatType}'. " +
                $"Use the typed Format methods (e.g., Format(ref int value)) instead.");
        }

        protected virtual void Dispose()
        {
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            Dispose();
            _anonymousMemberId = 0;
            if (_operationStack.Count > 0)
            {
                var operation = _operationStack.Peek();
                _operationStack.Clear();
                throw new InvalidOperationException(
                    $"Formatter disposed with unbalanced Begin/End operations. " +
                    $"Missing End{operation} call for the corresponding Begin{operation} operation.");
            }
            _operationStack.Clear();
            _disposed = true;
        }

        protected void ValidateDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("");
            }
        }

        protected virtual void OnSettingsChanged(DataFormatterSettings settings)
        {
        }

        /// <inheritdoc />
        void IDataFormatter.BeginMember(string name)
        {
            ValidateDisposed();
            if (_operationStack.Count > 0 && _operationStack.Peek() != OperationType.Object)
            {
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                // Generate auto-generated name for anonymous members using configured format
                string nameFormat = _settings?.AnonymousMemberNameFormat ?? "${0}";
                name = string.Format(nameFormat, _anonymousMemberId++);
            }

            BeginMember(name);
        }


        /// <summary>
        /// Validates the end of an operation and checks for proper pairing.
        /// </summary>
        /// <param name="operationType">The type of operation being ended.</param>
        /// <exception cref="InvalidOperationException">Thrown when the operation type does not match the expected type.</exception>
        private void ValidateEndOperation(OperationType operationType)
        {
            if (_operationStack.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot end {operationType} operation: no matching Begin operation found. The operation stack is empty.");
            }

            var expectedOperation = _operationStack.Peek();
            if (expectedOperation != operationType)
            {
                throw new InvalidOperationException(
                    $"Unbalanced Begin/End operations. Expected End{expectedOperation}, but called End{operationType}.");
            }
        }

        /// <inheritdoc />
        void IDataFormatter.BeginObject(Type type)
        {
            ValidateDisposed();
            BeginObject(type);
            _operationStack.Push(OperationType.Object);
        }

        /// <inheritdoc />
        void IDataFormatter.EndObject()
        {
            ValidateDisposed();
            ValidateEndOperation(OperationType.Object);
            EndObject();
            _operationStack.Pop();
        }

        /// <inheritdoc />
        void IDataFormatter.BeginArray(ref int length)
        {
            ValidateDisposed();
            BeginArray(ref length);
            _operationStack.Push(OperationType.Array);
        }

        /// <inheritdoc />
        void IDataFormatter.EndArray()
        {
            ValidateDisposed();
            ValidateEndOperation(OperationType.Array);
            EndArray();
            _operationStack.Pop();
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref int value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref sbyte value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref short value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref long value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref byte value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref ushort value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref uint value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref ulong value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref bool value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref bool[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref float value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref double value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref decimal value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref string str)
        {
            ValidateDisposed();
            Format(ref str);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref byte[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref sbyte[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref short[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref int[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref long[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref ushort[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref uint[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref ulong[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.Format(ref UnityEngine.Object unityObject)
        {
            ValidateDisposed();
            Format(ref unityObject);
        }

        /// <inheritdoc />
        void IDataFormatter.FormatNullable(ref bool isNull)
        {
            ValidateDisposed();
            FormatNullable(ref isNull);
        }

        /// <inheritdoc />
        void IDataFormatter.FormatGenericPrimitive<T>(ref T value)
        {
            ValidateDisposed();
            FormatGenericPrimitive(ref value);
        }

        /// <inheritdoc />
        void IDataFormatter.FormatGenericPrimitive<T>(ref T[] data)
        {
            ValidateDisposed();
            FormatGenericPrimitive(ref data);
        }

        /// <inheritdoc />
        IReadOnlyList<UnityEngine.Object> IObjectReferenceWriter.GetObjectTable()
        {
            ValidateDisposed();
            return GetObjectTable();
        }

        /// <inheritdoc />
        int IObjectReferenceWriter.RegisterReference(UnityEngine.Object obj)
        {
            ValidateDisposed();
            return RegisterReference(obj);
        }

        /// <inheritdoc />
        byte[] IWritingFormatter.GetBuffer()
        {
            ValidateDisposed();
            return GetBuffer();
        }

        /// <inheritdoc />
        int IWritingFormatter.GetPosition()
        {
            ValidateDisposed();
            return GetPosition();
        }

        /// <inheritdoc />
        int IWritingFormatter.GetLength()
        {
            ValidateDisposed();
            return GetLength();
        }

        /// <inheritdoc />
        byte[] IWritingFormatter.ToArray()
        {
            ValidateDisposed();
            return ToArray();
        }

        void IPoolObject.OnRent()
        {
            _disposed = false;
        }

        void IPoolObject.OnRelease()
        {
        }
    }
}
