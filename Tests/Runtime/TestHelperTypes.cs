using System;
using System.Collections.Generic;
using EasyToolkit.Serialization;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Processors;

namespace EasyToolkit.Serialization.Tests
{
    /// <summary>Test enum for serialization testing.</summary>
    public enum TestEnum
    {
        OptionA = 0,
        OptionB = 1,
        OptionC = 2
    }

    public interface IInventorySlot
    {
    }

    [Serializable]
    public class InventorySlot : IInventorySlot
    {
        public int Id;
        public string Name;
    }

    [EasySerializable(Ignore = true)]
    public class InventoryIgnoreSerializeSlot : IInventorySlot
    {
        public int Id;
        public string Name;
    }

    /// <summary>Test data class for serialization testing.</summary>
    [Serializable]
    public class TestDataClass
    {
        public int Id;
        public string Name;
        public float Health;
        public bool IsActive;
        public UnityEngine.Vector3 Position;
        public List<int> Scores;
        public byte[] Data;
        public int? OptionalId;
        public float? OptionalHealth;
        public bool? OptionalIsActive;
        public DateTime? OptionalTimestamp;
        public Guid? OptionalGuid;
        public IInventorySlot InventorySlot;
        public IInventorySlot IgnoreSerializeInventorySlot;
    }


