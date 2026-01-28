using System;
using System.Collections.Generic;
using System.IO;
using EasyToolkit.Core.Pooling;
using JetBrains.Annotations;

namespace EasyToolkit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// Abstract base class for reading formatters.
    /// Provides common Unity object reference resolution logic and Begin/End pairing validation.
    /// </summary>
    public abstract class ReadingFormatterBase : IReadingFormatter
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

        /// <inheritdoc />
        public void SetObjectTable(IReadOnlyList<UnityEngine.Object> objects)
        {
            _objectTable = objects;
        }

        /// <inheritdoc />
        public UnityEngine.Object ResolveReference(int index)
        {
            if (index <= 0 || _objectTable == null || index > _objectTable.Count)
                return null;
            return _objectTable[index - 1];
        }

        /// <inheritdoc />
        public abstract void SetBuffer(ReadOnlySpan<byte> buffer);

        /// <inheritdoc />
        public abstract ReadOnlySpan<byte> GetBuffer();

        /// <inheritdoc />
        public abstract int GetPosition();

        /// <inheritdoc />
        public abstract int GetRemainingLength();

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

        protected abstract void Format(ref float value);

        protected abstract void Format(ref double value);

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

        protected virtual void FormatGenericPrimitive<T>(ref T value) where T : unmanaged
        {
            throw new NotSupportedException(
                $"FormatGenericPrimitive is only supported in Binary format mode. " +
                $"Current format: '{FormatType}'. " +
                $"Use the typed Format methods (e.g., Format(ref int value)) for non-Binary formatters.");
        }

        protected virtual void FormatGenericPrimitive<T>(ref T[] data) where T : unmanaged
        {
            throw new NotSupportedException(
                $"FormatGenericPrimitive array is only supported in Binary format mode. " +
                $"Current format: '{FormatType}'. " +
                $"Use the typed Format methods (e.g., Format(ref int[] data)) for non-Binary formatters.");
        }

        /// <inheritdoc />
        void IDataFormatter.BeginMember(string name)
        {
            if (_operationStack.Count == 0 || _operationStack.Peek() != OperationType.Object)
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
        void IDataFormatter.BeginObject(Type type)
        {
            _operationStack.Push(OperationType.Object);
            BeginObject(type);
        }

        /// <inheritdoc />
        void IDataFormatter.EndObject()
        {
            PopAndValidateEndOperation(OperationType.Object);
            EndObject();
        }

        /// <inheritdoc />
        void IDataFormatter.BeginArray(ref int length)
        {
            _operationStack.Push(OperationType.Array);
            BeginArray(ref length);
        }

        /// <inheritdoc />
        void IDataFormatter.EndArray()
        {
            PopAndValidateEndOperation(OperationType.Array);
            EndArray();
        }

        void IDataFormatter.Format(ref int value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref sbyte value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref short value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref long value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref byte value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref ushort value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref uint value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref ulong value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref bool value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref float value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref double value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            Format(ref value);
        }

        void IDataFormatter.Format(ref string str)
        {
            if (!ValidateStreamBeforeRead(ref str))
            {
                return;
            }
            Format(ref str);
        }

        void IDataFormatter.Format(ref byte[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref sbyte[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref short[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref int[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref long[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref ushort[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref uint[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref ulong[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            Format(ref data);
        }

        void IDataFormatter.Format(ref UnityEngine.Object unityObject)
        {
            if (!ValidateStreamBeforeRead(ref unityObject))
            {
                return;
            }
            Format(ref unityObject);
        }

        void IDataFormatter.FormatGenericPrimitive<T>(ref T value)
        {
            if (!ValidateStreamBeforeRead(ref value))
            {
                return;
            }
            FormatGenericPrimitive(ref value);
        }

        void IDataFormatter.FormatGenericPrimitive<T>(ref T[] data)
        {
            if (!ValidateStreamBeforeRead(ref data))
            {
                return;
            }
            FormatGenericPrimitive(ref data);
        }

        /// <summary>
        /// Validates the stream state before reading a value.
        /// Sets the value to default and returns false if the stream has ended and ReturnDefaultOnStreamEnd is enabled.
        /// Throws an exception if the stream has ended and ReturnDefaultOnStreamEnd is disabled.
        /// </summary>
        /// <typeparam name="T">The type of value to read.</typeparam>
        /// <param name="value">The value to be read (output parameter).</param>
        /// <returns>True if the caller should proceed with reading; false if default value was set.</returns>
        /// <exception cref="EndOfStreamException">Thrown when the stream has ended and ReturnDefaultOnStreamEnd is false.</exception>
        private bool ValidateStreamBeforeRead<T>(ref T value)
        {
            if (GetRemainingLength() == 0)
            {
                if (_settings.ReturnDefaultOnStreamEnd)
                {
                    value = default;
                    return false;
                }
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");
            }
            return true;
        }

        /// <summary>
        /// Validates the end of an operation and checks for proper pairing.
        /// </summary>
        /// <param name="operationType">The type of operation being ended.</param>
        /// <exception cref="InvalidOperationException">Thrown when the operation type does not match the expected type.</exception>
        private void PopAndValidateEndOperation(OperationType operationType)
        {
            if (_operationStack.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Cannot end {operationType} operation: no matching Begin operation found. The operation stack is empty.");
            }

            var expectedOperation = _operationStack.Pop();
            if (expectedOperation != operationType)
            {
                throw new InvalidOperationException(
                    $"Unbalanced Begin/End operations. Expected End{expectedOperation}, but called End{operationType}.");
            }
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
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
        }

        protected virtual void OnSettingsChanged(DataFormatterSettings settings)
        {
        }
    }
}
