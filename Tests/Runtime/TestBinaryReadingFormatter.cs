using System;
using System.IO;
using NUnit.Framework;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Formatters.Implementations;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>
    /// Unit tests for <see cref="BinaryReadingFormatter"/>.
    /// </summary>
    [TestFixture]
    public class TestBinaryReadingFormatter
    {
        #region Constructor

        /// <summary>
        /// Verifies that BinaryReadingFormatter initializes with correct type.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_Constructor_ReturnsBinaryFormat()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();

            // Assert
            Assert.AreEqual(SerializationFormat.Binary, formatter.FormatType);
        }

        #endregion

        #region Buffer Management

        /// <summary>
        /// Verifies that SetBuffer sets the buffer correctly.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_SetBuffer_SetsBufferCorrectly()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            byte[] buffer = { 1, 2, 3, 4, 5 };

            // Act
            formatter.SetBuffer(buffer);

            // Assert
            Assert.AreEqual(0, formatter.GetPosition());
            Assert.AreEqual(5, formatter.GetRemainingLength());
        }

        /// <summary>
        /// Verifies that GetBuffer returns the set buffer.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetBuffer_ReturnsSetBuffer()
        {
            // Arrange
            var formatter = new BinaryReadingFormatter();
            byte[] buffer = { 10, 20, 30, 40, 50 };
            formatter.SetBuffer(buffer);

            // Act
            ReadOnlySpan<byte> result = formatter.GetBuffer();

            // Assert
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(50, result[4]);
        }

        /// <summary>
        /// Verifies that GetPosition returns current position.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetPosition_ReturnsCurrentPosition()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            IReadingFormatter readFormatter = new BinaryReadingFormatter();
            int value = 42;
            writeFormatter.Format(ref value);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);

            // Act
            readFormatter.Format(ref value);

            // Assert
            Assert.AreEqual(buffer.Length, readFormatter.GetPosition());
        }

        /// <summary>
        /// Verifies that GetRemainingLength returns remaining bytes.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_GetRemainingLength_ReturnsRemainingBytes()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            IReadingFormatter readFormatter = new BinaryReadingFormatter();
            int value = 42;
            writeFormatter.Format(ref value);
            byte[] buffer = writeFormatter.ToArray();
            readFormatter.SetBuffer(buffer);

            // Act
            int initialRemaining = readFormatter.GetRemainingLength();
            readFormatter.Format(ref value);
            int finalRemaining = readFormatter.GetRemainingLength();

            // Assert
            Assert.AreEqual(buffer.Length, initialRemaining);
            Assert.AreEqual(0, finalRemaining);
        }

        #endregion

        #region Read Primitive Types

        /// <summary>
        /// Verifies that reading an int with default options works correctly.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadInt_ReturnsCorrectValue()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            IReadingFormatter readFormatter = new BinaryReadingFormatter();
            int original = 65535;

            // Act
            writeFormatter.Format(ref original);
            readFormatter.SetBuffer(writeFormatter.ToArray());
            int result = 0;
            readFormatter.Format(ref result);

            // Assert
            Assert.AreEqual(65535, result);
        }

        /// <summary>
        /// Verifies that reading all integer types works correctly.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadAllIntegerTypes_ReturnsCorrectValues()
        {
            // Arrange
            var writeFormatter = new BinaryWritingFormatter();
            IReadingFormatter readFormatter = new BinaryReadingFormatter();

            byte byteValue = 255;
            sbyte sbyteValue = -128;
            short shortValue = -32768;
            ushort ushortValue = 65535;
            int intValue = -2147483648;
            uint uintValue = 4294967295;
            long longValue = -9223372036854775808;
            ulong ulongValue = 18446744073709551615;

            // Act
            writeFormatter.Format(ref byteValue);
            writeFormatter.Format(ref sbyteValue);
            writeFormatter.Format(ref shortValue);
            writeFormatter.Format(ref ushortValue);
            writeFormatter.Format(ref intValue);
            writeFormatter.Format(ref uintValue);
            writeFormatter.Format(ref longValue);
            writeFormatter.Format(ref ulongValue);

            readFormatter.SetBuffer(writeFormatter.ToArray());

            readFormatter.Format(ref byteValue);
            readFormatter.Format(ref sbyteValue);
            readFormatter.Format(ref shortValue);
            readFormatter.Format(ref ushortValue);
            readFormatter.Format(ref intValue);
            readFormatter.Format(ref uintValue);
            readFormatter.Format(ref longValue);
            readFormatter.Format(ref ulongValue);

            // Assert
            Assert.AreEqual((byte)255, byteValue);
            Assert.AreEqual((sbyte)-128, sbyteValue);
            Assert.AreEqual((short)-32768, shortValue);
            Assert.AreEqual((ushort)65535, ushortValue);
            Assert.AreEqual(-2147483648, intValue);
            Assert.AreEqual((uint)4294967295, uintValue);
            Assert.AreEqual(-9223372036854775808, longValue);
            Assert.AreEqual((ulong)18446744073709551615, ulongValue);
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// Verifies that reading past buffer end throws EndOfStreamException.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadPastEnd_ThrowsEndOfStreamException()
        {
            // Arrange
            IReadingFormatter formatter = new BinaryReadingFormatter();
            // Use settings without type tags to test insufficient buffer scenario
            formatter.Settings = new BinaryFormatterSettings { Options = BinaryFormatterOptions.None };
            formatter.SetBuffer(new byte[] { 1, 2 }); // Too small for an int

            // Act & Assert
            Assert.Throws<EndOfStreamException>(() =>
            {
                int value = 0;
                formatter.Format(ref value);
            });
        }

        /// <summary>
        /// Verifies that reading empty buffer throws EndOfStreamException.
        /// </summary>
        [Test]
        public void BinaryReadingFormatter_ReadEmptyBuffer_ThrowsEndOfStreamException()
        {
            // Arrange
            IReadingFormatter formatter = new BinaryReadingFormatter();
            // Use settings without type tags to test empty buffer scenario
            formatter.Settings = new BinaryFormatterSettings { Options = BinaryFormatterOptions.None, ReturnDefaultOnStreamEnd = false };
            formatter.SetBuffer(Array.Empty<byte>());

            // Act & Assert
            Assert.Throws<EndOfStreamException>(() =>
            {
                int value = 0;
                formatter.Format(ref value);
            });
        }

        #endregion
    }
}
