using System;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Serialization.Formatters;
using EasyToolkit.Serialization.Resolvers;
using EasyToolkit.Serialization.Utilities;
using UnityEngine.Assertions;

namespace EasyToolkit.Serialization.Processors
{
    public partial class GenericProcessor<T>
    {
        private void WriteMember(ref T value, SerializationMemberDefinition memberDefinition, IWritingFormatter formatter)
        {
            var getter = memberDefinition.ValueGetter;
            if (getter == null)
            {
                throw new SerializationException(
                    $"Cannot serialize member '{memberDefinition.Name}' on type '{typeof(T)}'. "
                    + $"The member does not have a readable getter. Ensure the member is either a field or a property with a get accessor.");
            }

            object memberValue;
            try
            {
                memberValue = getter(value);
            }
            catch (Exception ex)
            {
                throw new SerializationException(
                    $"Failed to get value from member '{memberDefinition.Name}' on type '{typeof(T)}'. "
                    + $"The getter threw an exception of type '{ex.GetType()}'. "
                    + $"Check that the getter is not performing invalid operations during serialization.", ex);
            }

            formatter.BeginMember(memberDefinition.Name);
            var isNull = memberValue == null;
            if (!typeof(T).IsValueType)
            {
                formatter.FormatNullable(ref isNull);
            }
            else
            {
                isNull = false;
            }

            if (!isNull)
            {
                // Use runtime type processor if enabled and value is not null
                if (memberDefinition.UseRuntimeType && memberValue != null)
                {
                    var runtimeType = memberValue.GetType();
                    // Only use runtime type if it differs from declared type
                    if (runtimeType != memberDefinition.MemberType)
                    {
                        // Check if runtime type is allowed based on member-level settings
                        if (runtimeType.IsAnonymousType() && !memberDefinition.AllowAnonymousTypes)
                        {
                            throw new SerializationException(
                                $"Cannot serialize member '{memberDefinition.Name}' with runtime anonymous type '{runtimeType}'. "
                                + $"Anonymous types are not allowed for this member. "
                                + $"Enable AllowAnonymousTypes in EasySerializableAttribute or SerializationContext.");
                        }

                        if (!runtimeType.IsDefined<SerializableAttribute>() &&
                            SerializedTypeUtility.GetDefinedEasySerializableAttribute(runtimeType) == null)
                        {
                            if (runtimeType.IsStructType() && !memberDefinition.AllowUnmarkedStructs)
                            {
                                throw new SerializationException(
                                    $"Cannot serialize member '{memberDefinition.Name}' with runtime struct type '{runtimeType}'. "
                                    + $"Structs must be marked with [Serializable] or [EasySerializable]. "
                                    + $"Enable AllowUnmarkedStructs in EasySerializableAttribute or SerializationContext.");
                            }

                            if (!runtimeType.IsValueType && !memberDefinition.AllowNonSerializableTypes
                                                         && !runtimeType.IsAnonymousType())
                            {
                                throw new SerializationException(
                                    $"Cannot serialize member '{memberDefinition.Name}' with runtime type '{runtimeType}'. "
                                    + $"Reference types without serialization attributes are not allowed for this member. "
                                    + $"Enable AllowNonSerializableTypes in EasySerializableAttribute or SerializationContext.");
                            }
                        }

                        var runtimeProcessor = _processorByType.GetOrAdd(runtimeType, CreateProcessor);
                        if (runtimeProcessor == null)
                        {
                            throw new SerializationException(
                                $"Cannot serialize member '{memberDefinition.Name}' with runtime type '{runtimeType}'. "
                                + $"No suitable processor was found for this type. "
                                + $"Ensure the runtime type is marked with [Serializable] or [EasySerializable], "
                                + $"enable AllowNonSerializableTypes in SerializationContext or EasySerializableAttribute, "
                                + $"or disable UseRuntimeType in SerializationContext or EasySerializableAttribute.");
                        }

                        runtimeProcessor.ProcessUntyped(ref memberValue, formatter);
                    }
                    else
                    {
                        if (memberDefinition.Processor == null)
                        {
                            Assert.IsNotNull(memberDefinition.SerializationException);
                            throw memberDefinition.SerializationException;
                        }
                        memberDefinition.Processor.ProcessUntyped(ref memberValue, formatter);
                    }
                }
                else
                {
                    if (memberDefinition.Processor == null)
                    {
                        Assert.IsNotNull(memberDefinition.SerializationException);
                        throw memberDefinition.SerializationException;
                    }
                    memberDefinition.Processor.ProcessUntyped(ref memberValue, formatter);
                }
            }
            else
            {
                memberValue = null;
            }
        }
    }
}
