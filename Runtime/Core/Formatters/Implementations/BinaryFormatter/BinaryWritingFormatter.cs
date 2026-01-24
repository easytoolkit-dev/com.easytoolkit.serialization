using System;
using System.Runtime.CompilerServices;
using System.Text;
using EasyToolKit.Core.Textual;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Binary writing formatter implementation. Serializes data to a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    public sealed class BinaryWritingFormatter : WritingFormatterBase
    {
        private byte[] _buffer;
        private int _nodeDepth;
        private const int DefaultInitialCapacity = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryWritingFormatter"/> class
        /// with the specified initial buffer capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the internal buffer in bytes.</param>
        public BinaryWritingFormatter(int initialCapacity = DefaultInitialCapacity)
        {
            if (initialCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be positive.");

            _buffer = new byte[initialCapacity];
            _nodeDepth = 0;
            _position = 0;
            _length = 0;
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Binary;

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
        public override void BeginMember(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                WriteByte((byte)BinaryFormatterTag.MemberBegin);
            }
            else
            {
                WriteByte((byte)BinaryFormatterTag.NamedMemberBegin);
                WriteBytes(name);
            }
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
            // Use zigzag encoding to handle negative numbers
            uint encoded = (uint)((value << 1) ^ (value >> 31));
            WriteVarint32(encoded);
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            // Use zigzag encoding to handle negative numbers
            int encoded = (value << 1) ^ (value >> 7);
            WriteByte((byte)encoded);
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            // Use zigzag encoding to handle negative numbers
            int encoded = (value << 1) ^ (value >> 15);
            WriteVarint32((uint)encoded);
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            // Use zigzag encoding to handle negative numbers
            ulong encoded = ((ulong)value << 1) ^ (ulong)(value >> 63);
            WriteVarint64(encoded);
        }

        /// <inheritdoc />
        public override void Format(ref byte value)
        {
            WriteByte(value);
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            WriteVarint32(value);
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            WriteVarint32(value);
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            WriteVarint64(value);
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            WriteByte(value ? (byte)1 : (byte)0);
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            WriteSingle(value);
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            WriteDouble(value);
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            WriteBytes(str);
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            if (data == null)
            {
                WriteVarint32(0);
                return;
            }

            WriteVarint32((uint)data.Length);
            WriteBytes(data);
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            var index = RegisterReference(unityObject);
            WriteVarint32((uint)index);
        }

        /// <summary>Ensures the buffer has enough capacity for the specified number of bytes.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int required)
        {
            if (_position + required <= _buffer.Length)
                return;

            // Exponential growth: double capacity until sufficient
            int newCapacity = Math.Max(_buffer.Length * 2, _position + required);
            Array.Resize(ref _buffer, newCapacity);
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

        /// <summary>Writes a byte array to the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            EnsureCapacity(data.Length);
            fixed (byte* srcPtr = data)
            fixed (byte* destPtr = &_buffer[_position])
            {
                Buffer.MemoryCopy(srcPtr, destPtr, data.Length, data.Length);
            }
            _position += data.Length;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>Writes a string as UTF-8 bytes to the buffer with a length prefix.</summary>
        private unsafe void WriteBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                WriteVarint32(0);
                return;
            }

            // Use stackalloc for small strings to avoid heap allocations
            int maxBytes = Encoding.UTF8.GetMaxByteCount(str.Length);
            if (maxBytes <= 256)
            {
                // Fast path for small strings using unsafe pointers
                byte* buffer = stackalloc byte[maxBytes];
                fixed (char* charPtr = str)
                {
                    int byteCount = Encoding.UTF8.GetBytes(charPtr, str.Length, buffer, maxBytes);
                    WriteVarint32((uint)byteCount);
                    EnsureCapacity(byteCount);
                    fixed (byte* destPtr = &_buffer[_position])
                    {
                        Buffer.MemoryCopy(buffer, destPtr, byteCount, byteCount);
                    }
                    _position += byteCount;
                    if (_position > _length)
                        _length = _position;
                }
            }
            else
            {
                // Fallback for large strings
                fixed (char* charPtr = str)
                {
                    int byteCount = Encoding.UTF8.GetByteCount(charPtr, str.Length);
                    WriteVarint32((uint)byteCount);
                    EnsureCapacity(byteCount);
                    fixed (byte* destPtr = &_buffer[_position])
                    {
                        Encoding.UTF8.GetBytes(charPtr, str.Length, destPtr, byteCount);
                    }
                    _position += byteCount;
                    if (_position > _length)
                        _length = _position;
                }
            }
        }

        /// <summary>Writes a 32-bit unsigned integer using variable-length encoding.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteVarint32(uint value)
        {
            // Stackalloc for unrolled varint (max 5 bytes)
            byte* buffer = stackalloc byte[5];
            int index = 0;

            // Special case for zero
            if (value == 0)
            {
                EnsureCapacity(1);
                _buffer[_position++] = 0;
                if (_position > _length)
                    _length = _position;
                return;
            }

            while (value > 0x7F)
            {
                buffer[index++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }
            buffer[index] = (byte)value;

            int count = index + 1;
            EnsureCapacity(count);
            fixed (byte* dest = &_buffer[_position])
            {
                Buffer.MemoryCopy(buffer, dest, count, count);
            }
            _position += count;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>Writes a 64-bit unsigned integer using variable-length encoding.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteVarint64(ulong value)
        {
            // Stackalloc for unrolled varint (max 10 bytes)
            byte* buffer = stackalloc byte[10];
            int index = 0;

            // Special case for zero
            if (value == 0)
            {
                EnsureCapacity(1);
                _buffer[_position++] = 0;
                if (_position > _length)
                    _length = _position;
                return;
            }

            while (value > 0x7F)
            {
                buffer[index++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }
            buffer[index] = (byte)value;

            int count = index + 1;
            EnsureCapacity(count);
            fixed (byte* dest = &_buffer[_position])
            {
                Buffer.MemoryCopy(buffer, dest, count, count);
            }
            _position += count;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>Writes a 32-bit float to the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSingle(float value)
        {
            const int size = sizeof(float);
            EnsureCapacity(size);

            // Use unsafe for direct conversion to bytes
            unsafe
            {
                uint intValue = *(uint*)&value;
                _buffer[_position] = (byte)intValue;
                _buffer[_position + 1] = (byte)(intValue >> 8);
                _buffer[_position + 2] = (byte)(intValue >> 16);
                _buffer[_position + 3] = (byte)(intValue >> 24);
            }
            _position += size;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>Writes a 64-bit double to the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteDouble(double value)
        {
            const int size = sizeof(double);
            EnsureCapacity(size);

            // Use unsafe for direct conversion to bytes
            unsafe
            {
                ulong longValue = *(ulong*)&value;
                _buffer[_position] = (byte)longValue;
                _buffer[_position + 1] = (byte)(longValue >> 8);
                _buffer[_position + 2] = (byte)(longValue >> 16);
                _buffer[_position + 3] = (byte)(longValue >> 24);
                _buffer[_position + 4] = (byte)(longValue >> 32);
                _buffer[_position + 5] = (byte)(longValue >> 40);
                _buffer[_position + 6] = (byte)(longValue >> 48);
                _buffer[_position + 7] = (byte)(longValue >> 56);
            }
            _position += size;
            if (_position > _length)
                _length = _position;
        }
    }
}
