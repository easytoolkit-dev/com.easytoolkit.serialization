using System;
using System.Collections.Generic;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Abstract base class for writing formatters.
    /// Provides common Unity object reference tracking logic.
    /// </summary>
    public abstract class WritingFormatterBase : IWritingFormatter
    {
        private readonly List<UnityEngine.Object> _objectTable = new();

        /// <inheritdoc />
        public abstract FormatterType Type { get; }

        /// <inheritdoc />
        public FormatterDirection Direction => FormatterDirection.Output;

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
        public abstract void BeginMember(string name);

        /// <inheritdoc />
        public abstract void EndMember();

        /// <inheritdoc />
        public abstract void BeginObject();

        /// <inheritdoc />
        public abstract void EndObject();

        /// <inheritdoc />
        public abstract bool Format(ref int value);

        /// <inheritdoc />
        public abstract bool Format(ref Varint32 value);

        /// <inheritdoc />
        public abstract bool Format(ref SizeTag size);

        /// <inheritdoc />
        public abstract bool Format(ref bool value);

        /// <inheritdoc />
        public abstract bool Format(ref float value);

        /// <inheritdoc />
        public abstract bool Format(ref double value);

        /// <inheritdoc />
        public abstract bool Format(ref string str);

        /// <inheritdoc />
        public abstract bool Format(ref byte[] data);

        /// <inheritdoc />
        public abstract bool Format(ref UnityEngine.Object unityObject);
    }
}
