using System;
using System.Text;
using NUnit.Framework;
using EasyToolkit.Serialization;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Formatters.Implementations;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>
    /// Unit tests for JSON reading formatter.
    /// </summary>
    [TestFixture]
    public class TestJsonReadingFormatter
    {
        #region Constructor

        /// <summary>
        /// Verifies that JsonReadingFormatter initializes with correct type.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_Constructor_ReturnsJsonFormat()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();

            // Assert
            Assert.AreEqual(SerializationFormat.Json, formatter.FormatType);
        }

        #endregion

        #region Buffer Management

        /// <summary>
        /// Verifies that SetBuffer parses JSON correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_SetBuffer_ParsesJsonCorrectly()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":42}";
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            // Act
            formatter.SetBuffer(buffer);

            // Assert
            Assert.AreEqual(json, Encoding.UTF8.GetString(formatter.GetBuffer()));
        }

        /// <summary>
        /// Verifies that GetBuffer returns the original JSON text.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_GetBuffer_ReturnsOriginalJson()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"test\":\"value\"}";
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            formatter.SetBuffer(buffer);

            // Act
            ReadOnlySpan<byte> result = formatter.GetBuffer();

            // Assert
            Assert.AreEqual(json, Encoding.UTF8.GetString(result));
        }

        /// <summary>
        /// Verifies that GetPosition throws NotSupportedException for JSON format.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_GetPosition_ThrowsNotSupportedException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":42}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() => formatter.GetPosition());
            Assert.IsTrue(ex.Message.Contains("GetPosition is not supported for JSON format"));
        }

        /// <summary>
        /// Verifies that GetRemainingLength throws NotSupportedException for JSON format.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_GetRemainingLength_ThrowsNotSupportedException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":42}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            var ex = Assert.Throws<NotSupportedException>(() => formatter.GetRemainingLength());
            Assert.IsTrue(ex.Message.Contains("GetRemainingLength is not supported for JSON format"));
        }

        #endregion

        #region Read Primitive Types

        /// <summary>
        /// Verifies that reading an integer from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadInt_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":12345}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            int result = 0;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(12345, result);
        }

        /// <summary>
        /// Verifies that reading a negative integer from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNegativeInt_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":-99999}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            int result = 0;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(-99999, result);
        }

        /// <summary>
        /// Verifies that reading a long from JSON works correctly.
        /// Note: Uses 2^52 which is within double's precise integer range (±2^53).
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadLong_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            // Use 2^52 which is within double's precise integer range
            long testValue = 4503599627370496;
            string json = "{\"value\":" + testValue + "}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            long result = 0;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(testValue, result);
        }

        /// <summary>
        /// Verifies that reading a float from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadFloat_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":3.14159}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            float result = 0f;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(3.14159f, result, 0.00001f);
        }

        /// <summary>
        /// Verifies that reading a double from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadDouble_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":123.45678901234}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            double result = 0d;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(123.45678901234, result, 0.00000001);
        }

        /// <summary>
        /// Verifies that reading a boolean true from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadBoolTrue_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":true}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            bool result = false;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that reading a boolean false from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadBoolFalse_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":false}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            bool result = true;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that reading a string from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadString_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":\"Hello, World!\"}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            string result = null;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        /// <summary>
        /// Verifies that reading an empty string from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadEmptyString_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":\"\"}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            string result = null;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that reading a null string from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNullString_ReturnsNull()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":null}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            string result = "not null";
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that reading a unicode string from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadUnicodeString_ReturnsCorrectValue()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":\"Hello 世界! 🌍\"}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            string result = null;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.AreEqual("Hello 世界! 🌍", result);
        }

        /// <summary>
        /// Verifies that reading all integer types from JSON works correctly.
        /// Note: long and ulong values are limited to ±2^52 range due to double precision.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadAllIntegerTypes_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            // Use values within double's precise integer range (±2^53)
            // 2^52 = 4503599627370496, 2^53-1 = 9007199254740991
            string json = "{\"byte\":255,\"sbyte\":-128,\"short\":-32768,\"ushort\":65535,\"int\":-2147483648,\"uint\":4294967295,\"long\":-4503599627370496,\"ulong\":9007199254740991}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            byte byteValue = 0;
            formatter.BeginMember("byte");
            formatter.Format(ref byteValue);

            sbyte sbyteValue = 0;
            formatter.BeginMember("sbyte");
            formatter.Format(ref sbyteValue);

            short shortValue = 0;
            formatter.BeginMember("short");
            formatter.Format(ref shortValue);

            ushort ushortValue = 0;
            formatter.BeginMember("ushort");
            formatter.Format(ref ushortValue);

            int intValue = 0;
            formatter.BeginMember("int");
            formatter.Format(ref intValue);

            uint uintValue = 0;
            formatter.BeginMember("uint");
            formatter.Format(ref uintValue);

            long longValue = 0;
            formatter.BeginMember("long");
            formatter.Format(ref longValue);

            ulong ulongValue = 0;
            formatter.BeginMember("ulong");
            formatter.Format(ref ulongValue);
            formatter.EndObject();

            // Assert
            Assert.AreEqual((byte)255, byteValue);
            Assert.AreEqual((sbyte)-128, sbyteValue);
            Assert.AreEqual((short)-32768, shortValue);
            Assert.AreEqual((ushort)65535, ushortValue);
            Assert.AreEqual(-2147483648, intValue);
            Assert.AreEqual((uint)4294967295, uintValue);
            // Use 2^52 which is within double's precise integer range
            Assert.AreEqual(-4503599627370496, longValue);
            // Use 2^53-1 which is within double's precise integer range
            Assert.AreEqual((ulong)9007199254740991, ulongValue);
        }

        #endregion

        #region Read Arrays

        /// <summary>
        /// Verifies that reading an int array from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadIntArray_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":[1,2,3,4,5]}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            int[] result = null;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(5, result[4]);
        }

        /// <summary>
        /// Verifies that reading an empty array from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadEmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":[]}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            int[] result = null;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }


        /// <summary>
        /// Verifies that reading a bool array from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadBoolArray_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":[true,false,true,false,true]}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("value");
            bool[] result = null;
            formatter.Format(ref result);
            formatter.EndObject();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(5, result.Length);
            Assert.IsTrue(result[0]);
            Assert.IsFalse(result[1]);
            Assert.IsTrue(result[4]);
        }

        #endregion

        #region Read Nested Objects

        /// <summary>
        /// Verifies that reading a nested object from JSON works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNestedObject_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"inner\":{\"id\":42,\"name\":\"test\"}}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("inner");
            formatter.BeginObject();

            int id = 0;
            formatter.BeginMember("id");
            formatter.Format(ref id);

            string name = null;
            formatter.BeginMember("name");
            formatter.Format(ref name);

            formatter.EndObject();

            // Assert
            Assert.AreEqual(42, id);
            Assert.AreEqual("test", name);
        }

        /// <summary>
        /// Verifies that reading multiple members from an object works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadMultipleMembers_AllReadCorrectly()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"intValue\":42,\"floatValue\":3.14,\"stringValue\":\"test\",\"boolValue\":true}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            int intValue = 0;
            formatter.BeginMember("intValue");
            formatter.Format(ref intValue);

            float floatValue = 0f;
            formatter.BeginMember("floatValue");
            formatter.Format(ref floatValue);

            string stringValue = null;
            formatter.BeginMember("stringValue");
            formatter.Format(ref stringValue);

            bool boolValue = false;
            formatter.BeginMember("boolValue");
            formatter.Format(ref boolValue);
            formatter.EndObject();

            // Assert
            Assert.AreEqual(42, intValue);
            Assert.AreEqual(3.14f, floatValue, 0.001f);
            Assert.AreEqual("test", stringValue);
            Assert.IsTrue(boolValue);
        }

        /// <summary>
        /// Verifies that reading nested objects with arrays works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNestedObjectWithArray_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"data\":{\"items\":[1,2,3],\"count\":3}}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("data");
            formatter.BeginObject();

            int[] items = null;
            formatter.BeginMember("items");
            formatter.Format(ref items);

            int count = 0;
            formatter.BeginMember("count");
            formatter.Format(ref count);

            formatter.EndObject();

            // Assert
            Assert.AreEqual(3, items.Length);
            Assert.AreEqual(1, items[0]);
            Assert.AreEqual(3, count);
        }

        /// <summary>
        /// Verifies that reading null object value works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNullObject_DoesNotEnterScope()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"data\":null,\"other\":42}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("data");
            // BeginObject on null should not enter hierarchy
            formatter.BeginObject();
            formatter.EndObject();

            int other = 0;
            formatter.BeginMember("other");
            formatter.Format(ref other);

            // Assert
            Assert.AreEqual(42, other);
        }

        #endregion

        #region Error Handling

        /// <summary>
        /// Verifies that reading a number when expecting a boolean throws DataFormatException.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNumberAsBool_ThrowsDataFormatException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":42}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            formatter.BeginObject();
            formatter.BeginMember("value");
            bool result = false;
            Assert.Throws<DataFormatException>(() => formatter.Format(ref result));
            formatter.EndObject();
        }

        /// <summary>
        /// Verifies that reading a string when expecting a number throws DataFormatException.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadStringAsNumber_ThrowsDataFormatException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":\"not a number\"}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            formatter.BeginObject();
            formatter.BeginMember("value");
            int result = 0;
            Assert.Throws<DataFormatException>(() => formatter.Format(ref result));
            formatter.EndObject();
        }

        /// <summary>
        /// Verifies that reading a non-array when expecting an array throws DataFormatException.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNonArrayAsArray_ThrowsDataFormatException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":\"not an array\"}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            formatter.BeginObject();
            formatter.BeginMember("value");
            Assert.Throws<DataFormatException>(() =>
            {
                int[] result = null;
                formatter.Format(ref result);
            });
            formatter.EndObject();
        }

        /// <summary>
        /// Verifies that reading a non-object when expecting an object throws DataFormatException.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadNonObjectAsObject_ThrowsDataFormatException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":\"not an object\"}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            formatter.BeginObject();
            formatter.BeginMember("value");
            Assert.Throws<DataFormatException>(() => formatter.BeginObject());
            formatter.EndObject();
        }

        /// <summary>
        /// Verifies that calling EndObject without BeginObject throws InvalidOperationException.
        /// Root is already in object context, so this should fail.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_EndObjectWithoutBegin_ThrowsInvalidOperationException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":42}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            // Root is already in object context, so EndObject without matching BeginObject should throw
            Assert.Throws<InvalidOperationException>(() => formatter.EndObject());
        }

        /// <summary>
        /// Verifies that calling EndArray without BeginArray throws InvalidOperationException.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_EndArrayWithoutBegin_ThrowsInvalidOperationException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"value\":[1,2,3]}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => formatter.EndArray());
        }

        /// <summary>
        /// Verifies that mismatched BeginObject/BeginArray throws InvalidOperationException.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_MismatchedBeginEnd_ThrowsInvalidOperationException()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"inner\":{\"value\":42}}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act & Assert
            // Root is already in object context
            formatter.BeginObject();
            formatter.BeginMember("inner");
            formatter.BeginObject();
            var ex = Assert.Throws<InvalidOperationException>(() => formatter.EndArray());
            Assert.IsTrue(ex.Message.Contains("Expected EndObject"));
        }

        #endregion

        #region Complex Scenarios

        /// <summary>
        /// Verifies that reading a complex nested structure works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadComplexStructure_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"player\":{\"id\":1,\"name\":\"Hero\",\"position\":{\"x\":10.5,\"y\":20.3},\"inventory\":[\"sword\",\"shield\",\"potion\"],\"stats\":{\"hp\":100,\"mp\":50}}}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("player");
            formatter.BeginObject();

            int id = 0;
            formatter.BeginMember("id");
            formatter.Format(ref id);

            string name = null;
            formatter.BeginMember("name");
            formatter.Format(ref name);

            // Read nested position object
            formatter.BeginMember("position");
            formatter.BeginObject();

            float x = 0f;
            formatter.BeginMember("x");
            formatter.Format(ref x);

            float y = 0f;
            formatter.BeginMember("y");
            formatter.Format(ref y);

            formatter.EndObject();

            // Read nested stats object
            formatter.BeginMember("stats");
            formatter.BeginObject();

            int hp = 0;
            formatter.BeginMember("hp");
            formatter.Format(ref hp);

            int mp = 0;
            formatter.BeginMember("mp");
            formatter.Format(ref mp);

            formatter.EndObject();
            formatter.EndObject();

            // Assert
            Assert.AreEqual(1, id);
            Assert.AreEqual("Hero", name);
            Assert.AreEqual(10.5f, x, 0.001f);
            Assert.AreEqual(20.3f, y, 0.001f);
            Assert.AreEqual(100, hp);
            Assert.AreEqual(50, mp);
        }

        /// <summary>
        /// Verifies that reading array of arrays works correctly.
        /// </summary>
        [Test]
        public void JsonReadingFormatter_ReadArrayOfArrays_ReturnsCorrectValues()
        {
            // Arrange
            IReadingFormatter formatter = new JsonReadingFormatter();
            string json = "{\"matrix\":[[1,2,3],[4,5,6],[7,8,9]]}";
            formatter.SetBuffer(Encoding.UTF8.GetBytes(json));

            // Act
            formatter.BeginObject();
            formatter.BeginMember("matrix");

            int outerLength = 0;
            formatter.BeginArray(ref outerLength);

            int[][] result = new int[outerLength][];
            for (int i = 0; i < outerLength; i++)
            {
                int innerLength = 0;
                formatter.BeginArray(ref innerLength);
                result[i] = new int[innerLength];
                for (int j = 0; j < innerLength; j++)
                {
                    formatter.Format(ref result[i][j]);
                }
                formatter.EndArray();
            }

            formatter.EndArray();

            // Assert
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(1, result[0][0]);
            Assert.AreEqual(5, result[1][1]);
            Assert.AreEqual(9, result[2][2]);
        }

        #endregion
    }
}
