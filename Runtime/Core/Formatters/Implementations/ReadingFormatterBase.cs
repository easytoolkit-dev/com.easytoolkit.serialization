using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Abstract base class for reading formatters.
    /// Provides common Unity object reference resolution logic.
    /// </summary>
    public abstract class ReadingFormatterBase : IReadingFormatter
    {
        [CanBeNull] private IReadOnlyList<UnityEngine.Object> _objectTable;

        /// <inheritdoc />
        public abstract SerializationFormat Type { get; }

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
        public abstract void BeginMember(string name);

        /// <inheritdoc />
        public abstract void EndMember();

        /// <inheritdoc />
        public abstract void BeginObject();

        /// <inheritdoc />
        public abstract void EndObject();

        /// <inheritdoc />
        public abstract void Format(ref int value);

        /// <inheritdoc />
        public abstract void Format(ref Varint32 value);

        /// <inheritdoc />
        public abstract void Format(ref SizeTag size);

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
    }
}
