using System;
using System.Runtime.CompilerServices;
using EasyToolKit.Serialization.Utilities;

namespace EasyToolKit.Serialization.Formatters.Implementations
{
    public sealed partial class BinaryWritingFormatter
    {
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
                MemoryUtility.FastMemoryCopy(srcPtr, destPtr, data.Length);
            }

            _position += data.Length;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>Writes a string as raw char memory (2 bytes per char) to the buffer with a length prefix.</summary>
        private unsafe void WriteBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                WriteVarint32(0);
                return;
            }

            // Write string length as char count (not byte count)
            int charCount = str.Length;
            WriteVarint32((uint)charCount);

            // Calculate byte count (2 bytes per char)
            int byteCount = charCount * sizeof(char);
            EnsureCapacity(byteCount);

            fixed (char* charPtr = str)
            fixed (byte* destPtr = &_buffer[_position])
            {
                // Copy raw char memory directly without encoding
                MemoryUtility.FastMemoryCopy(charPtr, destPtr, byteCount);
            }

            _position += byteCount;
            if (_position > _length)
                _length = _position;
        }

        /// <summary>Writes a 32-bit unsigned integer using variable-length encoding.</summary>
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

        /// <summary>Writes a 64-bit unsigned integer using variable-length encoding.</summary>
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
