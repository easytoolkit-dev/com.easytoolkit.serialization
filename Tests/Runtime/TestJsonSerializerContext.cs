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
    }
}
