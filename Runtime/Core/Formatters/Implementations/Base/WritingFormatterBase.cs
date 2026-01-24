using System;
using System.Collections.Generic;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Abstract base class for writing formatters.
    /// Provides common Unity object reference tracking logic and Begin/End pairing validation.
    /// </summary>
    public abstract class WritingFormatterBase : IWritingFormatter
    {
        /// <summary>Represents the type of formatting operation for tracking Begin/End pairs.</summary>
        private enum OperationType
        {
            Object,
            Array
        }

        private readonly List<UnityEngine.Object> _objectTable = new();
        private readonly Stack<OperationType> _operationStack = new();

        /// <inheritdoc />
        public abstract SerializationFormat Type { get; }

        /// <inheritdoc />
        public FormatterOperation Operation => FormatterOperation.Write;

        /// <inheritdoc />
        public IReadOnlyList<UnityEngine.Object> GetObjectTable() => _objectTable;

        /// <inheritdoc />
        public int RegisterReference(UnityEngine.Object obj)
        {
            if (obj == null) return 0;
            _objectTable.Add(obj);
            return _objectTable.Count;
        }

        /// <inheritdoc />
        public abstract byte[] GetBuffer();

        /// <inheritdoc />
        public abstract int GetPosition();

        /// <inheritdoc />
        public abstract int GetLength();

        /// <inheritdoc />
        public abstract byte[] ToArray();

        public abstract void BeginMember(string name);

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
        public abstract void Format(ref byte[] data);

        /// <inheritdoc />
        public abstract void Format(ref UnityEngine.Object unityObject);

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
