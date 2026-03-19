using System;
using System.Collections.Generic;
using NUnit.Framework;
using EasyToolkit.Serialization;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>
    /// Unit tests for JSON serialization functionality.
    /// </summary>
    [TestFixture]
    public class TestJsonSerializer
    {
        #region Primitive Types

        /// <summary>
        /// Verifies that serializing and deserializing an integer produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Integer_ReturnsOriginalValue()
        {
            // Arrange
            int original = 42;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int result = EasySerializer.DeserializeFromJson<int>(json);

            // Assert
            Assert.AreEqual(42, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative integer produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NegativeInteger_ReturnsOriginalValue()
        {
            // Arrange
            int original = -9999;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int result = EasySerializer.DeserializeFromJson<int>(json);

            // Assert
            Assert.AreEqual(-9999, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a boolean produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BooleanTrue_ReturnsOriginalValue()
        {
            // Arrange
            bool original = true;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            bool result = EasySerializer.DeserializeFromJson<bool>(json);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a boolean false produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BooleanFalse_ReturnsOriginalValue()
        {
            // Arrange
            bool original = false;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            bool result = EasySerializer.DeserializeFromJson<bool>(json);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a float produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Float_ReturnsOriginalValue()
        {
            // Arrange
            float original = 3.14159f;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            float result = EasySerializer.DeserializeFromJson<float>(json);

            // Assert
            Assert.AreEqual(3.14159f, result, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a double produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Double_ReturnsOriginalValue()
        {
            // Arrange
            double original = 123.456789;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            double result = EasySerializer.DeserializeFromJson<double>(json);

            // Assert
            Assert.AreEqual(123.456789, result, 0.0000001);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a sbyte (int8) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_SByte_ReturnsOriginalValue()
        {
            // Arrange
            sbyte original = 99;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            sbyte result = EasySerializer.DeserializeFromJson<sbyte>(json);

            // Assert
            Assert.AreEqual(99, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative sbyte (int8) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_SByteNegative_ReturnsOriginalValue()
        {
            // Arrange
            sbyte original = -128;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            sbyte result = EasySerializer.DeserializeFromJson<sbyte>(json);

            // Assert
            Assert.AreEqual(-128, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a short (int16) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Short_ReturnsOriginalValue()
        {
            // Arrange
            short original = 10000;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            short result = EasySerializer.DeserializeFromJson<short>(json);

            // Assert
            Assert.AreEqual(10000, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative short (int16) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ShortNegative_ReturnsOriginalValue()
        {
            // Arrange
            short original = -32768;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            short result = EasySerializer.DeserializeFromJson<short>(json);

            // Assert
            Assert.AreEqual(-32768, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a long (int64) produces the original value.
        /// Note: Uses value within double's precise integer range (±2^53).
        /// </summary>
        [Test]
        public void SerializeDeserialize_Long_ReturnsOriginalValue()
        {
            // Arrange
            // Use 2^52 which is within double's precise integer range
            long original = 4503599627370496;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            long result = EasySerializer.DeserializeFromJson<long>(json);

            // Assert
            Assert.AreEqual(4503599627370496, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative long (int64) produces the original value.
        /// Note: Uses value within double's precise integer range (±2^53).
        /// </summary>
        [Test]
        public void SerializeDeserialize_LongNegative_ReturnsOriginalValue()
        {
            // Arrange
            // Use -2^52 which is within double's precise integer range
            long original = -4503599627370496;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            long result = EasySerializer.DeserializeFromJson<long>(json);

            // Assert
            Assert.AreEqual(-4503599627370496, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a byte (uint8) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Byte_ReturnsOriginalValue()
        {
            // Arrange
            byte original = 255;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            byte result = EasySerializer.DeserializeFromJson<byte>(json);

            // Assert
            Assert.AreEqual(255, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a ushort (uint16) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UShort_ReturnsOriginalValue()
        {
            // Arrange
            ushort original = 65535;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            ushort result = EasySerializer.DeserializeFromJson<ushort>(json);

            // Assert
            Assert.AreEqual(65535, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a uint (uint32) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UInt_ReturnsOriginalValue()
        {
            // Arrange
            uint original = 4294967295;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            uint result = EasySerializer.DeserializeFromJson<uint>(json);

            // Assert
            Assert.AreEqual(4294967295u, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a ulong (uint64) produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ULong_ReturnsOriginalValue()
        {
            // Arrange
            ulong original = 90071992547409901;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            ulong result = EasySerializer.DeserializeFromJson<ulong>(json);

            // Assert
            Assert.AreEqual(90071992547409901ul, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a decimal produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Decimal_ReturnsOriginalValue()
        {
            // Arrange
            decimal original = 123.456789m;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            decimal result = EasySerializer.DeserializeFromJson<decimal>(json);

            // Assert
            Assert.AreEqual(123.456789m, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative decimal produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NegativeDecimal_ReturnsOriginalValue()
        {
            // Arrange
            decimal original = -99999.9999m;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            decimal result = EasySerializer.DeserializeFromJson<decimal>(json);

            // Assert
            Assert.AreEqual(-99999.9999m, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a very large decimal produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_LargeDecimal_ReturnsOriginalValue()
        {
            // Arrange
            decimal original = 79228162514264337593543950335m;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            decimal result = EasySerializer.DeserializeFromJson<decimal>(json);

            // Assert
            Assert.AreEqual(79228162514264337593543950335m, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a very small negative decimal produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_SmallDecimal_ReturnsOriginalValue()
        {
            // Arrange
            decimal original = -79228162514264337593543950335m;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            decimal result = EasySerializer.DeserializeFromJson<decimal>(json);

            // Assert
            Assert.AreEqual(-79228162514264337593543950335m, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing decimal with high precision produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_HighPrecisionDecimal_ReturnsOriginalValue()
        {
            // Arrange
            decimal original = 0.0000000000000000000000000001m;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            decimal result = EasySerializer.DeserializeFromJson<decimal>(json);

            // Assert
            Assert.AreEqual(0.0000000000000000000000000001m, result);
        }

        #endregion

        #region Nullable Types

        /// <summary>
        /// Verifies that serializing and deserializing a nullable int with a value produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullableIntWithValue_ReturnsOriginalValue()
        {
            // Arrange
            int? original = 42;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int? result = EasySerializer.DeserializeFromJson<int?>(json);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(42, result.Value);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a nullable int with null produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullableIntWithNull_ReturnsNull()
        {
            // Arrange
            int? original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int? result = EasySerializer.DeserializeFromJson<int?>(json);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a nullable DateTime with a value produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullableDateTimeWithValue_ReturnsOriginalValue()
        {
            // Arrange
            DateTime? original = new DateTime(2025, 3, 16, 14, 30, 45, DateTimeKind.Local);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            DateTime? result = EasySerializer.DeserializeFromJson<DateTime?>(json);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(new DateTime(2025, 3, 16, 14, 30, 45, DateTimeKind.Local), result.Value);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a nullable DateTime with null produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullableDateTimeWithNull_ReturnsNull()
        {
            // Arrange
            DateTime? original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            DateTime? result = EasySerializer.DeserializeFromJson<DateTime?>(json);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a nullable Guid with a value produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullableGuidWithValue_ReturnsOriginalValue()
        {
            // Arrange
            Guid? original = new Guid("12345678-1234-1234-1234-123456789abc");

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Guid? result = EasySerializer.DeserializeFromJson<Guid?>(json);

            // Assert
            Assert.IsTrue(result.HasValue);
            Assert.AreEqual(new Guid("12345678-1234-1234-1234-123456789abc"), result.Value);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a nullable Guid with null produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullableGuidWithNull_ReturnsNull()
        {
            // Arrange
            Guid? original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Guid? result = EasySerializer.DeserializeFromJson<Guid?>(json);

            // Assert
            Assert.IsFalse(result.HasValue);
        }

        #endregion

        #region String

        /// <summary>
        /// Verifies that serializing and deserializing a string produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_String_ReturnsOriginalValue()
        {
            // Arrange
            string original = "Hello, World!";

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            string result = EasySerializer.DeserializeFromJson<string>(json);

            // Assert
            Assert.AreEqual("Hello, World!", result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty string produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyString_ReturnsOriginalValue()
        {
            // Arrange
            string original = string.Empty;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            string result = EasySerializer.DeserializeFromJson<string>(json);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null string produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullString_ReturnsNull()
        {
            // Arrange
            string original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            string result = EasySerializer.DeserializeFromJson<string>(json);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a string with unicode characters produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UnicodeString_ReturnsOriginalValue()
        {
            // Arrange
            string original = "Hello 世界! 🌍";

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            string result = EasySerializer.DeserializeFromJson<string>(json);

            // Assert
            Assert.AreEqual("Hello 世界! 🌍", result);
        }

        #endregion

        #region Common Types

        /// <summary>
        /// Verifies that serializing and deserializing a Guid produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Guid_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Guid("12345678-1234-1234-1234-123456789abc");

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<Guid>(json);

            // Assert
            Assert.AreEqual(new Guid("12345678-1234-1234-1234-123456789abc"), result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty Guid produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyGuid_ReturnsOriginalValue()
        {
            // Arrange
            var original = Guid.Empty;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<Guid>(json);

            // Assert
            Assert.AreEqual(Guid.Empty, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a TimeSpan produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_TimeSpan_ReturnsOriginalValue()
        {
            // Arrange
            var original = TimeSpan.FromHours(2.5);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TimeSpan>(json);

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(2.5), result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a negative TimeSpan produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NegativeTimeSpan_ReturnsOriginalValue()
        {
            // Arrange
            var original = TimeSpan.FromSeconds(-30);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TimeSpan>(json);

            // Assert
            Assert.AreEqual(TimeSpan.FromSeconds(-30), result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a maximum TimeSpan produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_MaxTimeSpan_ReturnsOriginalValue()
        {
            // Arrange
            var original = TimeSpan.MaxValue;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TimeSpan>(json);

            // Assert
            Assert.AreEqual(TimeSpan.MaxValue, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a DateTime produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_DateTime_ReturnsOriginalValue()
        {
            // Arrange
            var original = new DateTime(2025, 3, 16, 14, 30, 45, DateTimeKind.Local);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<DateTime>(json);

            // Assert
            Assert.AreEqual(new DateTime(2025, 3, 16, 14, 30, 45, DateTimeKind.Local), result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a UTC DateTime produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_UtcDateTime_ReturnsOriginalValue()
        {
            // Arrange
            var original = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<DateTime>(json);

            // Assert
            Assert.AreEqual(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing MinValue DateTime produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_MinValueDateTime_ReturnsOriginalValue()
        {
            // Arrange
            var original = DateTime.MinValue;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<DateTime>(json);

            // Assert
            Assert.AreEqual(DateTime.MinValue, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing MaxValue DateTime produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_MaxValueDateTime_ReturnsOriginalValue()
        {
            // Arrange
            var original = DateTime.MaxValue;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<DateTime>(json);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, result);
        }

        #endregion

        #region Enum

        /// <summary>
        /// Verifies that serializing and deserializing an enum produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EnumOptionA_ReturnsOriginalValue()
        {
            // Arrange
            TestEnum original = TestEnum.OptionA;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            TestEnum result = EasySerializer.DeserializeFromJson<TestEnum>(json);

            // Assert
            Assert.AreEqual(TestEnum.OptionA, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an enum with a non-zero value produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EnumOptionC_ReturnsOriginalValue()
        {
            // Arrange
            TestEnum original = TestEnum.OptionC;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            TestEnum result = EasySerializer.DeserializeFromJson<TestEnum>(json);

            // Assert
            Assert.AreEqual(TestEnum.OptionC, result);
        }

        #endregion

        #region Unity Types

        /// <summary>
        /// Verifies that serializing and deserializing a Vector3 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector3_ReturnsOriginalValue()
        {
            // Arrange
            var original = new UnityEngine.Vector3(1.5f, 2.5f, 3.5f);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<UnityEngine.Vector3>(json);

            // Assert
            Assert.AreEqual(1.5f, result.x, 0.00001f);
            Assert.AreEqual(2.5f, result.y, 0.00001f);
            Assert.AreEqual(3.5f, result.z, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector2 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector2_ReturnsOriginalValue()
        {
            // Arrange
            var original = new UnityEngine.Vector2(10.5f, -5.25f);

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<UnityEngine.Vector2>(json);

            // Assert
            Assert.AreEqual(10.5f, result.x, 0.00001f);
            Assert.AreEqual(-5.25f, result.y, 0.00001f);
        }

        #endregion

        #region Arrays

        /// <summary>
        /// Verifies that serializing and deserializing an int array produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_IntArray_ReturnsOriginalValue()
        {
            // Arrange
            int[] original = { 1, 2, 3, 4, 5 };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int[] result = EasySerializer.DeserializeFromJson<int[]>(json);

            // Assert
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(4, result[3]);
            Assert.AreEqual(5, result[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an bool array produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BoolArray_ReturnsOriginalValue()
        {
            // Arrange
            bool[] original = { true, false, true, false, true };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            bool[] result = EasySerializer.DeserializeFromJson<bool[]>(json);

            // Assert
            Assert.AreEqual(5, result.Length);
            Assert.AreEqual(true, result[0]);
            Assert.AreEqual(false, result[1]);
            Assert.AreEqual(true, result[2]);
            Assert.AreEqual(false, result[3]);
            Assert.AreEqual(true, result[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty array produces an empty array.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyArray_ReturnsEmptyArray()
        {
            // Arrange
            int[] original = Array.Empty<int>();

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int[] result = EasySerializer.DeserializeFromJson<int[]>(json);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Length);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null array produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullArray_ReturnsNull()
        {
            // Arrange
            int[] original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            int[] result = EasySerializer.DeserializeFromJson<int[]>(json);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Lists

        /// <summary>
        /// Verifies that serializing and deserializing a list of integers produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_IntList_ReturnsOriginalValue()
        {
            // Arrange
            var original = new List<int> { 10, 20, 30, 40, 50 };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            List<int> result = EasySerializer.DeserializeFromJson<List<int>>(json);

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(10, result[0]);
            Assert.AreEqual(20, result[1]);
            Assert.AreEqual(30, result[2]);
            Assert.AreEqual(40, result[3]);
            Assert.AreEqual(50, result[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty list produces null.
        /// Empty lists are treated as null during deserialization.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyList_ReturnsNull_EmptyListBecomesNull()
        {
            // Arrange
            var original = new List<int>();

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            List<int> result = EasySerializer.DeserializeFromJson<List<int>>(json);

            // Assert
            Assert.IsNull(result, "Empty lists should deserialize to null");
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null list produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullList_ReturnsNull()
        {
            // Arrange
            List<int> original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            List<int> result = EasySerializer.DeserializeFromJson<List<int>>(json);

            // Assert
            Assert.IsNull(result);
        }

        #endregion

        #region Complex Objects

        /// <summary>
        /// Verifies that serializing and deserializing a complex object produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ComplexObject_ReturnsOriginalValue()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 100,
                Name = "TestPlayer",
                Health = 75.5f,
                IsActive = true,
                Position = new UnityEngine.Vector3(1, 2, 3),
                Scores = new List<int> { 100, 200, 300 },
                Data = new byte[] { 11, 22, 33, 44, 55 }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TestDataClass>(json);

            // Assert
            Assert.AreEqual(100, result.Id);
            Assert.AreEqual("TestPlayer", result.Name);
            Assert.AreEqual(75.5f, result.Health, 0.0001f);
            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(1, result.Position.x, 0.00001f);
            Assert.AreEqual(2, result.Position.y, 0.00001f);
            Assert.AreEqual(3, result.Position.z, 0.00001f);
            Assert.AreEqual(3, result.Scores.Count);
            Assert.AreEqual(100, result.Scores[0]);
            Assert.AreEqual(200, result.Scores[1]);
            Assert.AreEqual(300, result.Scores[2]);
            Assert.AreEqual(11, result.Data[0]);
            Assert.AreEqual(22, result.Data[1]);
            Assert.AreEqual(33, result.Data[2]);
            Assert.AreEqual(44, result.Data[3]);
            Assert.AreEqual(55, result.Data[4]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a complex object with null list produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ComplexObjectWithNullList_ReturnsOriginalValue()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 1,
                Name = "NullScores",
                Health = 100f,
                IsActive = false,
                Position = UnityEngine.Vector3.zero,
                Scores = null,
                Data = null
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TestDataClass>(json);

            // Assert
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("NullScores", result.Name);
            Assert.AreEqual(100f, result.Health, 0.0001f);
            Assert.IsFalse(result.IsActive);
            Assert.AreEqual(UnityEngine.Vector3.zero, result.Position);
            Assert.IsNull(result.Scores);
            Assert.AreEqual(null, result.Data);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a complex object with nullable fields having values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ComplexObjectWithNullableValues_ReturnsOriginalValue()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 200,
                Name = "NullableTest",
                Health = 50.5f,
                IsActive = true,
                Position = new UnityEngine.Vector3(5, 10, 15),
                Scores = new List<int> { 10, 20 },
                Data = new byte[] { 1, 2 },
                OptionalId = 999,
                OptionalHealth = 99.9f,
                OptionalIsActive = false,
                OptionalTimestamp = new DateTime(2025, 3, 17, 12, 30, 45, DateTimeKind.Local),
                OptionalGuid = new Guid("abcdef01-1234-5678-90ab-cdef12345678")
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TestDataClass>(json);

            // Assert
            Assert.AreEqual(200, result.Id);
            Assert.AreEqual("NullableTest", result.Name);
            Assert.AreEqual(50.5f, result.Health, 0.0001f);
            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(5, result.Position.x, 0.00001f);
            Assert.AreEqual(10, result.Position.y, 0.00001f);
            Assert.AreEqual(15, result.Position.z, 0.00001f);
            Assert.AreEqual(2, result.Scores.Count);
            Assert.AreEqual(10, result.Scores[0]);
            Assert.AreEqual(20, result.Scores[1]);
            Assert.AreEqual(1, result.Data[0]);
            Assert.AreEqual(2, result.Data[1]);

            // Nullable field assertions
            Assert.IsTrue(result.OptionalId.HasValue);
            Assert.AreEqual(999, result.OptionalId.Value);
            Assert.IsTrue(result.OptionalHealth.HasValue);
            Assert.AreEqual(99.9f, result.OptionalHealth.Value, 0.0001f);
            Assert.IsTrue(result.OptionalIsActive.HasValue);
            Assert.IsFalse(result.OptionalIsActive.Value);
            Assert.IsTrue(result.OptionalTimestamp.HasValue);
            Assert.AreEqual(new DateTime(2025, 3, 17, 12, 30, 45, DateTimeKind.Local), result.OptionalTimestamp.Value);
            Assert.IsTrue(result.OptionalGuid.HasValue);
            Assert.AreEqual(new Guid("abcdef01-1234-5678-90ab-cdef12345678"), result.OptionalGuid.Value);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a complex object with nullable fields being null produces null values.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ComplexObjectWithNullableNulls_ReturnsNullValues()
        {
            // Arrange
            var original = new TestDataClass
            {
                Id = 300,
                Name = "NullableNullTest",
                Health = 25.0f,
                IsActive = false,
                Position = UnityEngine.Vector3.one,
                Scores = null,
                Data = null,
                OptionalId = null,
                OptionalHealth = null,
                OptionalIsActive = null,
                OptionalTimestamp = null,
                OptionalGuid = null
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            var result = EasySerializer.DeserializeFromJson<TestDataClass>(json);

            // Assert
            Assert.AreEqual(300, result.Id);
            Assert.AreEqual("NullableNullTest", result.Name);
            Assert.AreEqual(25.0f, result.Health, 0.0001f);
            Assert.IsFalse(result.IsActive);
            Assert.AreEqual(UnityEngine.Vector3.one, result.Position);
            Assert.IsNull(result.Scores);
            Assert.IsNull(result.Data);

            // Nullable field null assertions
            Assert.IsFalse(result.OptionalId.HasValue);
            Assert.IsFalse(result.OptionalHealth.HasValue);
            Assert.IsFalse(result.OptionalIsActive.HasValue);
            Assert.IsFalse(result.OptionalTimestamp.HasValue);
            Assert.IsFalse(result.OptionalGuid.HasValue);
        }

        #endregion

        #region Dictionary

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with int keys and string values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_IntStringDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<int, string>
            {
                { 1, "One" },
                { 2, "Two" },
                { 3, "Three" }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, string> result = EasySerializer.DeserializeFromJson<Dictionary<int, string>>(json);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("One", result[1]);
            Assert.AreEqual("Two", result[2]);
            Assert.AreEqual("Three", result[3]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with string keys and int values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_StringIntDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<string, int>
            {
                { "Apple", 1 },
                { "Banana", 2 },
                { "Cherry", 3 }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<string, int> result = EasySerializer.DeserializeFromJson<Dictionary<string, int>>(json);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1, result["Apple"]);
            Assert.AreEqual(2, result["Banana"]);
            Assert.AreEqual(3, result["Cherry"]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an empty dictionary produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EmptyDictionary_ReturnsNull()
        {
            // Arrange
            var original = new Dictionary<int, string>();

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, string> result = EasySerializer.DeserializeFromJson<Dictionary<int, string>>(json);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a null dictionary produces null.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullDictionary_ReturnsNull()
        {
            // Arrange
            Dictionary<int, string> original = null;

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, string> result = EasySerializer.DeserializeFromJson<Dictionary<int, string>>(json);

            // Assert
            Assert.IsNull(result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with enum keys produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EnumKeyDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<TestEnum, string>
            {
                { TestEnum.OptionA, "A" },
                { TestEnum.OptionB, "B" },
                { TestEnum.OptionC, "C" }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<TestEnum, string> result = EasySerializer.DeserializeFromJson<Dictionary<TestEnum, string>>(json);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("A", result[TestEnum.OptionA]);
            Assert.AreEqual("B", result[TestEnum.OptionB]);
            Assert.AreEqual("C", result[TestEnum.OptionC]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with enum values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_EnumValueDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<string, TestEnum>
            {
                { "First", TestEnum.OptionA },
                { "Second", TestEnum.OptionB },
                { "Third", TestEnum.OptionC }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<string, TestEnum> result = EasySerializer.DeserializeFromJson<Dictionary<string, TestEnum>>(json);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(TestEnum.OptionA, result["First"]);
            Assert.AreEqual(TestEnum.OptionB, result["Second"]);
            Assert.AreEqual(TestEnum.OptionC, result["Third"]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with float values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_FloatValueDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<int, float>
            {
                { 1, 1.1f },
                { 2, 2.2f },
                { 3, 3.3f }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, float> result = EasySerializer.DeserializeFromJson<Dictionary<int, float>>(json);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(1.1f, result[1], 0.0001f);
            Assert.AreEqual(2.2f, result[2], 0.0001f);
            Assert.AreEqual(3.3f, result[3], 0.0001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with List values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ListValueDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<string, List<int>>
            {
                { "GroupA", new List<int> { 1, 2, 3 } },
                { "GroupB", new List<int> { 4, 5, 6 } }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<string, List<int>> result = EasySerializer.DeserializeFromJson<Dictionary<string, List<int>>>(json);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result["GroupA"].Count);
            Assert.AreEqual(1, result["GroupA"][0]);
            Assert.AreEqual(2, result["GroupA"][1]);
            Assert.AreEqual(3, result["GroupA"][2]);
            Assert.AreEqual(3, result["GroupB"].Count);
            Assert.AreEqual(4, result["GroupB"][0]);
            Assert.AreEqual(5, result["GroupB"][1]);
            Assert.AreEqual(6, result["GroupB"][2]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a dictionary with array values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ArrayValueDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<int, int[]>
            {
                { 1, new[] { 10, 20, 30 } },
                { 2, new[] { 40, 50, 60 } }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, int[]> result = EasySerializer.DeserializeFromJson<Dictionary<int, int[]>>(json);

            // Assert
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result[1].Length);
            Assert.AreEqual(10, result[1][0]);
            Assert.AreEqual(20, result[1][1]);
            Assert.AreEqual(30, result[1][2]);
            Assert.AreEqual(3, result[2].Length);
            Assert.AreEqual(40, result[2][0]);
            Assert.AreEqual(50, result[2][1]);
            Assert.AreEqual(60, result[2][2]);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a large dictionary produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_LargeDictionary_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Dictionary<int, string>();
            for (int i = 0; i < 1000; i++)
            {
                original[i] = $"Value{i}";
            }

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, string> result = EasySerializer.DeserializeFromJson<Dictionary<int, string>>(json);

            // Assert
            Assert.AreEqual(1000, result.Count);
            Assert.AreEqual("Value0", result[0]);
            Assert.AreEqual("Value500", result[500]);
            Assert.AreEqual("Value999", result[999]);
        }

        /// <summary>
        /// Verifies that dictionary preserves all key-value pairs correctly through serialization.
        /// </summary>
        [Test]
        public void SerializeDeserialize_DictionaryWithDuplicateValues_PreservesAllKeys()
        {
            // Arrange
            var original = new Dictionary<int, string>
            {
                { 1, "Same" },
                { 2, "Same" },
                { 3, "Same" }
            };

            // Act
            string json = EasySerializer.SerializeToJson(ref original);
            Dictionary<int, string> result = EasySerializer.DeserializeFromJson<Dictionary<int, string>>(json);

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.ContainsKey(1));
            Assert.IsTrue(result.ContainsKey(2));
            Assert.IsTrue(result.ContainsKey(3));
            Assert.AreEqual("Same", result[1]);
            Assert.AreEqual("Same", result[2]);
            Assert.AreEqual("Same", result[3]);
        }

        #endregion

        #region Runtime Type Serialization

        /// <summary>
        /// Verifies that serializing a derived class through base class reference preserves runtime type when UseRuntimeTypeSerialization is enabled.
        /// </summary>
        [Test]
        public void SerializeDeserialize_DerivedClassWithRuntimeTypeEnabled_PreservesRuntimeType()
        {
            // Arrange
            var context = new SerializationContext
            {
                UseRuntimeTypeSerialization = true,
                AllowAnonymousTypes = true
            };
            var original = new PetContainer
            {
                Pet = new Dog("Buddy", "Golden Retriever")
            };

            // Act
            string json = EasySerializer.SerializeToJson(original, null, context);
            var result = EasySerializer.DeserializeFromJson<PetContainer>(json, null, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Pet);
            Assert.AreEqual("Buddy", result.Pet.Name);
            Assert.IsInstanceOf(typeof(Dog), result.Pet);
            var dog = result.Pet as Dog;
            Assert.IsNotNull(dog);
            Assert.AreEqual("Golden Retriever", dog.Breed);
        }

        /// <summary>
        /// Verifies that serializing a derived class through base class reference loses derived type information when UseRuntimeTypeSerialization is disabled.
        /// </summary>
        [Test]
        public void SerializeDeserialize_DerivedClassWithRuntimeTypeDisabled_LosesDerivedTypeInfo()
        {
            // Arrange
            var context = new SerializationContext
            {
                UseRuntimeTypeSerialization = false
            };
            var original = new PetContainer
            {
                Pet = new Dog("Max", "Bulldog")
            };

            // Act
            string json = EasySerializer.SerializeToJson(original, null, context);
            var result = EasySerializer.DeserializeFromJson<PetContainer>(json, null, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Pet);
            Assert.AreEqual("Max", result.Pet.Name);
            // Without runtime type serialization, should deserialize as base type
            Assert.IsInstanceOf(typeof(Animal), result.Pet);
            Assert.IsNotInstanceOf(typeof(Dog), result.Pet);
        }

        /// <summary>
        /// Verifies that serializing a Cat derived class through base class reference preserves runtime type when UseRuntimeTypeSerialization is enabled.
        /// </summary>
        [Test]
        public void SerializeDeserialize_CatDerivedClassWithRuntimeTypeEnabled_PreservesRuntimeType()
        {
            // Arrange
            var context = new SerializationContext
            {
                UseRuntimeTypeSerialization = true,
                AllowAnonymousTypes = true
            };
            var original = new PetContainer
            {
                Pet = new Cat("Whiskers", true)
            };

            // Act
            string json = EasySerializer.SerializeToJson(original, null, context);
            var result = EasySerializer.DeserializeFromJson<PetContainer>(json, null, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Pet);
            Assert.AreEqual("Whiskers", result.Pet.Name);
            Assert.IsInstanceOf(typeof(Cat), result.Pet);
            var cat = result.Pet as Cat;
            Assert.IsNotNull(cat);
            Assert.IsTrue(cat.IsIndoor);
        }

        /// <summary>
        /// Verifies that serializing multiple different derived types through base class references preserves runtime types when UseRuntimeTypeSerialization is enabled.
        /// </summary>
        [Test]
        public void SerializeDeserialize_MultipleDerivedClassesWithRuntimeTypeEnabled_PreservesAllRuntimeTypes()
        {
            // Arrange
            var context = new SerializationContext
            {
                UseRuntimeTypeSerialization = true,
                AllowAnonymousTypes = true
            };
            var original = new ZooContainer
            {
                PrimaryAnimal = new Dog("Rex", "German Shepherd"),
                SecondaryAnimal = new Cat("Mittens", false)
            };

            // Act
            string json = EasySerializer.SerializeToJson(original, null, context);
            var result = EasySerializer.DeserializeFromJson<ZooContainer>(json, null, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.PrimaryAnimal);
            Assert.IsNotNull(result.SecondaryAnimal);

            // Verify primary animal is Dog
            Assert.AreEqual("Rex", result.PrimaryAnimal.Name);
            Assert.IsInstanceOf(typeof(Dog), result.PrimaryAnimal);
            var dog = result.PrimaryAnimal as Dog;
            Assert.IsNotNull(dog);
            Assert.AreEqual("German Shepherd", dog.Breed);

            // Verify secondary animal is Cat
            Assert.AreEqual("Mittens", result.SecondaryAnimal.Name);
            Assert.IsInstanceOf(typeof(Cat), result.SecondaryAnimal);
            var cat = result.SecondaryAnimal as Cat;
            Assert.IsNotNull(cat);
            Assert.IsFalse(cat.IsIndoor);
        }

        /// <summary>
        /// Verifies that serializing base class instance through base class reference works correctly when UseRuntimeTypeSerialization is enabled.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BaseClassWithRuntimeTypeEnabled_WorksCorrectly()
        {
            // Arrange
            var context = new SerializationContext
            {
                UseRuntimeTypeSerialization = true,
                AllowAnonymousTypes = true
            };
            var original = new PetContainer
            {
                Pet = new Animal("GenericAnimal")
            };

            // Act
            string json = EasySerializer.SerializeToJson(original, null, context);
            var result = EasySerializer.DeserializeFromJson<PetContainer>(json, null, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Pet);
            Assert.AreEqual("GenericAnimal", result.Pet.Name);
            Assert.IsInstanceOf(typeof(Animal), result.Pet);
            Assert.IsNotInstanceOf(typeof(Dog), result.Pet);
            Assert.IsNotInstanceOf(typeof(Cat), result.Pet);
        }

        /// <summary>
        /// Verifies that serializing a null reference through base class reference works correctly when UseRuntimeTypeSerialization is enabled.
        /// </summary>
        [Test]
        public void SerializeDeserialize_NullReferenceWithRuntimeTypeEnabled_WorksCorrectly()
        {
            // Arrange
            var context = new SerializationContext
            {
                UseRuntimeTypeSerialization = true,
                AllowAnonymousTypes = true
            };
            var original = new PetContainer
            {
                Pet = null
            };

            // Act
            string json = EasySerializer.SerializeToJson(original, null, context);
            var result = EasySerializer.DeserializeFromJson<PetContainer>(json, null, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Pet);
        }

        #endregion

        #region JsonFormatterSettings Options

        /// <summary>
        /// Verifies that serializing with IncludeObjectType option includes type metadata in JSON.
        /// </summary>
        [Test]
        public void SerializeWithOptions_IncludeObjectType_ContainsTypeField()
        {
            // Arrange
            var settings = new SerializationSettings
            {
                JsonFormatterSettings = new Formatters.JsonFormatterSettings
                {
                    Options = Formatters.JsonFormatterOptions.IncludeObjectType
                }
            };
            var context = new SerializationContext { AllowAnonymousTypes = true };
            var original = new TestDataClass { Id = 42, Name = "Test" };

            // Act
            string json = EasySerializer.SerializeToJson(original, settings, context);

            // Assert
            Assert.IsTrue(json.Contains("\"__meta_type__\""));
            Assert.IsTrue(json.Contains(typeof(TestDataClass).FullName));
        }

        /// <summary>
        /// Verifies that serializing with None option excludes type metadata from JSON.
        /// </summary>
        [Test]
        public void SerializeWithOptions_None_DoesNotContainTypeField()
        {
            // Arrange
            var settings = new SerializationSettings
            {
                JsonFormatterSettings = new Formatters.JsonFormatterSettings
                {
                    Options = Formatters.JsonFormatterOptions.None
                }
            };
            var context = new SerializationContext { AllowAnonymousTypes = true };
            var original = new TestDataClass { Id = 42, Name = "Test" };

            // Act
            string json = EasySerializer.SerializeToJson(original, settings, context);

            // Assert
            Assert.IsFalse(json.Contains("\"__meta_type__\""));
        }

        #endregion

        #region JsonFormatterSettings TypeNameField

        /// <summary>
        /// Verifies that custom TypeNameField is used in JSON output when IncludeObjectType is enabled.
        /// </summary>
        [Test]
        public void SerializeWithCustomTypeNameField_UsesCustomFieldName()
        {
            // Arrange
            var settings = new SerializationSettings
            {
                JsonFormatterSettings = new Formatters.JsonFormatterSettings
                {
                    Options = Formatters.JsonFormatterOptions.IncludeObjectType,
                    TypeNameField = "$type$"
                }
            };
            var context = new SerializationContext { AllowAnonymousTypes = true };
            var original = new TestDataClass { Id = 42, Name = "Test" };

            // Act
            string json = EasySerializer.SerializeToJson(original, settings, context);

            // Assert
            Assert.IsTrue(json.Contains("\"$type$\""));
            Assert.IsFalse(json.Contains("\"__meta_type__\""));
        }

        /// <summary>
        /// Verifies that serialization and deserialization work correctly with custom TypeNameField.
        /// </summary>
        [Test]
        public void SerializeDeserializeWithCustomTypeNameField_ReturnsOriginalValue()
        {
            // Arrange
            var settings = new SerializationSettings
            {
                JsonFormatterSettings = new Formatters.JsonFormatterSettings
                {
                    Options = Formatters.JsonFormatterOptions.IncludeObjectType,
                    TypeNameField = "@custom_type@"
                }
            };
            var context = new SerializationContext { AllowAnonymousTypes = true, UseRuntimeTypeSerialization = true };
            var original = new PetContainer { Pet = new Dog("Buddy", "Golden Retriever") };

            // Act
            string json = EasySerializer.SerializeToJson(original, settings, context);
            var result = EasySerializer.DeserializeFromJson<PetContainer>(json, settings, context);

            // Assert
            Assert.IsTrue(json.Contains("\"@custom_type@\""));
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Pet);
            Assert.AreEqual("Buddy", result.Pet.Name);
            Assert.IsInstanceOf<Dog>(result.Pet);
            var dog = result.Pet as Dog;
            Assert.AreEqual("Golden Retriever", dog.Breed);
        }

        #endregion
    }
}
