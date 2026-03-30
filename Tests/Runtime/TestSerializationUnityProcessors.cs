using NUnit.Framework;
using EasyToolkit.Serialization;
using UnityEngine;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>
    /// Unit tests for Unity type serialization processors.
    /// </summary>
    [TestFixture]
    public class TestSerializationUnityProcessors
    {
        #region Vector Types

        /// <summary>
        /// Verifies that serializing and deserializing a Vector2 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector2_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector2(10.5f, -5.25f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector2>(data);

            // Assert
            Assert.AreEqual(10.5f, result.x, 0.00001f);
            Assert.AreEqual(-5.25f, result.y, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector2 with zero values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector2Zero_ReturnsOriginalValue()
        {
            // Arrange
            var original = Vector2.zero;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector2>(data);

            // Assert
            Assert.AreEqual(Vector2.zero, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector3 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector3_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector3(1.5f, 2.5f, 3.5f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector3>(data);

            // Assert
            Assert.AreEqual(1.5f, result.x, 0.00001f);
            Assert.AreEqual(2.5f, result.y, 0.00001f);
            Assert.AreEqual(3.5f, result.z, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector3 with one produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector3One_ReturnsOriginalValue()
        {
            // Arrange
            var original = Vector3.one;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector3>(data);

            // Assert
            Assert.AreEqual(Vector3.one, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector4 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector4_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector4(1.0f, 2.0f, 3.0f, 4.0f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector4>(data);

            // Assert
            Assert.AreEqual(1.0f, result.x, 0.00001f);
            Assert.AreEqual(2.0f, result.y, 0.00001f);
            Assert.AreEqual(3.0f, result.z, 0.00001f);
            Assert.AreEqual(4.0f, result.w, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector4 with negative values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector4Negative_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector4(-1.5f, -2.5f, -3.5f, -4.5f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector4>(data);

            // Assert
            Assert.AreEqual(-1.5f, result.x, 0.00001f);
            Assert.AreEqual(-2.5f, result.y, 0.00001f);
            Assert.AreEqual(-3.5f, result.z, 0.00001f);
            Assert.AreEqual(-4.5f, result.w, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector2Int produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector2Int_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector2Int(100, -200);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector2Int>(data);

            // Assert
            Assert.AreEqual(100, result.x);
            Assert.AreEqual(-200, result.y);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector2Int with zero values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector2IntZero_ReturnsOriginalValue()
        {
            // Arrange
            var original = Vector2Int.zero;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector2Int>(data);

            // Assert
            Assert.AreEqual(Vector2Int.zero, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector3Int produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector3Int_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector3Int(10, 20, 30);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector3Int>(data);

            // Assert
            Assert.AreEqual(10, result.x);
            Assert.AreEqual(20, result.y);
            Assert.AreEqual(30, result.z);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Vector3Int with negative values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Vector3IntNegative_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Vector3Int(-10, -20, -30);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Vector3Int>(data);

            // Assert
            Assert.AreEqual(-10, result.x);
            Assert.AreEqual(-20, result.y);
            Assert.AreEqual(-30, result.z);
        }

        #endregion

        #region Quaternion

        /// <summary>
        /// Verifies that serializing and deserializing a Quaternion produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Quaternion_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Quaternion(0.1f, 0.2f, 0.3f, 0.4f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Quaternion>(data);

            // Assert
            Assert.AreEqual(0.1f, result.x, 0.00001f);
            Assert.AreEqual(0.2f, result.y, 0.00001f);
            Assert.AreEqual(0.3f, result.z, 0.00001f);
            Assert.AreEqual(0.4f, result.w, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an identity Quaternion produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_QuaternionIdentity_ReturnsOriginalValue()
        {
            // Arrange
            var original = Quaternion.identity;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Quaternion>(data);

            // Assert
            Assert.AreEqual(Quaternion.identity, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing an Euler angle rotation produces the correct value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_QuaternionEuler_ReturnsOriginalValue()
        {
            // Arrange
            var original = Quaternion.Euler(45, 90, 135);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Quaternion>(data);

            // Assert
            Assert.AreEqual(original.eulerAngles.x, result.eulerAngles.x, 0.0001f);
            Assert.AreEqual(original.eulerAngles.y, result.eulerAngles.y, 0.0001f);
            Assert.AreEqual(original.eulerAngles.z, result.eulerAngles.z, 0.0001f);
        }

        #endregion

        #region Color Types

        /// <summary>
        /// Verifies that serializing and deserializing a Color produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Color_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Color(0.5f, 0.7f, 0.9f, 1.0f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Color>(data);

            // Assert
            Assert.AreEqual(0.5f, result.r, 0.00001f);
            Assert.AreEqual(0.7f, result.g, 0.00001f);
            Assert.AreEqual(0.9f, result.b, 0.00001f);
            Assert.AreEqual(1.0f, result.a, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing red color produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_ColorRed_ReturnsOriginalValue()
        {
            // Arrange
            var original = Color.red;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Color>(data);

            // Assert
            Assert.AreEqual(Color.red, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Color32 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Color32_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Color32(128, 200, 255, 100);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Color32>(data);

            // Assert
            Assert.AreEqual(128, result.r);
            Assert.AreEqual(200, result.g);
            Assert.AreEqual(255, result.b);
            Assert.AreEqual(100, result.a);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Color32 with max values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Color32Max_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Color32(255, 255, 255, 255);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Color32>(data);

            // Assert
            Assert.AreEqual(255, result.r);
            Assert.AreEqual(255, result.g);
            Assert.AreEqual(255, result.b);
            Assert.AreEqual(255, result.a);
        }

        #endregion

        #region Rect Types

        /// <summary>
        /// Verifies that serializing and deserializing a Rect produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Rect_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Rect(10.5f, 20.5f, 100.5f, 200.5f);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Rect>(data);

            // Assert
            Assert.AreEqual(10.5f, result.x, 0.00001f);
            Assert.AreEqual(20.5f, result.y, 0.00001f);
            Assert.AreEqual(100.5f, result.width, 0.00001f);
            Assert.AreEqual(200.5f, result.height, 0.00001f);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Rect with zero values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_RectZero_ReturnsOriginalValue()
        {
            // Arrange
            var original = Rect.zero;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Rect>(data);

            // Assert
            Assert.AreEqual(Rect.zero, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a RectInt produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_RectInt_ReturnsOriginalValue()
        {
            // Arrange
            var original = new RectInt(10, 20, 100, 200);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<RectInt>(data);

            // Assert
            Assert.AreEqual(10, result.x);
            Assert.AreEqual(20, result.y);
            Assert.AreEqual(100, result.width);
            Assert.AreEqual(200, result.height);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a RectInt with negative values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_RectIntNegative_ReturnsOriginalValue()
        {
            // Arrange
            var original = new RectInt(-50, -100, 200, 300);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<RectInt>(data);

            // Assert
            Assert.AreEqual(-50, result.x);
            Assert.AreEqual(-100, result.y);
            Assert.AreEqual(200, result.width);
            Assert.AreEqual(300, result.height);
        }

        #endregion

        #region Bounds Types

        /// <summary>
        /// Verifies that serializing and deserializing a Bounds produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Bounds_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Bounds(new Vector3(1, 2, 3), new Vector3(10, 20, 30));

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Bounds>(data);

            // Assert
            Assert.AreEqual(new Vector3(1, 2, 3), result.center);
            Assert.AreEqual(new Vector3(10, 20, 30), result.size);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Bounds with zero size produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BoundsZeroSize_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Bounds(Vector3.zero, Vector3.zero);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Bounds>(data);

            // Assert
            Assert.AreEqual(Vector3.zero, result.center);
            Assert.AreEqual(Vector3.zero, result.size);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a BoundsInt produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BoundsInt_ReturnsOriginalValue()
        {
            // Arrange
            var original = new BoundsInt(new Vector3Int(5, 10, 15), new Vector3Int(20, 30, 40));

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<BoundsInt>(data);

            // Assert
            Assert.AreEqual(new Vector3Int(5, 10, 15), result.position);
            Assert.AreEqual(new Vector3Int(20, 30, 40), result.size);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a BoundsInt with negative position produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_BoundsIntNegativePosition_ReturnsOriginalValue()
        {
            // Arrange
            var original = new BoundsInt(new Vector3Int(-5, -10, -15), new Vector3Int(10, 20, 30));

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<BoundsInt>(data);

            // Assert
            Assert.AreEqual(new Vector3Int(-5, -10, -15), result.position);
            Assert.AreEqual(new Vector3Int(10, 20, 30), result.size);
        }

        #endregion

        #region Matrix4x4

        /// <summary>
        /// Verifies that serializing and deserializing an identity Matrix4x4 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Matrix4x4Identity_ReturnsOriginalValue()
        {
            // Arrange
            var original = Matrix4x4.identity;

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Matrix4x4>(data);

            // Assert
            Assert.AreEqual(Matrix4x4.identity, result);
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Matrix4x4 with custom values produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Matrix4x4_ReturnsOriginalValue()
        {
            // Arrange
            var original = Matrix4x4.Translate(new Vector3(1, 2, 3));

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Matrix4x4>(data);

            // Assert - Check all 16 elements
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Assert.AreEqual(original[row, col], result[row, col], 0.00001f,
                        $"Matrix element at [{row},{col}] should match");
                }
            }
        }

        /// <summary>
        /// Verifies that serializing and deserializing a zero Matrix4x4 produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Matrix4x4Zero_ReturnsOriginalValue()
        {
            // Arrange
            var original = new Matrix4x4();

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Matrix4x4>(data);

            // Assert - Check all 16 elements
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Assert.AreEqual(original[row, col], result[row, col], 0.00001f,
                        $"Matrix element at [{row},{col}] should match");
                }
            }
        }

        /// <summary>
        /// Verifies that serializing and deserializing a Matrix4x4 with rotation produces the original value.
        /// </summary>
        [Test]
        public void SerializeDeserialize_Matrix4x4Rotation_ReturnsOriginalValue()
        {
            // Arrange
            var rotation = Quaternion.Euler(45, 90, 135);
            var original = Matrix4x4.Rotate(rotation);

            // Act
            byte[] data = EasySerializer.SerializeToBinary(ref original);
            var result = EasySerializer.DeserializeFromBinary<Matrix4x4>(data);

            // Assert - Check all 16 elements
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    Assert.AreEqual(original[row, col], result[row, col], 0.0001f,
                        $"Matrix element at [{row},{col}] should match");
                }
            }
        }

        #endregion
    }
}
