using System;
using NUnit.Framework;
using EasyToolkit.Serialization;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>
    /// Unit tests for JSON serialization with custom SerializationContext.
    /// </summary>
    [TestFixture]
    public class TestJsonSerializerContext
    {
        #region MemberFlags

        /// <summary>
        /// Verifies that PublicFieldsOnly MemberFlags only serializes public fields.
        /// </summary>
        [Test]
        public void SerializeWithContext_PublicFieldsOnly_SerializesOnlyPublicFields()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.PublicFields
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field should NOT be serialized");
            Assert.AreEqual(0, result.PublicProperty, "Public property should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should NOT be serialized");
        }

        /// <summary>
        /// Verifies that PublicPropertiesOnly MemberFlags only serializes public properties.
        /// </summary>
        [Test]
        public void SerializeWithContext_PublicPropertiesOnly_SerializesOnlyPublicProperties()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.PublicProperties
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(0, result.publicField, "Public field should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field should NOT be serialized");
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should NOT be serialized");
        }

        /// <summary>
        /// Verifies that AllPublic MemberFlags serializes all public members (fields and properties).
        /// </summary>
        [Test]
        public void SerializeWithContext_AllPublic_SerializesAllPublicMembers()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllPublic
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field should NOT be serialized");
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should NOT be serialized");
        }

        /// <summary>
        /// Verifies that NonPublicFieldsOnly MemberFlags only serializes non-public fields with SerializeField.
        /// </summary>
        [Test]
        public void SerializeWithContext_NonPublicFieldsOnly_SerializesNonPublicFieldsWithSerializeField()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.NonPublicFields,
                RequireSerializeFieldOnNonPublic = true
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(0, result.publicField, "Public field should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field without SerializeField should NOT be serialized");
            Assert.AreEqual(0, result.PublicProperty, "Public property should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should NOT be serialized");
            Assert.AreEqual(5, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should be serialized");
        }

        /// <summary>
        /// Verifies that AllFields MemberFlags serializes all fields (public and non-public).
        /// </summary>
        [Test]
        public void SerializeWithContext_AllFields_SerializesAllFields()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllFields,
                RequireSerializeFieldOnNonPublic = false
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
            Assert.AreEqual(0, result.PublicProperty, "Public property should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateProperty(), "Private property should NOT be serialized");
            Assert.AreEqual(5, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should be serialized");
        }

        /// <summary>
        /// Verifies that AllProperties MemberFlags serializes all properties (public and non-public).
        /// </summary>
        [Test]
        public void SerializeWithContext_AllProperties_SerializesAllProperties()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllProperties
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(0, result.publicField, "Public field should NOT be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field should NOT be serialized");
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(4, result.GetPrivateProperty(), "Private property should be serialized");
            Assert.AreEqual(0, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should NOT be serialized");
        }

        /// <summary>
        /// Verifies that All MemberFlags serializes all members (fields and properties).
        /// </summary>
        [Test]
        public void SerializeWithContext_All_SerializesAllMembers()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.All,
                RequireSerializeFieldOnNonPublic = false
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
            Assert.AreEqual(3, result.PublicProperty, "Public property should be serialized");
            Assert.AreEqual(4, result.GetPrivateProperty(), "Private property should be serialized");
            Assert.AreEqual(5, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should be serialized");
        }

        #endregion

        #region RequireSerializeFieldOnNonPublic

        /// <summary>
        /// Verifies that RequireSerializeFieldOnNonPublic=true only serializes non-public fields with SerializeField.
        /// </summary>
        [Test]
        public void SerializeWithContext_RequireSerializeFieldTrue_OnlySerializeFieldsWithAttribute()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllFields,
                RequireSerializeFieldOnNonPublic = true
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field without SerializeField should NOT be serialized");
            Assert.AreEqual(5, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should be serialized");
        }

        /// <summary>
        /// Verifies that RequireSerializeFieldOnNonPublic=false serializes all non-public fields based on MemberFlags.
        /// </summary>
        [Test]
        public void SerializeWithContext_RequireSerializeFieldFalse_SerializesAllNonPublicFields()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllFields,
                RequireSerializeFieldOnNonPublic = false
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(2, result.GetPrivateField(), "Private field should be serialized");
            Assert.AreEqual(5, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should be serialized");
        }

        #endregion

        #region ExcludeNonSerialized

        /// <summary>
        /// Verifies that ExcludeNonSerialized=true excludes fields with NonSerialized attribute.
        /// </summary>
        [Test]
        public void SerializeWithContext_ExcludeNonSerializedTrue_ExcludesNonSerializedFields()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllFields,
                ExcludeNonSerialized = true
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.publicFieldWithNonSerialized, "Public field with NonSerialized should NOT be serialized");
        }

        /// <summary>
        /// Verifies that ExcludeNonSerialized=false includes fields with NonSerialized attribute.
        /// </summary>
        [Test]
        public void SerializeWithContext_ExcludeNonSerializedFalse_IncludesNonSerializedFields()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllFields,
                ExcludeNonSerialized = false
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: context);

            // Assert
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(6, result.publicFieldWithNonSerialized, "Public field with NonSerialized should be serialized");
        }

        #endregion

        #region Context Isolation

        /// <summary>
        /// Verifies that different context instances maintain independent caches.
        /// </summary>
        [Test]
        public void SerializeWithContext_DifferentContexts_IndependentBehavior()
        {
            // Arrange
            var context1 = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.PublicFields
            };
            var context2 = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.AllFields,
                RequireSerializeFieldOnNonPublic = false
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act
            string json1 = EasySerializer.SerializeToJson(ref original, context: context1);
            string json2 = EasySerializer.SerializeToJson(ref original, context: context2);
            var result1 = EasySerializer.DeserializeFromJson<ContextTestClass>(json1, context: context1);
            var result2 = EasySerializer.DeserializeFromJson<ContextTestClass>(json2, context: context2);

            // Assert
            // context1 should only serialize public fields
            Assert.AreEqual(1, result1.publicField, "context1: Public field should be serialized");
            Assert.AreEqual(0, result1.GetPrivateField(), "context1: Private field should NOT be serialized");

            // context2 should serialize all fields
            Assert.AreEqual(1, result2.publicField, "context2: Public field should be serialized");
            Assert.AreEqual(2, result2.GetPrivateField(), "context2: Private field should be serialized");
        }

        /// <summary>
        /// Verifies that modifying context settings clears the processor cache.
        /// </summary>
        [Test]
        public void SerializeWithContext_ModifyingContext_ClearsProcessorCache()
        {
            // Arrange
            var context = new SerializationContext
            {
                MemberFlags = SerializableMemberFlags.PublicFields
            };
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act - Serialize with PublicFieldsOnly
            string json1 = EasySerializer.SerializeToJson(ref original, context: context);
            var result1 = EasySerializer.DeserializeFromJson<ContextTestClass>(json1, context: context);

            // Modify context to AllFields
            context.MemberFlags = SerializableMemberFlags.AllFields;
            context.RequireSerializeFieldOnNonPublic = false;

            // Serialize again with new settings
            string json2 = EasySerializer.SerializeToJson(ref original, context: context);
            var result2 = EasySerializer.DeserializeFromJson<ContextTestClass>(json2, context: context);

            // Assert
            // First serialization should only have public fields
            Assert.AreEqual(1, result1.publicField, "First: Public field should be serialized");
            Assert.AreEqual(0, result1.GetPrivateField(), "First: Private field should NOT be serialized");

            // Second serialization should have all fields
            Assert.AreEqual(1, result2.publicField, "Second: Public field should be serialized");
            Assert.AreEqual(2, result2.GetPrivateField(), "Second: Private field should be serialized");
        }

        /// <summary>
        /// Verifies that Shared context works correctly with JSON serialization.
        /// </summary>
        [Test]
        public void SerializeWithContext_SharedContext_UsesDefaultSettings()
        {
            // Arrange
            var original = new ContextTestClass(1, 2, 3, 4, 5, 6);

            // Act - Use Shared context (default is AllFields with RequireSerializeFieldOnNonPublic=true)
            string json = EasySerializer.SerializeToJson(ref original, context: SerializationContext.Shared);
            var result = EasySerializer.DeserializeFromJson<ContextTestClass>(json, context: SerializationContext.Shared);

            // Assert
            // Default context should serialize all fields, but private fields need SerializeField
            Assert.AreEqual(1, result.publicField, "Public field should be serialized");
            Assert.AreEqual(0, result.GetPrivateField(), "Private field without SerializeField should NOT be serialized");
            Assert.AreEqual(5, result.GetPrivateFieldWithAttribute(), "Private field with SerializeField should be serialized");
            Assert.AreEqual(0, result.PublicProperty, "Properties should NOT be serialized with AllFields");
        }

        #endregion

        #region AllowUnmarkedStructs

        /// <summary>
        /// Verifies that AllowUnmarkedStructs=true allows serialization of unmarked structs.
        /// </summary>
        [Test]
        public void SerializeWithContext_AllowUnmarkedStructsTrue_SerializesUnmarkedStructs()
        {
            // Arrange
            var context = new SerializationContext
            {
                AllowUnmarkedStructs = true
            };
            var original = new StructContainerClass(
                new UnmarkedStruct(10, 20),
                new SerializableStruct(100, "test"),
                new EasySerializableStruct(3.5f, true)
            );

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<StructContainerClass>(json, context: context);

            // Assert - All structs should be serialized
            Assert.AreEqual(10, result.UnmarkedField.X, "Unmarked struct X should be serialized");
            Assert.AreEqual(20, result.UnmarkedField.Y, "Unmarked struct Y should be serialized");
            Assert.AreEqual(100, result.SerializableField.Value, "Serializable struct should be serialized");
            Assert.AreEqual("test", result.SerializableField.Name, "Serializable struct name should be serialized");
            Assert.AreEqual(3.5f, result.EasySerializableField.Score, 0.001f, "EasySerializable struct score should be serialized");
            Assert.IsTrue(result.EasySerializableField.IsActive, "EasySerializable struct IsActive should be serialized");
        }

        /// <summary>
        /// Verifies that AllowUnmarkedStructs=false prevents serialization of unmarked structs.
        /// </summary>
        [Test]
        public void SerializeWithContext_AllowUnmarkedStructsFalse_PreventsUnmarkedStructsSerialization()
        {
            // Arrange
            var context = new SerializationContext
            {
                AllowUnmarkedStructs = false
            };
            var original = new StructContainerClass(
                new UnmarkedStruct(10, 20),
                new SerializableStruct(100, "test"),
                new EasySerializableStruct(3.5f, true)
            );

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);
            var result = EasySerializer.DeserializeFromJson<StructContainerClass>(json, context: context);

            // Assert - Unmarked struct should use default values (not serialized)
            Assert.AreEqual(0, result.UnmarkedField.X, "Unmarked struct X should NOT be serialized (default value)");
            Assert.AreEqual(0, result.UnmarkedField.Y, "Unmarked struct Y should NOT be serialized (default value)");
            // Marked structs should still be serialized
            Assert.AreEqual(100, result.SerializableField.Value, "Serializable struct should be serialized");
            Assert.AreEqual("test", result.SerializableField.Name, "Serializable struct name should be serialized");
            Assert.AreEqual(3.5f, result.EasySerializableField.Score, 0.001f, "EasySerializable struct score should be serialized");
        }

        /// <summary>
        /// Verifies that modifying AllowUnmarkedStructs clears the processor cache.
        /// </summary>
        [Test]
        public void SerializeWithContext_ModifyAllowUnmarkedStructs_ClearsProcessorCache()
        {
            // Arrange
            var context = new SerializationContext
            {
                AllowUnmarkedStructs = true
            };
            var original = new StructContainerClass(
                new UnmarkedStruct(10, 20),
                new SerializableStruct(100, "test"),
                new EasySerializableStruct(3.5f, true)
            );

            // Act - Serialize with AllowUnmarkedStructs=true
            string json1 = EasySerializer.SerializeToJson(ref original, context: context);
            var result1 = EasySerializer.DeserializeFromJson<StructContainerClass>(json1, context: context);

            // Modify context to AllowUnmarkedStructs=false
            context.AllowUnmarkedStructs = false;

            // Serialize again with new settings
            string json2 = EasySerializer.SerializeToJson(ref original, context: context);
            var result2 = EasySerializer.DeserializeFromJson<StructContainerClass>(json2, context: context);

            // Assert
            // First serialization should include unmarked structs
            Assert.AreEqual(10, result1.UnmarkedField.X, "First: Unmarked struct X should be serialized");
            Assert.AreEqual(20, result1.UnmarkedField.Y, "First: Unmarked struct Y should be serialized");

            // Second serialization should NOT include unmarked structs
            Assert.AreEqual(0, result2.UnmarkedField.X, "Second: Unmarked struct X should NOT be serialized");
            Assert.AreEqual(0, result2.UnmarkedField.Y, "Second: Unmarked struct Y should NOT be serialized");
        }

        #endregion

        #region AllowAnonymousTypes

        /// <summary>
        /// Verifies that AllowAnonymousTypes=true allows serialization of anonymous types.
        /// </summary>
        /// <remarks>
        /// Anonymous types have read-only properties and cannot be deserialized.
        /// This test verifies serialization by checking the JSON string output.
        /// </remarks>
        [Test]
        public void SerializeWithContext_AllowAnonymousTypesTrue_SerializesAnonymousTypes()
        {
            // Arrange
            var context = new SerializationContext
            {
                AllowAnonymousTypes = true,
                MemberFlags = SerializableMemberFlags.AllPublic
            };
            var anonymousData = new { Name = "Test", Value = 42, IsActive = true };
            var original = new AnonymousTypeContainerClass(anonymousData);

            // Act
            string json = EasySerializer.SerializeToJson(ref original, context: context);

            // Assert - Anonymous type should be serialized (verified via string matching)
            // Note: Anonymous types have read-only properties and cannot be deserialized
            Assert.IsTrue(json.Contains("\"Name\":\"Test\""), "JSON should contain Name property");
            Assert.IsTrue(json.Contains("\"Value\":42"), "JSON should contain Value property");
            Assert.IsTrue(json.Contains("\"IsActive\":true"), "JSON should contain IsActive property");
        }

        #endregion
    }
}
