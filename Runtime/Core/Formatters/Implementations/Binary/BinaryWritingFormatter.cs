using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EasyToolKit.Core.Pooling;
using EasyToolKit.Serialization.Utilities;

namespace EasyToolKit.Serialization.Formatters.Implementations
{
    /// <summary>
    /// Binary writing formatter implementation. Serializes data to a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    public sealed partial class BinaryWritingFormatter : WritingFormatterBase
    {
        private int _position;
        private int _length;
        private byte[] _buffer;
        private int _nodeDepth;
        private readonly Dictionary<Type, int> _idByType;
        private const int DefaultInitialCapacity = 1024;

        public BinaryWritingFormatter()
            : this(DefaultInitialCapacity)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWritingFormatter"/> class
        /// with the specified initial buffer capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the internal buffer in bytes.</param>
        public BinaryWritingFormatter(int initialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be positive.");

            _buffer = new byte[initialCapacity];
            _nodeDepth = 0;
            _position = 0;
            _length = 0;
            _idByType = new Dictionary<Type, int>();
        }

        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Binary;

        /// <summary>
        /// Gets the formatter options from settings, or Default if settings is not a BinaryFormatterSettings.
        /// </summary>
        private BinaryFormatterOptions Options =>
            ((BinaryFormatterSettings)Settings)?.Options ?? BinaryFormatterOptions.Default;

        /// <inheritdoc />
        public override byte[] GetBuffer() => _buffer;

        /// <inheritdoc />
        public override int GetPosition() => _position;

        /// <inheritdoc />
        public override int GetLength() => _length;

        /// <inheritdoc />
        public override byte[] ToArray()
        {
            var result = new byte[_length];
            Array.Copy(_buffer, 0, result, 0, _length);
            return result;
        }

        /// <summary>
        /// Resets the formatter for object pool reuse. Clears position and retains the internal buffer.
        /// </summary>
        public void Reset()
        {
            _position = 0;
            _length = 0;
            _nodeDepth = 0;
            _idByType.Clear();
        }

        /// <inheritdoc />
        protected override void BeginMember(string name)
        {
            WriteOptionTag(BinaryFormatterTag.MemberBegin);
            if ((Options & BinaryFormatterOptions.IncludeMemberNames) != 0)
            {
                WriteBytes(name);
            }
        }

        /// <inheritdoc />
        protected override void BeginObject(Type type)
        {
            WriteOptionTag(BinaryFormatterTag.ObjectBegin);
            if ((Options & BinaryFormatterOptions.IncludeObjectType) != 0)
            {
                // Type deduplication: use tag + id/name for different type scenarios
                if (type == null)
                {
                    WriteTag(BinaryFormatterTag.NullType);
                }
                else if (_idByType.TryGetValue(type, out var typeId))
                {
                    WriteTag(BinaryFormatterTag.TypeId);
                    WriteUInt32Optimized((uint)typeId);
                }
                else
                {
                    WriteTag(BinaryFormatterTag.TypeName);
                    WriteBytes(SerializedTypeUtility.TypeToName(type));
                    _idByType[type] = _idByType.Count;
                }
            }

            WriteUInt32Optimized((uint)_nodeDepth);
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            _nodeDepth--;
            WriteUInt32Optimized((uint)_nodeDepth);
            WriteOptionTag(BinaryFormatterTag.ObjectEnd);
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            WriteOptionTag(BinaryFormatterTag.ArrayBegin);
            WriteUInt32Optimized((uint)length);
            WriteUInt32Optimized((uint)_nodeDepth);
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            _nodeDepth--;
            WriteUInt32Optimized((uint)_nodeDepth);
            WriteOptionTag(BinaryFormatterTag.ArrayEnd);
        }

        /// <inheritdoc />
        public override void Format(ref int value)
        {
            WriteOptionTag(BinaryFormatterTag.Int32);
            // Use zigzag encoding to handle negative numbers
            uint encoded = (uint)((value << 1) ^ (value >> 31));

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint32(encoded);
            }
            else
            {
                WriteUInt32Fixed(encoded);
            }
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            WriteOptionTag(BinaryFormatterTag.Int8);
            // Use zigzag encoding to handle negative numbers
            int encoded = (value << 1) ^ (value >> 7);
            WriteByte((byte)encoded);
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            WriteOptionTag(BinaryFormatterTag.Int16);
            // Use zigzag encoding to handle negative numbers
            uint encoded = (uint)((value << 1) ^ (value >> 15));

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint32(encoded);
            }
            else
            {
                WriteUInt16Fixed((ushort)encoded);
            }
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            WriteOptionTag(BinaryFormatterTag.Int64);
            // Use zigzag encoding to handle negative numbers
            ulong encoded = ((ulong)value << 1) ^ (ulong)(value >> 63);

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint64(encoded);
            }
            else
            {
                WriteUInt64Fixed(encoded);
            }
        }

        /// <inheritdoc />
        public override void Format(ref byte value)
        {
            WriteOptionTag(BinaryFormatterTag.UInt8);
            WriteByte(value);
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            WriteOptionTag(BinaryFormatterTag.UInt16);

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint32(value);
            }
            else
            {
                WriteUInt16Fixed(value);
            }
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            WriteOptionTag(BinaryFormatterTag.UInt32);

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint32(value);
            }
            else
            {
                WriteUInt32Fixed(value);
            }
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            WriteOptionTag(BinaryFormatterTag.UInt64);

            if ((Options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint64(value);
            }
            else
            {
                WriteUInt64Fixed(value);
            }
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            WriteOptionTag(BinaryFormatterTag.Boolean);
            WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            WriteOptionTag(BinaryFormatterTag.Single);
            WriteSingle(value);
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            WriteOptionTag(BinaryFormatterTag.Double);
            WriteDouble(value);
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            WriteOptionTag(BinaryFormatterTag.String);
            WriteBytes(str);
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            WriteOptionTag(BinaryFormatterTag.ByteArray);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WriteBytes(data);
        }

        /// <inheritdoc />
        public override void Format(ref sbyte[] data)
        {
            WriteOptionTag(BinaryFormatterTag.SByteArray);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref short[] data)
        {
            WriteOptionTag(BinaryFormatterTag.Int16Array);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref int[] data)
        {
            WriteOptionTag(BinaryFormatterTag.Int32Array);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref long[] data)
        {
            WriteOptionTag(BinaryFormatterTag.Int64Array);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref ushort[] data)
        {
            WriteOptionTag(BinaryFormatterTag.UInt16Array);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref uint[] data)
        {
            WriteOptionTag(BinaryFormatterTag.UInt32Array);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref ulong[] data)
        {
            WriteOptionTag(BinaryFormatterTag.UInt64Array);
            if (data == null)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            WriteOptionTag(BinaryFormatterTag.UnityObjectRef);
            var index = RegisterReference(unityObject);
            WriteUInt32Optimized((uint)index);
        }

        /// <inheritdoc />
        public override void FormatGenericPrimitive<T>(ref T value)
        {
            WriteOptionTag(BinaryFormatterTag.UnmanagedValue);
            WritePrimitiveValue(value);
        }

        /// <inheritdoc />
        public override void FormatGenericPrimitive<T>(ref T[] data)
        {
            WriteOptionTag(BinaryFormatterTag.UnmanagedArray);
            if (data == null || data.Length == 0)
            {
                WriteUInt32Optimized(0);
                return;
            }

            WriteUInt32Optimized((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <summary>Writes a single byte to the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte value)
        {
            EnsureCapacity(1);
            _buffer[_position++] = value;
            if (_position > _length)
                _length = _position;
        }
        /// <inheritdoc />
        public override void Dispose()
        {
            _position = 0;
            _length = 0;
            _nodeDepth = 0;
            _idByType.Clear();
            Array.Clear(_buffer, 0, _buffer.Length);
            PoolUtility.ReleaseObject(this);
            base.Dispose();
        }
    }
}