    /// <summary>Test class with default member flags (AllFields).</summary>
    [EasySerializable]
    public class DefaultMemberFlagsClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;
        [UnityEngine.SerializeField] protected int protectedField;
        [UnityEngine.SerializeField] internal int internalField;

        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }
        protected int ProtectedProperty { get; set; }

        public DefaultMemberFlagsClass()
        {
        }

        public DefaultMemberFlagsClass(
            int publicField, int privateField, int protectedField, int internalField,
            int publicProperty, int privateProperty, int protectedProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.protectedField = protectedField;
            this.internalField = internalField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
            this.ProtectedProperty = protectedProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetProtectedField() => protectedField;
        public int GetPrivateProperty() => PrivateProperty;
        public int GetProtectedProperty() => ProtectedProperty;
    }

    /// <summary>Test class with PublicFields only.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.PublicFields)]
    public class PublicFieldsOnlyClass
    {
        public int publicField;
        private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public PublicFieldsOnlyClass()
        {
        }

        public PublicFieldsOnlyClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with PublicProperties only.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.PublicProperties)]
    public class PublicPropertiesOnlyClass
    {
        public int publicField;
        private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public PublicPropertiesOnlyClass()
        {
        }

        public PublicPropertiesOnlyClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with AllPublic (fields and properties).</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.AllPublic)]
    public class AllPublicClass
    {
        public int publicField;
        private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public AllPublicClass()
        {
        }

        public AllPublicClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with NonPublicFields only.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.NonPublicFields)]
    public class NonPublicFieldsOnlyClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;

        public NonPublicFieldsOnlyClass()
        {
        }

        public NonPublicFieldsOnlyClass(int publicField, int privateField)
        {
            this.publicField = publicField;
            this.privateField = privateField;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
    }

    /// <summary>Test class with AllFields (public and non-public).</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.AllFields)]
    public class AllFieldsClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;
        [UnityEngine.SerializeField] protected int protectedField;

        public AllFieldsClass()
        {
        }

        public AllFieldsClass(int publicField, int privateField, int protectedField)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.protectedField = protectedField;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetProtectedField() => protectedField;
    }

    /// <summary>Test class with AllProperties (public and non-public).</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.AllProperties)]
    public class AllPropertiesClass
    {
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }
        protected int ProtectedProperty { get; set; }

        public AllPropertiesClass()
        {
        }

        public AllPropertiesClass(int publicProperty, int privateProperty, int protectedProperty)
        {
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
            this.ProtectedProperty = protectedProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateProperty() => PrivateProperty;
        public int GetProtectedProperty() => ProtectedProperty;
    }

    /// <summary>Test class with All members.</summary>
    [EasySerializable(MemberFlags = SerializableMemberFlags.All)]
    public class AllMembersClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        public AllMembersClass()
        {
        }

        public AllMembersClass(int publicField, int privateField, int publicProperty, int privateProperty)
        {
            this.publicField = publicField;
            this.privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
        public int GetPrivateProperty() => PrivateProperty;
    }

    /// <summary>Test class with RequireSerializeFieldOnNonPublic enabled.</summary>
    [EasySerializable(RequireSerializeFieldOnNonPublic = true)]
    public class RequireSerializeFieldClass
    {
        public int publicField;
        [UnityEngine.SerializeField] private int privateFieldWithAttribute;
        private int privateFieldWithoutAttribute;

        public RequireSerializeFieldClass()
        {
        }

        public RequireSerializeFieldClass(int publicField, int privateFieldWithAttribute,
            int privateFieldWithoutAttribute)
        {
            this.publicField = publicField;
            this.privateFieldWithAttribute = privateFieldWithAttribute;
            this.privateFieldWithoutAttribute = privateFieldWithoutAttribute;
        }

        // Getter methods for testing non-public members
        public int GetPrivateFieldWithAttribute() => privateFieldWithAttribute;
        public int GetPrivateFieldWithoutAttribute() => privateFieldWithoutAttribute;
    }

    /// <summary>Test class with RequireSerializeFieldOnNonPublic disabled.</summary>
    [EasySerializable(RequireSerializeFieldOnNonPublic = false)]
    public class NotRequireSerializeFieldClass
    {
        public int publicField;
        private int privateField;

        public NotRequireSerializeFieldClass()
        {
        }

        public NotRequireSerializeFieldClass(int publicField, int privateField)
        {
            this.publicField = publicField;
            this.privateField = privateField;
        }

        // Getter methods for testing non-public members
        public int GetPrivateField() => privateField;
    }

    /// <summary>Base class with AllocInherit disabled.</summary>
    [EasySerializable(AllocInherit = false)]
    public class BaseClassNoInherit
    {
        public int baseField;

        public BaseClassNoInherit(int value)
        {
            baseField = value;
        }
    }

    /// <summary>Derived class from BaseClassNoInherit (should not inherit).</summary>
    public class DerivedFromNoInherit : BaseClassNoInherit
    {
        public int derivedField;

        public DerivedFromNoInherit(int baseValue, int derivedValue) : base(baseValue)
        {
            derivedField = derivedValue;
        }
    }

    /// <summary>Base class with AllocInherit enabled.</summary>
    [EasySerializable(AllocInherit = true, MemberFlags = SerializableMemberFlags.AllPublic)]
    public class BaseClassWithInherit
    {
        public int baseField;
        private int basePrivateField;

        public BaseClassWithInherit(int publicValue, int privateValue)
        {
            baseField = publicValue;
            basePrivateField = privateValue;
        }

        // Getter methods for testing non-public members
        public int GetBasePrivateField() => basePrivateField;
    }

    /// <summary>Derived class from BaseClassWithInherit (should inherit).</summary>
    public class DerivedFromWithInherit : BaseClassWithInherit
    {
        public int derivedField;

        public DerivedFromWithInherit(int basePublicValue, int basePrivateValue, int derivedValue)
            : base(basePublicValue, basePrivateValue)
        {
            derivedField = derivedValue;
        }
    }

    /// <summary>Base class without attribute.</summary>
    public class BaseClassNoAttribute
    {
        public int baseField;

        public BaseClassNoAttribute(int value)
        {
            baseField = value;
        }
    }

    /// <summary>Derived class with attribute.</summary>
    [EasySerializable]
    public class DerivedWithAttribute : BaseClassNoAttribute
    {
        public int derivedField;

        public DerivedWithAttribute(int baseValue, int derivedValue) : base(baseValue)
        {
            derivedField = derivedValue;
        }
    }

    /// <summary>Test unmanaged struct for FormatGenericPrimitive testing.</summary>
    public struct TestUnmanagedStruct
    {
        public int X;
        public float Y;
        public byte Z;

        public TestUnmanagedStruct(int x, float y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            return obj is TestUnmanagedStruct other &&
                   X == other.X &&
                   Y.Equals(other.Y) &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }

    #region Context Test Classes

    /// <summary>Test class with EasySerializable but no explicit settings for context testing.</summary>
    [EasySerializable]
    public class ContextTestClass
    {
        public int publicField;
        private int _privateField;
        public int PublicProperty { get; set; }
        private int PrivateProperty { get; set; }

        [UnityEngine.SerializeField] private int _privateFieldWithAttribute;
        [NonSerialized] public int publicFieldWithNonSerialized;

        public ContextTestClass()
        {
        }

        public ContextTestClass(int publicField, int privateField, int publicProperty,
            int privateProperty, int privateFieldWithAttribute, int publicFieldWithNonSerialized)
        {
            this.publicField = publicField;
            this._privateField = privateField;
            this.PublicProperty = publicProperty;
            this.PrivateProperty = privateProperty;
            this._privateFieldWithAttribute = privateFieldWithAttribute;
            this.publicFieldWithNonSerialized = publicFieldWithNonSerialized;
        }

        public int GetPrivateField() => _privateField;
        public int GetPrivateProperty() => PrivateProperty;
        public int GetPrivateFieldWithAttribute() => _privateFieldWithAttribute;
    }

    /// <summary>Test class with NonSerialized field for ExcludeNonSerialized testing.</summary>
    [EasySerializable]
    public class NonSerializedTestClass
    {
        public int normalField;
        [NonSerialized] public int nonSerializedField;

        public NonSerializedTestClass()
        {
        }

        public NonSerializedTestClass(int normalField, int nonSerializedField)
        {
            this.normalField = normalField;
            this.nonSerializedField = nonSerializedField;
        }
    }

    #endregion

    #region Struct Test Types

    /// <summary>Unmarked struct without Serializable or EasySerializable attribute.</summary>
    public struct UnmarkedStruct
    {
        public int X;
        public int Y;

        public UnmarkedStruct(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>Struct marked with Serializable attribute.</summary>
    [System.Serializable]
    public struct SerializableStruct
    {
        public int Value;
        public string Name;

        public SerializableStruct(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }

    /// <summary>Struct marked with EasySerializable attribute.</summary>
    [EasySerializable]
    public struct EasySerializableStruct
    {
        public float Score;
        public bool IsActive;

        public EasySerializableStruct(float score, bool isActive)
        {
            Score = score;
            IsActive = isActive;
        }
    }

    /// <summary>Test class containing struct fields for AllowUnmarkedStructs testing.</summary>
    [EasySerializable]
    public class StructContainerClass
    {
        public UnmarkedStruct UnmarkedField;
        public SerializableStruct SerializableField;
        public EasySerializableStruct EasySerializableField;

        public StructContainerClass()
        {
        }

        public StructContainerClass(UnmarkedStruct unmarked, SerializableStruct serializable,
            EasySerializableStruct easySerializable)
        {
            UnmarkedField = unmarked;
            SerializableField = serializable;
            EasySerializableField = easySerializable;
        }
    }

    #endregion

    #region Anonymous Type Test Classes

    /// <summary>Test class with object field that can hold anonymous types.</summary>
    [EasySerializable]
    public class AnonymousTypeContainerClass
    {
        public object Data;

        public AnonymousTypeContainerClass()
        {
        }

        public AnonymousTypeContainerClass(object data)
        {
            Data = data;
        }
    }

    #endregion

    #region Circular Dependency Test Types

    /// <summary>
    /// Test class for circular dependency testing.
    /// </summary>
    [Serializable]
    public class CircularDependencyTestClass
    {
        public int Value;
    }

    /// <summary>
    /// Test class for safe recursive dependency testing.
    /// </summary>
    [Serializable]
    public class SafeRecursiveDependencyTestClass
    {
        public int Value;
    }

    #endregion

    #region Circular Dependency Test Processors

    /// <summary>
    /// Test processor that injects ISerializationProcessor<T> for the same type T
    /// without excluding itself, which should trigger circular dependency detection.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Custom)]
    public class CircularDependencyTestProcessor : SerializationProcessor<CircularDependencyTestClass>
    {
        [DependencyProcessor]
        private readonly ISerializationProcessor<CircularDependencyTestClass> _dependency;

        public override bool CanProcess(Type valueType, SerializationContext context)
        {
            return valueType == typeof(CircularDependencyTestClass);
        }

        protected override void Process(ref CircularDependencyTestClass value, IDataFormatter formatter)
        {
            // Implementation not needed for this test
        }
    }

    /// <summary>
    /// Test processor that safely injects ISerializationProcessor<T> by using
    /// ExcludedTypesGetter to exclude itself, following the GenericPrimitiveProcessor pattern.
    /// </summary>
    [ProcessorConfiguration(ProcessorPriorityLevel.Custom)]
    public class SafeRecursiveDependencyTestProcessor : SerializationProcessor<SafeRecursiveDependencyTestClass>
    {
        private static readonly Type[] ExcludedTypes = { typeof(SafeRecursiveDependencyTestProcessor) };

        [DependencyProcessor(ExcludedTypesGetter = nameof(ExcludedTypes))]
        private readonly ISerializationProcessor<SafeRecursiveDependencyTestClass> _dependency;

        public override bool CanProcess(Type valueType, SerializationContext context)
        {
            return valueType == typeof(SafeRecursiveDependencyTestClass);
        }

        protected override void Process(ref SafeRecursiveDependencyTestClass value, IDataFormatter formatter)
        {
            // Implementation not needed for this test
        }
    }

    #endregion

    #region Runtime Type Serialization Test Types

    /// <summary>Base class for testing runtime type serialization.</summary>
    [EasySerializable]
    public class Animal
    {
        public string Name;

        public Animal()
        {
        }

        public Animal(string name)
        {
            Name = name;
        }
    }

    /// <summary>Derived class for testing runtime type serialization.</summary>
    [EasySerializable]
    public class Dog : Animal
    {
        public string Breed;

        public Dog()
        {
        }

        public Dog(string name, string breed) : base(name)
        {
            Breed = breed;
        }
    }

    /// <summary>Another derived class for testing runtime type serialization.</summary>
    [EasySerializable]
    public class Cat : Animal
    {
        public bool IsIndoor;

        public Cat()
        {
        }

        public Cat(string name, bool isIndoor) : base(name)
        {
            IsIndoor = isIndoor;
        }
    }

    /// <summary>Container class for testing polymorphic serialization.</summary>
    [EasySerializable]
    public class PetContainer
    {
        public Animal Pet;

        public PetContainer()
        {
        }

        public PetContainer(Animal pet)
        {
            Pet = pet;
        }
    }

    /// <summary>Container class with multiple animals for testing runtime type serialization.</summary>
    [EasySerializable]
    public class ZooContainer
    {
        public Animal PrimaryAnimal;
        public Animal SecondaryAnimal;

        public ZooContainer()
        {
        }

        public ZooContainer(Animal primary, Animal secondary)
        {
            PrimaryAnimal = primary;
            SecondaryAnimal = secondary;
        }
    }

    #endregion

    #region ReturnDefaultOnEmptyMember Test Types

    /// <summary>Version 1 of player data for testing forward compatibility.</summary>
    [EasySerializable]
    public class PlayerDataV1
    {
        public int PlayerId;
        public string PlayerName;
        public float Health;

        public PlayerDataV1()
        {
        }

        public PlayerDataV1(int playerId, string playerName, float health)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Health = health;
        }
    }

    /// <summary>Version 2 of player data with additional fields for testing forward compatibility.</summary>
    [EasySerializable]
    public class PlayerDataV2
    {
        public int PlayerId;
        public string PlayerName;
        public float Health;
        public int Level;              // New field in V2
        public float Experience;        // New field in V2
        public bool IsPremium;          // New field in V2
        public List<string> Items;      // New field in V2

        public PlayerDataV2()
        {
        }

        public PlayerDataV2(int playerId, string playerName, float health, int level, float experience, bool isPremium, List<string> items)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            Health = health;
            Level = level;
            Experience = experience;
            IsPremium = isPremium;
            Items = items;
        }
    }

    /// <summary>Empty test class for ReturnDefaultOnEmptyMember testing.</summary>
    [EasySerializable]
    public class EmptyTestClass
    {
        public int Value;
        public string Name;

        public EmptyTestClass()
        {
        }

        public EmptyTestClass(int value, string name)
        {
            Value = value;
            Name = name;
        }
    }

    #endregion
}
