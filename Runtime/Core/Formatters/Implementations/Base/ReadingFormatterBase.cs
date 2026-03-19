using System;
using System.Collections.Generic;
using System.IO;
using EasyToolkit.Core.Pooling;
using EasyToolkit.Core.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// Abstract base class for reading formatters.
    /// Provides common Unity object reference resolution logic and Begin/End pairing validation.
    /// </summary>
    public abstract class ReadingFormatterBase : IReadingFormatter, IPoolObject
    {
        /// <summary>Represents the type of formatting operation for tracking Begin/End pairs.</summary>
        private enum OperationType
        {
            Object,
            Array
        }

        [CanBeNull] private IReadOnlyList<UnityEngine.Object> _objectTable;
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
        public FormatterOperation Operation => FormatterOperation.Read;

        public bool IsInObjectScope => _operationStack.Count > 0 && _operationStack.Peek() == OperationType.Object;
        public bool IsInArrayScope => _operationStack.Count > 0 && _operationStack.Peek() == OperationType.Array;

        protected virtual void SetObjectTable(IReadOnlyList<UnityEngine.Object> objects)
        {
            _objectTable = objects;
        }

        protected virtual UnityEngine.Object ResolveReference(int index)
        {
            if (index <= 0 || _objectTable == null || index > _objectTable.Count)
                return null;
            return _objectTable[index - 1];
        }

        protected abstract void SetBuffer(ReadOnlySpan<byte> buffer);

        protected abstract ReadOnlySpan<byte> GetBuffer();

        protected abstract int GetPosition();

        protected abstract int GetRemainingLength();

        protected abstract void BeginMember(string name);

        protected abstract void BeginObject(Type expectedType);

        protected abstract Type PeekType(Type expectedType);

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
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<bool>();
                return;
            }
            data = new bool[length];
            for (int i = 0; i < length; i++)
            {
                bool item = false;
                Format(ref item);
                data[i] = item;
            }
        }

        protected abstract void Format(ref float value);

        protected abstract void Format(ref double value);

        protected abstract void Format(ref decimal value);

        protected abstract void Format(ref string str);

        protected virtual void Format(ref byte[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<byte>();
            }
            data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byte item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref sbyte[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<sbyte>();
                return;
            }
            data = new sbyte[length];
            for (int i = 0; i < length; i++)
            {
                sbyte item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref short[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<short>();
                return;
            }
            data = new short[length];
            for (int i = 0; i < length; i++)
            {
                short item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref int[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<int>();
                return;
            }
            data = new int[length];
            for (int i = 0; i < length; i++)
            {
                int item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref long[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<long>();
                return;
            }
            data = new long[length];
            for (int i = 0; i < length; i++)
            {
                long item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref ushort[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<ushort>();
                return;
            }
            data = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                ushort item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref uint[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<uint>();
                return;
            }
            data = new uint[length];
            for (int i = 0; i < length; i++)
            {
                uint item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        protected virtual void Format(ref ulong[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            if (length == 0)
            {
                data = Array.Empty<ulong>();
                return;
            }
            data = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                ulong item = 0;
                Format(ref item);
                data[i] = item;
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
        /// - Binary: Reads a boolean value directly
        /// - JSON: Reads a null token as a marker
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

        /// <inheritdoc />
        void IDataFormatter.BeginObject()
        {
            ValidateDisposed();
            BeginObject(null);
            _operationStack.Push(OperationType.Object);
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
            ValidateEndOperationType(OperationType.Object);
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
            ValidateEndOperationType(OperationType.Array);
            EndArray();
            _operationStack.Pop();
        }

        void IDataFormatter.Format(ref int value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref sbyte value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref short value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref long value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref byte value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref ushort value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref uint value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref ulong value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref bool value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref bool[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref float value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref double value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref decimal value)
        {
            ValidateDisposed();
            Format(ref value);
        }

        void IDataFormatter.Format(ref string str)
        {
            ValidateDisposed();
            Format(ref str);
        }

        void IDataFormatter.Format(ref byte[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref sbyte[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref short[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref int[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref long[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref ushort[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref uint[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref ulong[] data)
        {
            ValidateDisposed();
            Format(ref data);
        }

        void IDataFormatter.Format(ref UnityEngine.Object unityObject)
        {
            ValidateDisposed();
            Format(ref unityObject);
        }

        void IDataFormatter.FormatGenericPrimitive<T>(ref T value)
        {
            ValidateDisposed();
            FormatGenericPrimitive(ref value);
        }

        void IDataFormatter.FormatGenericPrimitive<T>(ref T[] data)
        {
            ValidateDisposed();
            FormatGenericPrimitive(ref data);
        }

        /// <inheritdoc />
        void IDataFormatter.FormatNullable(ref bool isNull)
        {
            ValidateDisposed();
            FormatNullable(ref isNull);
        }

        /// <summary>
        /// Validates the end of an operation and checks for proper pairing.
        /// </summary>
        /// <param name="operationType">The type of operation being ended.</param>
        /// <exception cref="InvalidOperationException">Thrown when the operation type does not match the expected type.</exception>
        private void ValidateEndOperationType(OperationType operationType)
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
                Debug.LogError(
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
                throw new ObjectDisposedException(
                    GetType().ToCodeString(),
                    $"Cannot perform operation on disposed formatter. " +
                    $"The formatter has been disposed and can no longer be used. " +
                    $"Obtain a new formatter instance from the pool instead.");
            }
        }

        protected virtual void OnSettingsChanged(DataFormatterSettings settings)
        {
        }

        /// <inheritdoc />
        void IObjectReferenceReader.SetObjectTable(IReadOnlyList<UnityEngine.Object> objects)
        {
            ValidateDisposed();
            SetObjectTable(objects);
        }

        /// <inheritdoc />
        UnityEngine.Object IObjectReferenceReader.ResolveReference(int index)
        {
            ValidateDisposed();
            return ResolveReference(index);
        }

        /// <inheritdoc />
        void IReadingFormatter.SetBuffer(ReadOnlySpan<byte> buffer)
        {
            ValidateDisposed();
            SetBuffer(buffer);
        }

        /// <inheritdoc />
        ReadOnlySpan<byte> IReadingFormatter.GetBuffer()
        {
            ValidateDisposed();
            return GetBuffer();
        }

        /// <inheritdoc />
        int IReadingFormatter.GetPosition()
        {
            ValidateDisposed();
            return GetPosition();
        }

        /// <inheritdoc />
        int IReadingFormatter.GetRemainingLength()
        {
            ValidateDisposed();
            return GetRemainingLength();
        }

        /// <inheritdoc />
        Type IReadingFormatter.PeekType(Type expectedType)
        {
            ValidateDisposed();
            return PeekType(expectedType);
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
