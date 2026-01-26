using System;
using System.Runtime.CompilerServices;
using EasyToolKit.Serialization.Utilities;

namespace EasyToolKit.Serialization.Formatters.Implementations
{
    public sealed partial class BinaryWritingFormatter
    {
        /// <summary>
        /// Ensures the buffer has enough capacity for the specified number of bytes.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCapacity(int required)
        {
            if (_position + required <= _buffer.Length)
                return;

            // Exponential growth: double capacity until sufficient
            int newCapacity = Math.Max(_buffer.Length * 2, _position + required);
            Array.Resize(ref _buffer, newCapacity);
        }

        /// <summary>
        /// Writes a byte array to the buffer.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            EnsureCapacity(data.Length);
            fixed (byte* srcPtr = data)
            fixed (byte* destPtr = &_buffer[_position])
            {
                MemoryUtility.FastMemoryCopy(srcPtr, destPtr, data.Length);
            }

            _position += data.Length;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a string as raw char memory (2 bytes per char) to the buffer with a length prefix.
        /// </summary>
        private unsafe void WriteBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                WriteUInt32Optimized(0);
                return;
            }

            if ((_options & BinaryFormatterOptions.EnableDirectMemoryCopy) != 0)
            {
                // Current implementation: 2 bytes per char (UTF-16LE)
                int charCount = str.Length;
                WriteUInt32Optimized((uint)charCount);
                int byteCount = charCount * sizeof(char);
                EnsureCapacity(byteCount);

                fixed (char* charPtr = str)
                fixed (byte* destPtr = &_buffer[_position])
                {
                    MemoryUtility.FastMemoryCopy(charPtr, destPtr, byteCount);
                }

                _position += byteCount;
            }
            else
            {
                // UTF-8 encoding (smaller for ASCII, but encoding overhead)
                byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(str);
                WriteUInt32Optimized((uint)utf8Bytes.Length);
                WriteBytes(utf8Bytes);
            }

            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer using variable-length encoding.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteVarint32(uint value)
        {
            // Special case for zero
            if (value == 0)
            {
                EnsureCapacity(1);
                _buffer[_position++] = 0;
                if (_position > _length)
                    _length = _position;
                return;
            }

            // Unrolled loop for varint encoding (max 5 bytes)
            EnsureCapacity(5);

            byte* buffer = stackalloc byte[5];
            int index = 0;

            while (value > 0x7F)
            {
                buffer[index++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }

            buffer[index] = (byte)value;

            int count = index + 1;
            for (int i = 0; i < count; i++)
            {
                _buffer[_position++] = buffer[i];
            }

            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer using variable-length encoding.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WriteVarint64(ulong value)
        {
            // Special case for zero
            if (value == 0)
            {
                EnsureCapacity(1);
                _buffer[_position++] = 0;
                if (_position > _length)
                    _length = _position;
                return;
            }

            // Optimized loop for 64-bit varint (max 10 bytes)
            EnsureCapacity(10);

            byte* buffer = stackalloc byte[10];
            int index = 0;

            while (value > 0x7F)
            {
                buffer[index++] = (byte)((value & 0x7F) | 0x80);
                value >>= 7;
            }

            buffer[index] = (byte)value;

            int count = index + 1;
            for (int i = 0; i < count; i++)
            {
                _buffer[_position++] = buffer[i];
            }

            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a 16-bit unsigned integer using fixed-width encoding (2 bytes).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUInt16Fixed(ushort value)
        {
            const int size = 2;
            EnsureCapacity(size);
            _buffer[_position++] = (byte)value;
            _buffer[_position++] = (byte)(value >> 8);
            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer using fixed-width encoding (4 bytes).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUInt32Fixed(uint value)
        {
            const int size = 4;
            EnsureCapacity(size);
            _buffer[_position++] = (byte)value;
            _buffer[_position++] = (byte)(value >> 8);
            _buffer[_position++] = (byte)(value >> 16);
            _buffer[_position++] = (byte)(value >> 24);
            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a 32-bit unsigned integer using varint or fixed encoding based on formatter options.
        /// </summary>
        /// <param name="value">The value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUInt32Optimized(uint value)
        {
            if ((_options & BinaryFormatterOptions.EnableVarintEncoding) != 0)
            {
                WriteVarint32(value);
            }
            else
            {
                WriteUInt32Fixed(value);
            }
        }

        /// <summary>
        /// Writes a 64-bit unsigned integer using fixed-width encoding (8 bytes).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUInt64Fixed(ulong value)
        {
            const int size = 8;
            EnsureCapacity(size);
            _buffer[_position++] = (byte)value;
            _buffer[_position++] = (byte)(value >> 8);
            _buffer[_position++] = (byte)(value >> 16);
            _buffer[_position++] = (byte)(value >> 24);
            _buffer[_position++] = (byte)(value >> 32);
            _buffer[_position++] = (byte)(value >> 40);
            _buffer[_position++] = (byte)(value >> 48);
            _buffer[_position++] = (byte)(value >> 56);
            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a 32-bit float to the buffer.
        /// </summary>
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

        /// <summary>
        /// Writes a 64-bit double to the buffer.
        /// </summary>
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

        /// <summary>
        /// Writes a primitive array to the buffer using direct memory copy.
        /// </summary>
        /// <typeparam name="T">The unmanaged type of elements in the array.</typeparam>
        /// <param name="data">The array to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WritePrimitiveArray<T>(T[] data) where T : unmanaged
        {
            if (data.Length == 0)
                return;

            int byteCount = data.Length * sizeof(T);
            EnsureCapacity(byteCount);

            fixed (T* srcPtr = data)
            fixed (byte* destPtr = &_buffer[_position])
            {
                MemoryUtility.FastMemoryCopy(srcPtr, destPtr, byteCount);
            }

            _position += byteCount;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes an unmanaged value to the buffer using direct memory copy.
        /// </summary>
        /// <typeparam name="T">The unmanaged type to write.</typeparam>
        /// <param name="value">The value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe void WritePrimitiveValue<T>(T value) where T : unmanaged
        {
            int byteCount = sizeof(T);
            EnsureCapacity(byteCount);

            fixed (byte* destPtr = &_buffer[_position])
            {
                T* srcPtr = &value;
                MemoryUtility.FastMemoryCopy(srcPtr, destPtr, byteCount);
            }

            _position += byteCount;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>
        /// Writes a tag when IncludeTypeTags option is enabled.
        /// </summary>
        /// <param name="tag">The tag value to write.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteOptionTag(BinaryFormatterTag tag)
        {
            if ((_options & BinaryFormatterOptions.IncludeTypeTags) != 0)
            {
                WriteTag(tag);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteTag(BinaryFormatterTag tag)
        {
            WriteByte((byte)tag);
        }
    }
}
