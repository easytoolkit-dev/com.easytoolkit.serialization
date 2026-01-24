using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using EasyToolKit.Core.Pooling;

namespace EasyToolKit.Serialization.Implementations
{
    /// <summary>
    /// Binary reading formatter implementation. Deserializes data from a binary format
    /// using length-prefixed field names and varint encoding.
    /// </summary>
    public sealed class BinaryReadingFormatter : ReadingFormatterBase
    {
        private int _position;
        private byte[] _buffer;
        private int _nodeDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryReadingFormatter"/> class
        /// for object pool reuse. Use <see cref="SetBuffer"/> to set the data.
        /// </summary>
        public BinaryReadingFormatter()
        {
            _buffer = Array.Empty<byte>();
            _nodeDepth = 0;
            _position = 0;
        }

        /// <inheritdoc />
        public override SerializationFormat Type => SerializationFormat.Binary;

        /// <inheritdoc />
        public override void SetBuffer(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer.ToArray();
            _position = 0;
            _nodeDepth = 0;
        }

        /// <inheritdoc />
        public override ReadOnlySpan<byte> GetBuffer() => _buffer;

        /// <inheritdoc />
        public override int GetPosition() => _position;

        /// <inheritdoc />
        public override int GetRemainingLength() => _buffer.Length - _position;

