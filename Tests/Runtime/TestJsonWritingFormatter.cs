using System;
using System.Text;
using NUnit.Framework;
using EasyToolkit.Serialization;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Formatters.Implementations;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>
    /// Unit tests for <see cref="JsonWritingFormatter"/>.
    /// </summary>
    [TestFixture]
    public class TestJsonWritingFormatter
    {
        #region Constructor

        /// <summary>
        /// Verifies that JsonWritingFormatter initializes with correct type.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_Constructor_ReturnsJsonFormat()
        {
            // Arrange
            IWritingFormatter formatter = new JsonWritingFormatter();

            // Assert
            Assert.AreEqual(SerializationFormat.Json, formatter.FormatType);
        }

        #endregion

        #region Buffer Management

        /// <summary>
        /// Verifies that GetBuffer returns JSON bytes.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_GetBuffer_ReturnsJsonBytes()
        {
            // Arrange
            IWritingFormatter formatter = new JsonWritingFormatter();
            int value = 42;
            formatter.BeginObject();
            formatter.BeginMember("test");
            formatter.Format(ref value);
            formatter.EndObject();

            // Act
            byte[] buffer = formatter.GetBuffer();

            // Assert
            Assert.IsNotNull(buffer);
            Assert.IsNotEmpty(buffer);
            Assert.AreEqual("{\"test\":42}", Encoding.UTF8.GetString(buffer));
        }

        /// <summary>
        /// Verifies that ToArray returns same buffer as GetBuffer.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_ToArray_ReturnsSameAsGetBuffer()
        {
            // Arrange
            IWritingFormatter formatter = new JsonWritingFormatter();
            int value = 42;
            formatter.BeginObject();
            formatter.BeginMember("test");
            formatter.Format(ref value);
            formatter.EndObject();

            // Act
            byte[] toArray = formatter.ToArray();
            byte[] getBuffer = formatter.GetBuffer();

            // Assert
            Assert.AreEqual(toArray, getBuffer);
        }

        /// <summary>
        /// Verifies that GetLength returns buffer length.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_GetLength_ReturnsBufferLength()
        {
            // Arrange
            IWritingFormatter formatter = new JsonWritingFormatter();
            int value = 42;
            formatter.BeginObject();
            formatter.BeginMember("test");
            formatter.Format(ref value);
            formatter.EndObject();

            // Act
            int length = formatter.GetLength();

            // Assert
            Assert.AreEqual(formatter.GetBuffer().Length, length);
        }

        /// <summary>
        /// Verifies that GetPosition throws NotSupportedException for JSON format.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_GetPosition_ThrowsNotSupportedException()
        {
            // Arrange
            IWritingFormatter formatter = new JsonWritingFormatter();

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() => formatter.GetPosition());
            Assert.IsTrue(ex.Message.Contains("GetPosition is not supported for JSON format"));
        }

        /// <summary>
        /// Verifies that empty formatter returns empty object.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_EmptyFormatter_ReturnsEmptyObject()
        {
            // Arrange
            IWritingFormatter formatter = new JsonWritingFormatter();

            // Act
            byte[] buffer = formatter.GetBuffer();

            // Assert
            Assert.AreEqual("{}", Encoding.UTF8.GetString(buffer));
        }

        #endregion

        #region Write Primitive Types

        /// <summary>
        /// Verifies that writing and reading an int produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteInt_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            int original = 12345;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            int result = 0;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(12345, result);
        }

        /// <summary>
        /// Verifies that writing and reading a negative int produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteNegativeInt_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            int original = -99999;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            int result = 0;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(-99999, result);
        }

        /// <summary>
        /// Verifies that writing and reading a long produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteLong_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            long original = 98765432101234;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            long result = 0;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(98765432101234, result);
        }

        /// <summary>
        /// Verifies that writing and reading a float produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteFloat_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            float original = 3.14159f;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            float result = 0f;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(3.14159f, result, 0.00001f);
        }

        /// <summary>
        /// Verifies that writing and reading a double produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteDouble_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            double original = 123.45678901234;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            double result = 0d;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(123.45678901234, result, 0.00000001);
        }

        /// <summary>
        /// Verifies that writing and reading a bool true produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteBoolTrue_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            bool original = true;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            bool result = false;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that writing and reading a bool false produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteBoolFalse_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            bool original = false;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            bool result = true;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Write Strings

        /// <summary>
        /// Verifies that writing and reading a string produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteString_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            string original = "Hello, World!";

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            string result = null;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        /// <summary>
        /// Verifies that writing and reading an empty string produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteEmptyString_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            string original = string.Empty;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            string result = "not empty";
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that writing and reading a null string produces null.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteNullString_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            string original = null;

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            string result = "not null";
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that writing and reading a unicode string produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteUnicodeString_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            string original = "Hello 世界! 🌍";

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("value");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("value");
            string result = null;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual("Hello 世界! 🌍", result);
        }

        #endregion

        #region Write All Integer Types

        /// <summary>
        /// Verifies that writing and reading all integer types produces the original values.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteAllIntegerTypes_AllCanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act
            writeFormatter.BeginObject();

            byte byteValue = 255;
            writeFormatter.BeginMember("byte");
            writeFormatter.Format(ref byteValue);

            sbyte sbyteValue = -128;
            writeFormatter.BeginMember("sbyte");
            writeFormatter.Format(ref sbyteValue);

            short shortValue = -32768;
            writeFormatter.BeginMember("short");
            writeFormatter.Format(ref shortValue);

            ushort ushortValue = 65535;
            writeFormatter.BeginMember("ushort");
            writeFormatter.Format(ref ushortValue);

            uint uintValue = 4294967295;
            writeFormatter.BeginMember("uint");
            writeFormatter.Format(ref uintValue);

            ulong ulongValue = 9007199254740990;
            writeFormatter.BeginMember("ulong");
            writeFormatter.Format(ref ulongValue);

            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();

            byteValue = 0;
            readFormatter.BeginMember("byte");
            readFormatter.Format(ref byteValue);

            sbyteValue = 0;
            readFormatter.BeginMember("sbyte");
            readFormatter.Format(ref sbyteValue);

            shortValue = 0;
            readFormatter.BeginMember("short");
            readFormatter.Format(ref shortValue);

            ushortValue = 0;
            readFormatter.BeginMember("ushort");
            readFormatter.Format(ref ushortValue);

            uintValue = 0;
            readFormatter.BeginMember("uint");
            readFormatter.Format(ref uintValue);

            ulongValue = 0;
            readFormatter.BeginMember("ulong");
            readFormatter.Format(ref ulongValue);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual((byte)255, byteValue);
            Assert.AreEqual((sbyte)-128, sbyteValue);
            Assert.AreEqual((short)-32768, shortValue);
            Assert.AreEqual((ushort)65535, ushortValue);
            Assert.AreEqual((uint)4294967295, uintValue);
            Assert.AreEqual((ulong)9007199254740990, ulongValue);
        }

        #endregion

        #region Write Arrays

        /// <summary>
        /// Verifies that writing and reading an int array produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteIntArray_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            int[] original = { 1, 2, 3, 4, 5 };

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("values");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();
            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("values");
            int[] result = null;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(5, result[4]);
        }

        /// <summary>
        /// Verifies that writing and reading an empty array produces empty array.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            int[] original = Array.Empty<int>();

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("values");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();
            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("values");
            int[] result = null;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        /// <summary>
        /// Verifies that writing and reading a bool array produces the original value.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteBoolArray_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();
            bool[] original = { true, false, true, false, true };

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("values");
            writeFormatter.Format(ref original);
            writeFormatter.EndObject();
            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();
            readFormatter.BeginMember("values");
            bool[] result = null;
            readFormatter.Format(ref result);
            readFormatter.EndObject();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.IsTrue(result[0]);
            Assert.IsFalse(result[1]);
            Assert.IsTrue(result[4]);
        }

        #endregion

        #region Write Nested Objects

        /// <summary>
        /// Verifies that writing and reading a nested object produces the original values.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteNestedObject_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act - Write nested object
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("inner");
            writeFormatter.BeginObject(typeof(TestDataClass));

            int id = 42;
            writeFormatter.BeginMember("id");
            writeFormatter.Format(ref id);

            string name = "test";
            writeFormatter.BeginMember("name");
            writeFormatter.Format(ref name);

            writeFormatter.EndObject();
            writeFormatter.EndObject();

            // Read back
            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();

            readFormatter.BeginMember("inner");
            readFormatter.BeginObject(typeof(TestDataClass));

            id = 0;
            readFormatter.BeginMember("id");
            readFormatter.Format(ref id);

            name = null;
            readFormatter.BeginMember("name");
            readFormatter.Format(ref name);

            readFormatter.EndObject();
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(42, id);
            Assert.AreEqual("test", name);
        }

        /// <summary>
        /// Verifies that writing multiple members works correctly.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteMultipleMembers_AllCanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act
            writeFormatter.BeginObject();

            int intValue = 42;
            writeFormatter.BeginMember("intValue");
            writeFormatter.Format(ref intValue);

            float floatValue = 3.14f;
            writeFormatter.BeginMember("floatValue");
            writeFormatter.Format(ref floatValue);

            string stringValue = "test";
            writeFormatter.BeginMember("stringValue");
            writeFormatter.Format(ref stringValue);

            bool boolValue = true;
            writeFormatter.BeginMember("boolValue");
            writeFormatter.Format(ref boolValue);

            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();

            intValue = 0;
            readFormatter.BeginMember("intValue");
            readFormatter.Format(ref intValue);

            floatValue = 0f;
            readFormatter.BeginMember("floatValue");
            readFormatter.Format(ref floatValue);

            stringValue = null;
            readFormatter.BeginMember("stringValue");
            readFormatter.Format(ref stringValue);

            boolValue = false;
            readFormatter.BeginMember("boolValue");
            readFormatter.Format(ref boolValue);
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(42, intValue);
            Assert.AreEqual(3.14f, floatValue, 0.001f);
            Assert.AreEqual("test", stringValue);
            Assert.IsTrue(boolValue);
        }

        /// <summary>
        /// Verifies that writing nested object with array works correctly.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteNestedObjectWithArray_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act
            writeFormatter.BeginObject();
            writeFormatter.BeginMember("data");
            writeFormatter.BeginObject(typeof(TestDataClass));

            int[] items = { 1, 2, 3 };
            writeFormatter.BeginMember("items");
            writeFormatter.Format(ref items);

            int count = 3;
            writeFormatter.BeginMember("count");
            writeFormatter.Format(ref count);

            writeFormatter.EndObject();
            writeFormatter.EndObject();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);
            readFormatter.BeginObject();

            readFormatter.BeginMember("data");
            readFormatter.BeginObject(typeof(TestDataClass));

            items = null;
            readFormatter.BeginMember("items");
            readFormatter.Format(ref items);

            count = 0;
            readFormatter.BeginMember("count");
            readFormatter.Format(ref count);

            readFormatter.EndObject();
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual(1, items[0]);
            Assert.AreEqual(3, count);
        }

        #endregion

        #region Write Array Elements

        /// <summary>
        /// Verifies that writing values to array context works correctly.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteArrayElements_ValuesInCorrectOrder()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act
            writeFormatter.BeginMember("array");
            int length = 3;
            writeFormatter.BeginArray(ref length);

            int value1 = 10;
            writeFormatter.Format(ref value1);

            int value2 = 20;
            writeFormatter.Format(ref value2);

            int value3 = 30;
            writeFormatter.Format(ref value3);

            writeFormatter.EndArray();

            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);

            readFormatter.BeginMember("array");
            readFormatter.BeginArray(ref length);
            Assert.AreEqual(3, length);

            value1 = 0;
            readFormatter.Format(ref value1);

            value2 = 0;
            readFormatter.Format(ref value2);

            value3 = 0;
            readFormatter.Format(ref value3);

            readFormatter.EndArray();

            // Assert
            Assert.AreEqual(10, value1);
            Assert.AreEqual(20, value2);
            Assert.AreEqual(30, value3);
        }

        #endregion

        #region Unity Object Reference

        /// <summary>
        /// Verifies that Unity object reference is tracked correctly.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteUnityObject_ReturnsReferenceIndex()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();

            // Act
            writeFormatter.BeginMember("obj");
            int index = writeFormatter.RegisterReference(null);

            // Assert
            Assert.AreEqual(0, index);
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// Verifies that FormatGenericPrimitive throws NotSupportedException for JSON format.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_FormatGenericPrimitive_ThrowsNotSupportedException()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            var value = new TestUnmanagedStruct(42, 3.14f, 255);

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() =>
                writeFormatter.FormatGenericPrimitive(ref value));
            Assert.IsTrue(ex.Message.Contains("FormatGenericPrimitive is not supported in format type"));
        }

        /// <summary>
        /// Verifies that FormatGenericPrimitive array throws NotSupportedException for JSON format.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_FormatGenericPrimitiveArray_ThrowsNotSupportedException()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            var value = new[] { new TestUnmanagedStruct(42, 3.14f, 255) };

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() =>
                writeFormatter.FormatGenericPrimitive(ref value));
            Assert.IsTrue(ex.Message.Contains("FormatGenericPrimitive array is not supported in format type"));
        }

        /// <summary>
        /// Verifies that mismatched BeginObject/EndArray throws InvalidOperationException.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_MismatchedBeginEnd_ThrowsInvalidOperationException()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();

            // Act
            Type type = typeof(TestDataClass);
            writeFormatter.BeginObject(type);

            // Assert - EndArray after BeginObject should throw
            var ex = Assert.Throws<InvalidOperationException>(() => writeFormatter.EndArray());
            Assert.IsTrue(ex.Message.Contains("Expected EndObject"));
        }

        #endregion

        #region Complex Scenarios

        /// <summary>
        /// Verifies that writing a complex nested structure works correctly.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteComplexStructure_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act - Write player structure
            writeFormatter.BeginMember("player");
            Type type = typeof(TestDataClass);
            writeFormatter.BeginObject(type);

            int id = 1;
            writeFormatter.BeginMember("id");
            writeFormatter.Format(ref id);

            string name = "Hero";
            writeFormatter.BeginMember("name");
            writeFormatter.Format(ref name);

            // Nested position object
            writeFormatter.BeginMember("position");
            type = typeof(TestDataClass);
            writeFormatter.BeginObject(type);

            float x = 10.5f;
            writeFormatter.BeginMember("x");
            writeFormatter.Format(ref x);

            float y = 20.3f;
            writeFormatter.BeginMember("y");
            writeFormatter.Format(ref y);

            writeFormatter.EndObject();

            // Nested stats object
            writeFormatter.BeginMember("stats");
            type = typeof(TestDataClass);
            writeFormatter.BeginObject(type);

            int hp = 100;
            writeFormatter.BeginMember("hp");
            writeFormatter.Format(ref hp);

            int mp = 50;
            writeFormatter.BeginMember("mp");
            writeFormatter.Format(ref mp);

            writeFormatter.EndObject();
            writeFormatter.EndObject();

            // Read back
            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);

            readFormatter.BeginMember("player");
            readFormatter.BeginObject();

            id = 0;
            readFormatter.BeginMember("id");
            readFormatter.Format(ref id);

            name = null;
            readFormatter.BeginMember("name");
            readFormatter.Format(ref name);

            readFormatter.BeginMember("position");
            readFormatter.BeginObject();

            x = 0f;
            readFormatter.BeginMember("x");
            readFormatter.Format(ref x);

            y = 0f;
            readFormatter.BeginMember("y");
            readFormatter.Format(ref y);

            readFormatter.EndObject();

            readFormatter.BeginMember("stats");
            readFormatter.BeginObject();

            hp = 0;
            readFormatter.BeginMember("hp");
            readFormatter.Format(ref hp);

            mp = 0;
            readFormatter.BeginMember("mp");
            readFormatter.Format(ref mp);

            readFormatter.EndObject();
            readFormatter.EndObject();

            // Assert
            Assert.AreEqual(1, id);
            Assert.AreEqual("Hero", name);
            Assert.AreEqual(10.5f, x, 0.001f);
            Assert.AreEqual(20.3f, y, 0.001f);
            Assert.AreEqual(100, hp);
            Assert.AreEqual(50, mp);
        }

        /// <summary>
        /// Verifies that writing array of arrays works correctly.
        /// </summary>
        [Test]
        public void JsonWritingFormatter_WriteArrayOfArrays_CanBeReadBack()
        {
            // Arrange
            IWritingFormatter writeFormatter = new JsonWritingFormatter();
            IReadingFormatter readFormatter = new JsonReadingFormatter();

            // Act - Write matrix
            writeFormatter.BeginMember("matrix");
            int outerLength = 3;
            writeFormatter.BeginArray(ref outerLength);

            // First row
            int innerLength = 3;
            writeFormatter.BeginArray(ref innerLength);
            var val = 1;
            writeFormatter.Format(ref val); // 1
            val = 2;
            writeFormatter.Format(ref val); // 2
            val = 3;
            writeFormatter.Format(ref val); // 3
            writeFormatter.EndArray();

            // Second row
            innerLength = 3;
            writeFormatter.BeginArray(ref innerLength);
            val = 4;
            writeFormatter.Format(ref val); // 4
            val = 5;
            writeFormatter.Format(ref val); // 5
            val = 6;
            writeFormatter.Format(ref val); // 6
            writeFormatter.EndArray();

            // Third row
            innerLength = 3;
            writeFormatter.BeginArray(ref innerLength);
            val = 7;
            writeFormatter.Format(ref val); // 7
            val = 8;
            writeFormatter.Format(ref val); // 8
            val = 9;
            writeFormatter.Format(ref val); // 9
            writeFormatter.EndArray();

            writeFormatter.EndArray();

            // Read back
            byte[] buffer = writeFormatter.GetBuffer();
            readFormatter.SetBuffer(buffer);

            readFormatter.BeginMember("matrix");
            readFormatter.BeginArray(ref outerLength);

            int[][] result = new int[outerLength][];
            for (int i = 0; i < outerLength; i++)
            {
                readFormatter.BeginArray(ref innerLength);
                result[i] = new int[innerLength];
                for (int j = 0; j < innerLength; j++)
                {
                    readFormatter.Format(ref result[i][j]);
                }
                readFormatter.EndArray();
            }

            readFormatter.EndArray();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1, result[0][0]);
            Assert.AreEqual(5, result[1][1]);
            Assert.AreEqual(9, result[2][2]);
        }

        #endregion

        #region Integration Tests

        /// <summary>
        /// Verifies that formatter can be reused after disposal.
        /// </summary>
        #endregion
    }
}
