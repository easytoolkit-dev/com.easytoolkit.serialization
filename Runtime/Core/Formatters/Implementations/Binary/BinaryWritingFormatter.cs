using System;
using System.Runtime.CompilerServices;
using EasyToolKit.Core.Pooling;

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
        }

        /// <inheritdoc />
        public override SerializationFormat FormatType => SerializationFormat.Binary;

        /// <summary>
        /// Gets the formatter options from settings, or Default if settings is not a BinaryFormatterSettings.
        /// </summary>
        private BinaryFormatterOptions Options =>
            ((BinaryFormatterSettings)Settings)?.Options ?? BinaryFormatterOptions.Default;

        /// <summary>
        /// Writes a tag when IncludeTypeTags option is enabled.
        /// </summary>
        /// <param name="tag">The tag value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteTypeTag(BinaryFormatterTag tag)
        {
            if ((Options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                WriteByte((byte)tag);
            }
        }

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
        }

        /// <inheritdoc />
        protected override void BeginMember(string name)
        {
            if ((Options & BinaryFormatterOptions.IncludeMemberNames) != 0)
            {
                WriteByte((byte)BinaryFormatterTag.MemberBegin);
                WriteBytes(name);
            }
            // When disabled, members are identified by position only
        }

        /// <inheritdoc />
        protected override void BeginObject()
        {
            WriteByte((byte)BinaryFormatterTag.ObjectBegin);
            WriteVarint32((uint)_nodeDepth);
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            _nodeDepth--;
            WriteVarint32((uint)_nodeDepth);
            WriteByte((byte)BinaryFormatterTag.ObjectEnd);
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            WriteByte((byte)BinaryFormatterTag.ArrayBegin);
            WriteVarint32((uint)length);
            WriteVarint32((uint)_nodeDepth);
            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            _nodeDepth--;
            WriteVarint32((uint)_nodeDepth);
            WriteByte((byte)BinaryFormatterTag.ArrayEnd);
        }

        /// <inheritdoc />
        public override void Format(ref int value)
        {
            WriteTypeTag(BinaryFormatterTag.Int32);
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
            WriteTypeTag(BinaryFormatterTag.Int8);
            // Use zigzag encoding to handle negative numbers
            int encoded = (value << 1) ^ (value >> 7);
            WriteByte((byte)encoded);
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            WriteTypeTag(BinaryFormatterTag.Int16);
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
            WriteTypeTag(BinaryFormatterTag.Int64);
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
            WriteTypeTag(BinaryFormatterTag.UInt8);
            WriteByte(value);
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            WriteTypeTag(BinaryFormatterTag.UInt16);

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
            WriteTypeTag(BinaryFormatterTag.UInt32);

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
            WriteTypeTag(BinaryFormatterTag.UInt64);

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
            WriteTypeTag(BinaryFormatterTag.Boolean);
            WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            WriteTypeTag(BinaryFormatterTag.Single);
            WriteSingle(value);
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            WriteTypeTag(BinaryFormatterTag.Double);
            WriteDouble(value);
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            WriteTypeTag(BinaryFormatterTag.String);
            WriteBytes(str);
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            WriteTypeTag(BinaryFormatterTag.ByteArray);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WriteBytes(data);
        }

        /// <inheritdoc />
        public override void Format(ref sbyte[] data)
        {
            WriteTypeTag(BinaryFormatterTag.SByteArray);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref short[] data)
        {
            WriteTypeTag(BinaryFormatterTag.Int16Array);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref int[] data)
        {
            WriteTypeTag(BinaryFormatterTag.Int32Array);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref long[] data)
        {
            WriteTypeTag(BinaryFormatterTag.Int64Array);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref ushort[] data)
        {
            WriteTypeTag(BinaryFormatterTag.UInt16Array);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref uint[] data)
        {
            WriteTypeTag(BinaryFormatterTag.UInt32Array);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref ulong[] data)
        {
            WriteTypeTag(BinaryFormatterTag.UInt64Array);
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WritePrimitiveArray(data);
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            WriteTypeTag(BinaryFormatterTag.UnityObjectRef);
            var index = RegisterReference(unityObject);
            WriteVarint32((uint)index);
        }

        /// <inheritdoc />
        public override void FormatGenericPrimitive<T>(ref T value)
        {
            WriteTypeTag(BinaryFormatterTag.UnmanagedValue);
            WritePrimitiveValue(value);
        }

        /// <inheritdoc />
        public override void FormatGenericPrimitive<T>(ref T[] data)
        {
            WriteTypeTag(BinaryFormatterTag.UnmanagedArray);
            if (data == null || data.Length == 0)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
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
            Array.Clear(_buffer, 0, _buffer.Length);
            PoolUtility.ReleaseObject(this);
            base.Dispose();
        }
    }
}
