using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Formatters.Implementations
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
        public abstract SerializationFormat Type { get; }

        /// <inheritdoc />
        public DataFormatterSettings Settings
        {
            get => _settings;
            set => _settings = value;
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

        protected abstract void BeginMember(string name, bool isInArrayContext);

        protected abstract void BeginObject();

        protected abstract void EndObject();

        protected abstract void BeginArray(ref int length);

        protected abstract void EndArray();

        /// <inheritdoc />
        public abstract void Format(ref int value);

        /// <inheritdoc />
        public abstract void Format(ref sbyte value);

        /// <inheritdoc />
        public abstract void Format(ref short value);

        /// <inheritdoc />
        public abstract void Format(ref long value);

        /// <inheritdoc />
        public abstract void Format(ref byte value);

        /// <inheritdoc />
        public abstract void Format(ref ushort value);

        /// <inheritdoc />
        public abstract void Format(ref uint value);

        /// <inheritdoc />
        public abstract void Format(ref ulong value);

        /// <inheritdoc />
        public abstract void Format(ref bool value);

        /// <inheritdoc />
        public abstract void Format(ref float value);

        /// <inheritdoc />
        public abstract void Format(ref double value);

        /// <inheritdoc />
        public abstract void Format(ref string str);

        /// <inheritdoc />
        public virtual void Format(ref byte[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byte item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref sbyte[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new sbyte[length];
            for (int i = 0; i < length; i++)
            {
                sbyte item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref short[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new short[length];
            for (int i = 0; i < length; i++)
            {
                short item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref int[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new int[length];
            for (int i = 0; i < length; i++)
            {
                int item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref long[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new long[length];
            for (int i = 0; i < length; i++)
            {
                long item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref ushort[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                ushort item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref uint[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new uint[length];
            for (int i = 0; i < length; i++)
            {
                uint item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public virtual void Format(ref ulong[] data)
        {
            var length = 0;
            using var scope = this.EnterArray(ref length);
            data = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                ulong item = 0;
                Format(ref item);
                data[i] = item;
            }
        }

        /// <inheritdoc />
        public abstract void Format(ref UnityEngine.Object unityObject);

        /// <inheritdoc />
        void IDataFormatter.BeginMember(string name)
        {
            // Skip name generation if in Array context
            bool isInArrayContext = _operationStack.Count > 0 && _operationStack.Peek() == OperationType.Array;

            if (!isInArrayContext && string.IsNullOrEmpty(name))
            {
                // Generate auto-generated name for anonymous members using configured format
                string nameFormat = _settings?.AnonymousMemberNameFormat ?? "${0}";
                name = string.Format(nameFormat, _anonymousMemberId++);
            }

            BeginMember(name, isInArrayContext);
        }

        /// <inheritdoc />
        void IDataFormatter.BeginObject()
        {
            _operationStack.Push(OperationType.Object);
            BeginObject();
        }

        /// <inheritdoc />
        void IDataFormatter.EndObject()
        {
            ValidateEndOperation(OperationType.Object);
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
            ValidateEndOperation(OperationType.Array);
            EndArray();
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
                throw new InvalidOperationException(
                    $"Formatter disposed with unbalanced Begin/End operations. " +
                    $"Missing End{operation} call for the corresponding Begin{operation} operation.");
            }
        }
    }
}