        /// <inheritdoc />
        public override void BeginMember(string name)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag == BinaryFormatterTag.NamedMemberBegin)
            {
                var length = ReadVarint32();
                if (length > 0)
                {
                    var readName = ReadString((int)length);
                    // Verify name matches if provided
                    if (!string.IsNullOrEmpty(name) && readName != name)
                    {
                        throw new InvalidOperationException(
                            $"Member name mismatch. Expected '{name}', found '{readName}'.");
                    }
                }
            }
            else if (tag != BinaryFormatterTag.MemberBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginMember. Expected {BinaryFormatterTag.MemberBegin} or {BinaryFormatterTag.NamedMemberBegin}, found {tag}.");
            }
        }

        /// <inheritdoc />
        protected override void BeginObject()
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ObjectBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginObject. Expected {BinaryFormatterTag.ObjectBegin}, found {tag}.");
            }

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at BeginObject. Expected {_nodeDepth}, found {depth}.");
            }

            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndObject()
        {
            _nodeDepth--;

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at EndObject. Expected {_nodeDepth}, found {depth}.");
            }

            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ObjectEnd)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at EndObject. Expected {BinaryFormatterTag.ObjectEnd}, found {tag}.");
            }
        }

        /// <inheritdoc />
        protected override void BeginArray(ref int length)
        {
            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ArrayBegin)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at BeginArray. Expected {BinaryFormatterTag.ArrayBegin}, found {tag}.");
            }

            length = (int)ReadVarint32();

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at BeginArray. Expected {_nodeDepth}, found {depth}.");
            }

            _nodeDepth++;
        }

        /// <inheritdoc />
        protected override void EndArray()
        {
            _nodeDepth--;

            var depth = (int)ReadVarint32();
            if (depth != _nodeDepth)
            {
                throw new InvalidOperationException(
                    $"Depth mismatch at EndArray. Expected {_nodeDepth}, found {depth}.");
            }

            var tag = (BinaryFormatterTag)ReadByte();
            if (tag != BinaryFormatterTag.ArrayEnd)
            {
                throw new InvalidOperationException(
                    $"Invalid tag at EndArray. Expected {BinaryFormatterTag.ArrayEnd}, found {tag}.");
            }
        }

        /// <inheritdoc />
        public override void Format(ref int value)
        {
            // Decode zigzag encoding to recover signed integer
            uint encoded = ReadVarint32();
            value = (int)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref sbyte value)
        {
            // Decode zigzag encoding to recover signed byte
            int encoded = ReadByte();
            value = (sbyte)((encoded >> 1) ^ -(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref short value)
        {
            // Decode zigzag encoding to recover signed short
            uint encoded = ReadVarint32();
            value = (short)((encoded >> 1) ^ -(int)(encoded & 1));
        }

        /// <inheritdoc />
        public override void Format(ref long value)
        {
            // Decode zigzag encoding to recover signed long
            ulong encoded = ReadVarint64();
            long decoded = (long)(encoded >> 1);
            if ((encoded & 1) == 1)
                decoded = ~decoded;
            value = decoded;
        }

        /// <inheritdoc />
        public override void Format(ref byte value)
        {
            value = ReadByte();
        }

        /// <inheritdoc />
        public override void Format(ref ushort value)
        {
            value = (ushort)ReadVarint32();
        }

        /// <inheritdoc />
        public override void Format(ref uint value)
        {
            value = ReadVarint32();
        }

        /// <inheritdoc />
        public override void Format(ref ulong value)
        {
            value = ReadVarint64();
        }

        /// <inheritdoc />
        public override void Format(ref bool value)
        {
            var byteValue = ReadByte();
            value = byteValue != 0;
        }

        /// <inheritdoc />
        public override void Format(ref float value)
        {
            value = ReadSingle();
        }

        /// <inheritdoc />
        public override void Format(ref double value)
        {
            value = ReadDouble();
        }

        /// <inheritdoc />
        public override void Format(ref string str)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                str = string.Empty;
                return;
            }

            str = ReadString((int)length);
        }

        /// <summary>Reads a UTF-8 string from the buffer with the specified byte length.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ReadString(int byteCount)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount), "Byte count cannot be negative.");
            if (_position + byteCount > _buffer.Length)
                throw new EndOfStreamException($"Attempted to read {byteCount} bytes but only {_buffer.Length - _position} bytes available.");

            unsafe
            {
                fixed (byte* bytePtr = &_buffer[_position])
                {
                    _position += byteCount;
                    return Encoding.UTF8.GetString(bytePtr, byteCount);
                }
            }
        }

        /// <inheritdoc />
        public override void Format(ref byte[] data)
        {
            var length = ReadVarint32();
            if (length == 0)
            {
                data = null;
                return;
            }

            data = ReadBytes((int)length).ToArray();
        }

        /// <inheritdoc />
        public override void Format(ref UnityEngine.Object unityObject)
        {
            var index = ReadVarint32();
            unityObject = ResolveReference((int)index);
        }

        /// <summary>Reads a single byte from the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ReadByte()
        {
            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");
            return _buffer[_position++];
        }

        /// <summary>Reads the specified number of bytes from the buffer.</summary>
        private ReadOnlySpan<byte> ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot be negative.");
            if (_position + count > _buffer.Length)
                throw new EndOfStreamException($"Attempted to read {count} bytes but only {_buffer.Length - _position} bytes available.");

            var result = _buffer.AsSpan(_position, count);
            _position += count;
            return result;
        }

        /// <summary>Reads a 32-bit unsigned integer using variable-length decoding.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ReadVarint32()
        {
            uint value = 0;
            int shift = 0;

            // Unrolled loop for performance (handles up to 5 bytes inline)
            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            byte b = _buffer[_position++];
            value = (uint)(b & 0x7F);
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 7;
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 14;
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 21;
            if ((b & 0x80) == 0) return value;

            if (_position >= _buffer.Length)
                throw new EndOfStreamException("Attempted to read past the end of the buffer.");

            b = _buffer[_position++];
            value |= (uint)(b & 0x7F) << 28;
            if ((b & 0x80) == 0) return value;

            throw new InvalidDataException("Invalid varint32: too many bytes.");
        }

        /// <summary>Reads a 64-bit unsigned integer using variable-length decoding.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ulong ReadVarint64()
        {
            ulong value = 0;
            int shift = 0;
            byte b;

            // Optimized loop for 64-bit varint
            do
            {
                if (_position >= _buffer.Length)
                    throw new EndOfStreamException("Attempted to read past the end of the buffer.");

                b = _buffer[_position++];

                if (shift >= 64)
                    throw new InvalidDataException("Invalid varint64: too many bytes.");

                value |= (ulong)(b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);

            return value;
        }

        /// <summary>Reads a 32-bit float from the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ReadSingle()
        {
            const int size = sizeof(float);
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException($"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            // Use unsafe for direct conversion from bytes
            unsafe
            {
                uint value =
                    (uint)_buffer[_position] |
                    (uint)_buffer[_position + 1] << 8 |
                    (uint)_buffer[_position + 2] << 16 |
                    (uint)_buffer[_position + 3] << 24;
                _position += size;
                return *(float*)&value;
            }
        }

        /// <summary>Reads a 64-bit double from the buffer.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ReadDouble()
        {
            const int size = sizeof(double);
            if (_position + size > _buffer.Length)
                throw new EndOfStreamException($"Attempted to read {size} bytes but only {_buffer.Length - _position} bytes available.");

            // Use unsafe for direct conversion from bytes
            unsafe
            {
                ulong low =
                    (ulong)_buffer[_position] |
                    (ulong)_buffer[_position + 1] << 8 |
                    (ulong)_buffer[_position + 2] << 16 |
                    (ulong)_buffer[_position + 3] << 24;
                ulong high =
                    (ulong)_buffer[_position + 4] |
                    (ulong)_buffer[_position + 5] << 8 |
                    (ulong)_buffer[_position + 6] << 16 |
                    (ulong)_buffer[_position + 7] << 24;
                _position += size;
                ulong value = low | (high << 32);
                return *(double*)&value;
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            _position = 0;
            _nodeDepth = 0;
            PoolUtility.ReleaseObject(this);
            base.Dispose();
        }
    }
}
